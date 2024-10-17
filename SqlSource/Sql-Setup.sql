DECLARE @TableName NVARCHAR(255);
DECLARE @SQL NVARCHAR(MAX);

-- Cursor to iterate through all tables in the current database
DECLARE table_cursor CURSOR FOR
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';

OPEN table_cursor;

FETCH NEXT FROM table_cursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @SQL = 'IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''' + @TableName + ''') ' + CHAR(10) +
                'BEGIN' + CHAR(10) +
                '    CREATE TABLE [' + @TableName + '] (';
    
    SELECT @SQL = @SQL + CHAR(10) + '        [' + COLUMN_NAME + '] ' + 
                   DATA_TYPE + 
                   CASE 
                       WHEN CHARACTER_MAXIMUM_LENGTH IS NOT NULL AND DATA_TYPE IN ('varchar', 'nvarchar', 'char', 'nchar') 
                       THEN '(' + 
                            CASE 
                                WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN 'MAX' 
                                ELSE CAST(CHARACTER_MAXIMUM_LENGTH AS NVARCHAR(10)) 
                            END + ')' 
                       ELSE '' 
                   END +
                   CASE 
                       WHEN COLUMN_DEFAULT IS NOT NULL THEN ' DEFAULT ' + COLUMN_DEFAULT 
                       ELSE '' 
                   END +
                   CASE 
                       WHEN IS_NULLABLE = 'NO' THEN ' NOT NULL' 
                       ELSE ' NULL' 
                   END + ',' 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = @TableName;

    -- Remove the trailing comma
    SET @SQL = LEFT(@SQL, LEN(@SQL) - 1) + CHAR(10) + '    );' + CHAR(10) +
                'END';

    PRINT @SQL; -- Print the CREATE TABLE statement

    FETCH NEXT FROM table_cursor INTO @TableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

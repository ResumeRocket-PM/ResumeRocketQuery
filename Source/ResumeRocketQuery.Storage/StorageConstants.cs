namespace ResumeRocketQuery.Storage
{
    public static class StorageConstants
    {
        public static class StoredProcedures
        {
            public static string InsertAccount = "[ResumeRocketQueryService].[usp_InsertAccount]";
            public static string SelectAccount = "[ResumeRocketQueryService].[usp_SelectAccount]";

            public static string InsertEmailAddress = "[ResumeRocketQueryService].[usp_InsertEmailAddress]";
            public static string SelectEmailAddress = "[ResumeRocketQueryService].[usp_SelectEmailAddress]";
            public static string SelectEmailAddressByEmailAddress = "[ResumeRocketQueryService].[usp_SelectEmailAddressByEmailAddress]";
            public static string SelectEmailAddressByAccountId = "[ResumeRocketQueryService].[usp_SelectEmailAddressByAccountID]";
        }
    }
}

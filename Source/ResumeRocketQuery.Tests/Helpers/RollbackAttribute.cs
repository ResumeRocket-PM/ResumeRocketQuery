using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System;
using Xunit.Sdk;
using System.Transactions;
using System.Data.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RollbackAttribute : BeforeAfterTestAttribute
{
    private TransactionScope _transactionScope;

    public override void After(MethodInfo methodUnderTest)
    {
        _transactionScope.Dispose();
    }

    public override void Before(MethodInfo methodUnderTest)
    {
        TransactionOptions transactionOptions = new TransactionOptions();

        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

        _transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
    }
}
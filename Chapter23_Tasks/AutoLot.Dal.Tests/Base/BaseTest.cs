using System;
using System.Data;
using AutoLot.Dal.EfStructures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace AutoLot.Dal.Tests.Base
{
    public abstract class BaseTest : IDisposable
    {
        protected readonly IConfiguration Configuration;
        protected readonly ApplicationDbContext Context;

        public virtual void Dispose()
        {
            Context.Dispose();
        }

        protected BaseTest()
        {
            Configuration = TestHelpers.GetConfiguration();
            Context = TestHelpers.GetContext(Configuration);
        }

        protected void ExecuteInATransaction(Action actionToExecute)
        {
            var strategy = Context.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using var trans = Context.Database.BeginTransaction();
                actionToExecute();
                trans.Rollback();
            });
        }

        protected void ExecuteInASharedTransaction(Action<IDbContextTransaction> actionToExecute)
        {
            var strategy = Context.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using IDbContextTransaction trans =
                Context.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
                actionToExecute(trans);
                trans.Rollback();
            });
        }
    }
}

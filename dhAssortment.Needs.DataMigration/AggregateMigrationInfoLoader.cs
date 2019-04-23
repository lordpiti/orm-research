namespace dhAssortment.Needs.DataMigration
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentMigrator;
    using FluentMigrator.Expressions;
    using FluentMigrator.Infrastructure;
    using FluentMigrator.Runner;
    using FluentMigrator.Runner.Initialization;
    using Microsoft.Extensions.Options;

    public class AggregateMigrationInfoLoader : DefaultMigrationInformationLoader, IMigrationInformationLoader
    {
        private readonly IMigrationContext myMigrationContext;

        public AggregateMigrationInfoLoader(
            IMigrationContext migrationContext,
            IMigrationSource source,
            IOptionsSnapshot<TypeFilterOptions> filterOptions,
            IMigrationRunnerConventions conventions,
            IOptions<RunnerOptions> runnerOptions)
            : base(source, filterOptions, conventions, runnerOptions)
        {
            this.myMigrationContext = migrationContext;
        }

        SortedList<long, IMigrationInfo> IMigrationInformationLoader.LoadMigrations()
        {
            var migrationList = this.LoadMigrations();

            foreach (var item in migrationList)
            {
                item.Value.Migration.GetUpExpressions(this.myMigrationContext);

                // ((MigrationTestExtended)item.Value.Migration).ApplicationContext = this.myMigrationContext;
            }

            // Loop over the expressions, and put the add/remove columns into the right table expression one
            var test = this.myMigrationContext.Expressions.Where(x => x is CreateTableExpression).Cast<CreateTableExpression>();
            var test2 = this.myMigrationContext.Expressions.Where(x => x is CreateColumnExpression).Cast<CreateColumnExpression>();

            var productTableExpression = test.FirstOrDefault(x => x.TableName == "person");
            var columnExpression = test2.FirstOrDefault(x => x.TableName == "person");
            productTableExpression.Columns.Add(columnExpression.Column);
            this.myMigrationContext.Expressions.Remove(columnExpression);

            var lastMigration = migrationList.LastOrDefault();

            // ((MigrationTestExtended)lastMigration.Value.Migration).ApplicationContext = this.myMigrationContext;
            var aggreagateMigrationInfo = new MigrationInfo(
                lastMigration.Value.Version,
                lastMigration.Value.TransactionBehavior,
                new AggregateMigration(this.myMigrationContext.Expressions));
            var newList = new SortedList<long, IMigrationInfo>();
            newList.Add(lastMigration.Key, aggreagateMigrationInfo);

            // var expressionList = this.myMigrationContext.Expressions;
            return newList;
        }
    }

    public class AggregateMigration : IMigration
    {
        private readonly ICollection<IMigrationExpression> expressions;

        public AggregateMigration(ICollection<IMigrationExpression> expressions)
        {
            this.expressions = expressions;
        }

        public object ApplicationContext { get; private set; }

        public string ConnectionString { get; private set; }

        public void GetDownExpressions(IMigrationContext context)
        {
            throw new System.NotImplementedException();
        }

        public void GetUpExpressions(IMigrationContext context)
        {
            this.ApplicationContext = context.ApplicationContext;
            this.ConnectionString = context.Connection;
            context.Expressions = this.expressions;
        }
    }
}

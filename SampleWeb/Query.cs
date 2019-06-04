using System.Collections.Generic;
using System.Linq;
using GraphQL.EntityFramework;
using GraphQL.Types;
using SampleWeb.DataContext;

#region QueryUsedInController

public class Query :
    QueryGraphType<TestContext>
{
    public Query(IEfGraphQLService<TestContext> efGraphQlService) :
        base(efGraphQlService)
    {
        AddQueryField(
            name: "companies",
            resolve: context => context.DbContext.Companies);

        #endregion

        AddSingleField(
            resolve: context => context.DbContext.Companies,
            name: "company");

        AddQueryConnectionField(
            name: "companiesConnection",
            resolve: context => context.DbContext.Companies);

        AddQueryField(
            name: "employees",
            resolve: context => context.DbContext.Employees);

        AddQueryField(
            name: "employeesByArgument",
            resolve: context =>
            {
                var content = context.GetArgument<string>("content");
                return context.DbContext.Employees.Where(x => x.Content == content);
            },
            arguments: new QueryArguments(
                new QueryArgument<StringGraphType>
                {
                    Name = "content"
                }));

        AddQueryConnectionField(
            name: "employeesConnection",
            resolve: context => context.DbContext.Employees);

        #region ManuallyApplyWhere

        Field<ListGraphType<EmployeeSummaryGraph>>(
            name: "employeeSummary",
            arguments: new QueryArguments(
                new QueryArgument<ListGraphType<WhereExpressionGraph>>
                {
                    Name = "where"
                }
            ),
            resolve: context =>
            {
                var dbContext = (TestContext) context.UserContext;
                IQueryable<Employee> query = dbContext.Employees;

                if (context.HasArgument("where"))
                {
                    var wheres = context.GetArgument<List<WhereExpression>>("where");
                    foreach (var where in wheres)
                    {
                        var predicate = ExpressionBuilder<Employee>.BuildPredicate(where);
                        query = query.Where(predicate);
                    }
                }

                return from q in query
                    group q by new {q.CompanyId}
                    into g
                    select new EmployeeSummary
                    {
                        CompanyId = g.Key.CompanyId,
                        AverageAge = g.Average(x => x.Age),
                    };
            });

        #endregion
    }
}
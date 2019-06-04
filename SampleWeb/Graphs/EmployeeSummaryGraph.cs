using GraphQL.EntityFramework;
using SampleWeb.DataContext;

public class EmployeeSummaryGraph :
    EfObjectGraphType<TestContext, EmployeeSummary>
{
    public EmployeeSummaryGraph(IEfGraphQLService<TestContext> graphQlService) :
        base(graphQlService)
    {
        Field(x => x.CompanyId);
        Field(x => x.AverageAge);
    }
}
using GraphQL.EntityFramework;
using SampleWeb.DataContext;

public class EmployeeGraph :
    EfObjectGraphType<TestContext, Employee>
{
    public EmployeeGraph(IEfGraphQLService<TestContext> graphQlService) :
        base(graphQlService)
    {
        Field(x => x.Id);
        Field(x => x.Content);
        Field(x => x.Age);
        AddNavigationField(
            name: "company",
            resolve: context => context.Source.Company);
    }
}
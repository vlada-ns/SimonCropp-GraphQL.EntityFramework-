using GraphQL.EntityFramework;
using SampleWeb.DataContext;

public class CompanyGraph :
    EfObjectGraphType<TestContext, Company>
{
    public CompanyGraph(IEfGraphQLService<TestContext> graphQlService) :
        base(graphQlService)
    {
        Field(x => x.Id);
        Field(x => x.Content);
        AddNavigationListField(
            name: "employees",
            resolve: context => context.Source.Employees);
        AddNavigationConnectionField(
            name: "employeesConnection",
            resolve: context => context.Source.Employees,
            includeNames: new[] {"Employees"});
    }
}
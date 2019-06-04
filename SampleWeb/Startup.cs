using System;
using System.Collections.Generic;
using System.Linq;
using GraphiQl;
using GraphQL.EntityFramework;
using GraphQL;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SampleWeb.DataContext;
using Microsoft.EntityFrameworkCore;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        GraphTypeTypeRegistry.Register<Employee, EmployeeGraph>();
        GraphTypeTypeRegistry.Register<EmployeeSummary, EmployeeSummaryGraph>();
        GraphTypeTypeRegistry.Register<Company, CompanyGraph>();

        //services.AddScoped(provider => DbContextBuilder.BuildDbContext());

        //EfGraphQLConventions.RegisterInContainer(
        //    services,
        //    DbContextBuilder.BuildDbContext(),
        //    userContext => (GraphQlEfSampleDbContext) userContext);


        //services.AddDbContext<TestContext>(options => options.UseSqlServer(""));
        var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
        optionsBuilder.UseSqlServer("Data Source=VASIC;Initial Catalog=GraphQLEntityFrameworkSample;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        using (var context = new TestContext(optionsBuilder.Options))
        {
            EfGraphQLConventions.RegisterInContainer(
            services,
            context,
            userContext => (TestContext)userContext);
        }








        foreach (var type in GetGraphQlTypes())
        {
            services.AddSingleton(type);
        }

        services.AddGraphQL(options => options.ExposeExceptions = true).AddWebSockets();
        services.AddSingleton<ContextFactory>();
        services.AddSingleton<IDocumentExecuter, EfDocumentExecuter>();
        services.AddSingleton<IDependencyResolver>(
            provider => new FuncDependencyResolver(provider.GetRequiredService));
        services.AddSingleton<ISchema, Schema>();
        var mvc = services.AddMvc();
        mvc.SetCompatibilityVersion(CompatibilityVersion.Latest);
    }

    static IEnumerable<Type> GetGraphQlTypes()
    {
        return typeof(Startup).Assembly
            .GetTypes()
            .Where(x => !x.IsAbstract &&
                        (typeof(IObjectGraphType).IsAssignableFrom(x) ||
                         typeof(IInputObjectGraphType).IsAssignableFrom(x)));
    }

    public void Configure(IApplicationBuilder builder)
    {
        builder.UseWebSockets();
        builder.UseGraphQLWebSockets<ISchema>();
        builder.UseGraphiQl("/graphiql", "/graphql");
        builder.UseMvc();
    }
}
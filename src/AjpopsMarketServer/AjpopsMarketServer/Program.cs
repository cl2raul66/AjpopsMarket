using AjpopsMarketServer.Enums;
using AjpopsMarketServer.Repository;
using AjpopsMarketServer.Services;
using AjpopsMarketServer.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<ICatalogRepository, CatalogInLiteDbService>()
    .AddSingleton<ICategoryRepository, CategoryInLiteDbService>()
    .AddSingleton<IMemberRepository, MemberInLiteDbService>()
    .AddSingleton<IOrderRepository, OrderInLiteDbService>()
    .AddSingleton<IProductRepository, ProductInLiteDbService>()
    .AddSingleton<IUserRepository, UserInLiteDbService>();

builder
    .AddGraphQL()
    .AddTypes()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<UserType>();

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);

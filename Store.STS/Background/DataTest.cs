using ExcelDataReader;
using Grpc.Net.Client.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Store.STS.Data;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Store.STS.Background
{
    public class TestData : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public TestData(IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken);

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("admin-client") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "admin-client",
                    ClientSecret = "4c5755b7-d1a5-49a4-bae6-cb9df5de90f9",
                    DisplayName = "Default admin application",
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.Password,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.Prefixes.GrantType + "verification_token",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "mobile-api",
                    }
                });
            }

            if (await manager.FindByClientIdAsync("mobile-client") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "mobile-client",
                    ClientSecret = "6ba0d894-16c1-4e20-8d71-d4335cba4d45",
                    DisplayName = "Default mobile application",
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.Password,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.Prefixes.GrantType + "verification_token",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "mobile-api"
                    }
                });
            }

            await CreateIntrospectionClient(manager, "test-resource", "d023b5eb-a021-4e53-875e-72b7f69d0f73", cancellationToken);
            await CreateIntrospectionClient(manager, "product-resource", "d6c2de27-4cf7-4fb2-bc7a-55de58b86f94", cancellationToken);
            await CreateIntrospectionClient(manager, "localization-resource", "cab02c49-b8cb-4d0b-98f4-1f01831bb1c8", cancellationToken);
            await CreateIntrospectionClient(manager, "order-resource", "a431e1b9-aad7-47dc-bc29-7d28a78d9f3c", cancellationToken);
            await CreateScopesAsync(scope, cancellationToken);
        }
        private static async Task CreateIntrospectionClient(IOpenIddictApplicationManager manager,
            string clientId,
            string clientsecret,
            CancellationToken cancellationToken)
        {
            if (await manager.FindByClientIdAsync(clientId, cancellationToken) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = clientId,
                    ClientSecret = clientsecret,
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Introspection
                    }
                }, cancellationToken);
            }
        }
        private static async Task CreateScopesAsync(IServiceScope scope, CancellationToken cancellationToken)
        {
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

            if (await manager.FindByNameAsync("mobile-api", cancellationToken) == null)
            {
                var descriptor = new OpenIddictScopeDescriptor
                {
                    Name = "mobile-api",
                    Resources = {
                        "test-resource",
                        "order-resource"
                    }
                };
                await manager.CreateAsync(descriptor, cancellationToken);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}

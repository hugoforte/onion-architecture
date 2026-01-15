using NServiceBus;
using Starter.Infrastructure.Extensions;
using Starter.Services.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .UseConsoleLifetime()
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure(context.Configuration);
        services.AddApplication();
    })
    .UseNServiceBus(_ =>
    {
        var endpointConfiguration = new EndpointConfiguration("Starter.ServiceBus");

        var transport = endpointConfiguration.UseTransport<LearningTransport>();
        transport.StorageDirectory(".learning-transport");

        endpointConfiguration.AuditProcessedMessagesTo("audit");
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningCommandsAs(type => type.Namespace is not null && type.Namespace.Contains("Commands"));
        conventions.DefiningEventsAs(type => type.Namespace is not null && type.Namespace.Contains("Events"));

        return endpointConfiguration;
    })
    .Build();

await host.RunAsync();

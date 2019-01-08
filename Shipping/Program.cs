using NServiceBus;
using System;
using System.Threading.Tasks;

namespace Shipping
{
    using System.Data.SqlClient;
    using Messages;
    using NServiceBus.Features;
    using NServiceBus.Persistence.Sql;

    class Program
    {
        static async Task Main()
        {
            Console.Title = "Shipping";

            //var endpointConfiguration = new EndpointConfiguration("Shipping");

            //var transport = endpointConfiguration.UseTransport<LearningTransport>();
            //var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();

            //var endpointInstance = await Endpoint.Start(endpointConfiguration)
            //    .ConfigureAwait(false);

            //Console.WriteLine("Press Enter to exit.");
            //Console.ReadLine();

            //await endpointInstance.Stop()
            //    .ConfigureAwait(false);


            var endpointConfiguration = new EndpointConfiguration("Shipping");
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.EnableFeature<AutoSubscribe>();


            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);
            transport.ConnectionString("Server=.\\dev;Database=MessagingDB;Trusted_Connection=True;");
            
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            
            var subscriptions = persistence.SubscriptionSettings();
            //subscriptions.CacheFor(TimeSpan.FromMinutes(1));
            subscriptions.DisableCache();

            var routing = transport.Routing();
            routing.RegisterPublisher(typeof(OrderBilled), "Billing");
            routing.RegisterPublisher(typeof(OrderPlaced), "Sales");

            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(
                connectionBuilder: () => new SqlConnection("Server=.\\dev;Database=MessagingDB;Trusted_Connection=True;"));

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            
           

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();


            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
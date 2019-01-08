using System;
using System.Threading.Tasks;
using NServiceBus;

namespace Sales
{
    using System.Data.SqlClient;
    using NServiceBus.Persistence.Sql;

    class Program
    {
        static async Task Main()
        {
            Console.Title = "Sales";

            //var endpointConfiguration = new EndpointConfiguration("Sales");

            //var transport = endpointConfiguration.UseTransport<LearningTransport>();

            //var endpointInstance = await Endpoint.Start(endpointConfiguration)
            //    .ConfigureAwait(false);

            //Console.WriteLine("Press Enter to exit.");
            //Console.ReadLine();

            //await endpointInstance.Stop()
            //    .ConfigureAwait(false);


            var endpointConfiguration = new EndpointConfiguration("Sales");
            endpointConfiguration.EnableInstallers();
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);
            transport.ConnectionString("Server=.\\dev;Database=MessagingDB;Trusted_Connection=True;");

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var subscriptions = persistence.SubscriptionSettings();
            //subscriptions.CacheFor(TimeSpan.FromMinutes(1));
            subscriptions.DisableCache();
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
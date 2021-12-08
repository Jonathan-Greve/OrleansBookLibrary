using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using BookLibrary.GrainInterfaces;
using BookLibrary.Models;

namespace OrleansBasics
{
    public class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    await DoClientWork(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBookLibrary";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            // example of calling grains from the initialized client
            var libraryGrain = client.GetGrain<ILibraryGrain>(Guid.Empty);
            var response = await libraryGrain.GetBookCatalog();
            foreach (Book book in response)
            {
                Console.WriteLine(
                    $"Book) guid: {book.Guid.Value}, num_pages: {book.Pages.Count}, title: {book.Title.Value}");
            }

            var book2 = response[1];

            var bookManagerGrain = client.GetGrain<IBookManagerGrain>(book2.Guid.Value);
            var bookGrain = await bookManagerGrain.RequestBook();
            if (bookGrain != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    var page = await bookGrain?.CurrPage()!;
                    Console.WriteLine($"page: {page}");
                    var nextPage = await bookGrain.NextPage();
                }
                for (int i = 0; i < 5; i++)
                {
                    var page = await bookGrain?.CurrPage();
                    Console.WriteLine($"page: {page}");
                    var nextPage = await bookGrain.PrevPage();
                }
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using BookLibrary.GrainInterfaces;
using Orleans.TestingHost;
using Xunit;

namespace Tests
{
    public class BookManagerGrainTests
    {
        [Fact]
        public async Task RequestBookWithAvailableCopiesSucceed()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();

            var libraryGrain =
                cluster.GrainFactory.GetGrain<ILibraryGrain>(Guid.Empty);
            var bookCatalog = await libraryGrain.GetBookCatalog();
            var bookManagerGrain =
                cluster.GrainFactory.GetGrain<IBookManagerGrain>(
                    bookCatalog[0].Guid.Value);
            var bookGrain = await bookManagerGrain.RequestBook();
            
            cluster.StopAllSilos();

            Assert.NotNull(bookGrain);
        }

        [Fact]
        public async Task RequestBookWithNoAvailableCopiesReturnsNull()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();

            var libraryGrain =
                cluster.GrainFactory.GetGrain<ILibraryGrain>(Guid.Empty);
            var bookCatalog = await libraryGrain.GetBookCatalog();
            var bookManagerGrain =
                cluster.GrainFactory.GetGrain<IBookManagerGrain>(
                    bookCatalog[0].Guid.Value);
            for (int i = 0; i < 3; i++)
            {
                await bookManagerGrain.RequestBook();
            }

            var bookGrain = await bookManagerGrain.RequestBook();
            
            cluster.StopAllSilos();

            Assert.Null(bookGrain);
        }

        [Fact]
        public async Task RequestNonExistentBookWithAvailableReturnsNull()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();

            var bookManagerGrain =
                cluster.GrainFactory.GetGrain<IBookManagerGrain>(Guid.NewGuid());
            var bookGrain = await bookManagerGrain.RequestBook();
            
            cluster.StopAllSilos();

            Assert.Null(bookGrain);
        }

        [Fact]
        public async Task RequestNonExistentBookWithNoAvailableReturnsNull()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();

            var bookManagerGrain =
                cluster.GrainFactory.GetGrain<IBookManagerGrain>(Guid.NewGuid());
            for (int i = 0; i < 3; i++)
            {
                await bookManagerGrain.RequestBook();
            }

            var bookGrain = await bookManagerGrain.RequestBook();

            cluster.StopAllSilos();

            Assert.Null(bookGrain);
        }
    }
}
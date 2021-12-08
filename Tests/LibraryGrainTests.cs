using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibrary.GrainInterfaces;
using BookLibrary.Models;
using Orleans;
using Orleans.TestingHost;
using Xunit;

namespace Tests
{
    public class BookLibraryTests
    {
        [Fact]
        public async Task GetBookCatalogReturnsAtleastOneBook()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();

            var libraryGrain =
                cluster.GrainFactory.GetGrain<ILibraryGrain>(Guid.NewGuid());
            var bookCatalog = await libraryGrain.GetBookCatalog();

            cluster.StopAllSilos();

            Assert.True(bookCatalog.Count > 0);
        }

        [Fact]
        public async Task GetBookFetchesCorrectBook()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();

            var libraryGrain =
                cluster.GrainFactory.GetGrain<ILibraryGrain>(Guid.NewGuid());
            var bookCatalog = await libraryGrain.GetBookCatalog();
            var books = new List<Book?>();

            foreach (var book in bookCatalog)
            {
                var receivedBook = await libraryGrain.GetBook(book.Guid.Value);
                books.Add(receivedBook);
            }
            cluster.StopAllSilos();

            Assert.Equal(bookCatalog, books);
        }
        
        [Fact]
        public async Task GetBookReturnsNullForNonExistentBook()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();

            var libraryGrain =
                cluster.GrainFactory.GetGrain<ILibraryGrain>(Guid.NewGuid());
            var bookCatalog = await libraryGrain.GetBookCatalog();
            var book = await libraryGrain.GetBook(Guid.NewGuid());
            cluster.StopAllSilos();

            Assert.Equal(null, book);
        }
    }
}
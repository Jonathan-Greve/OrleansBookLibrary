using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibrary.GrainInterfaces;
using BookLibrary.Models;
using Orleans.Concurrency;
using Orleans.TestingHost;
using Xunit;

namespace Tests
{
    public class BookGrainTests
    {
        [Fact]
        public async Task SetBookWillUpdateTheGrainsBook()
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
            var oldBookPage0 = await bookGrain.GoToPage(0);

            var newBook =
                new Book(new Immutable<Guid>(Guid.NewGuid()),
                    new List<string> {Guid.NewGuid().ToString()},
                    new Immutable<string>("newBookTitle"));
            await bookGrain.SetBook(newBook);
            var newBookPage0 = await bookGrain.GoToPage(0);

            cluster.StopAllSilos();

            Assert.NotEqual(oldBookPage0, newBookPage0);
            Assert.Equal(newBookPage0, newBook.Pages[0]);
        }
        
        [Fact]
        public async Task GoToPageWithinPageLimitReturnsCorrectPage()
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

            var page0 = await bookGrain.GoToPage(0);
            var page1 = await bookGrain.GoToPage(1);
            var page2 = await bookGrain.GoToPage(2);

            cluster.StopAllSilos();
            
            Assert.Equal(page0, bookCatalog[0].Pages[0]);
            Assert.Equal(page1, bookCatalog[0].Pages[1]);
            Assert.Equal(page2, bookCatalog[0].Pages[2]);
        }
        
        [Fact]
        public async Task GoToPageWithNegativeIndexReturnsNull()
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

            var page = await bookGrain.GoToPage(-1);

            cluster.StopAllSilos();
            
            Assert.Null(page);
        }
        
        [Fact]
        public async Task GoToPageWithIndexExceedingPageCountReturnNull()
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

            var page = await bookGrain.GoToPage(4);

            cluster.StopAllSilos();
            
            Assert.Null(page);
        }
        
        [Fact]
        public async Task NextPageWithinPageLimitsReturnsCorrectPage()
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

            var page1 = await bookGrain.NextPage();
            var page2 = await bookGrain.NextPage();

            cluster.StopAllSilos();
            
            Assert.Equal(page1, bookCatalog[0].Pages[1]);
            Assert.Equal(page2, bookCatalog[0].Pages[2]);
        }
        
        [Fact]
        public async Task NextPageWithIndexExceedingPageCountReturnNull()
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

            var page1 = await bookGrain.NextPage();
            var page2 = await bookGrain.NextPage();
            var page3 = await bookGrain.NextPage();

            cluster.StopAllSilos();
            
            Assert.Equal(page1, bookCatalog[0].Pages[1]);
            Assert.Equal(page2, bookCatalog[0].Pages[2]);
            Assert.Null(page3);
        }
        
        [Fact]
        public async Task NextPageOnNullBookReturnsNull()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();
            var bookGrain =
                cluster.GrainFactory.GetGrain<IBookGrain>(Guid.NewGuid(), "test");

            var nextPage = await bookGrain.NextPage();

            cluster.StopAllSilos();
            
            Assert.Null(nextPage);
        }
        
        [Fact]
        public async Task PrevPageWithinPageLimitsReturnsCorrectPage()
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

            await bookGrain.GoToPage(2);
            var page1 = await bookGrain.PrevPage();
            var page0 = await bookGrain.PrevPage();

            cluster.StopAllSilos();
            
            Assert.Equal(page1, bookCatalog[0].Pages[1]);
            Assert.Equal(page0, bookCatalog[0].Pages[0]);
        }
        
        [Fact]
        public async Task PrevPageWithNegativeReturnNull()
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

            var page = await bookGrain.PrevPage();

            cluster.StopAllSilos();
            
            Assert.Null(page);
        }
        
        [Fact]
        public async Task PrevPageOnNullBookReturnsNull()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();
            var bookGrain =
                cluster.GrainFactory.GetGrain<IBookGrain>(Guid.NewGuid(), "test");

            await bookGrain.GoToPage(1);
            var prevPage = await bookGrain.PrevPage();

            cluster.StopAllSilos();
            
            Assert.Null(prevPage);
        }
        
        [Fact]
        public async Task CurrPageReturnsTheCurrentPage()
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

            var page0 = await bookGrain.CurrPage();
            await bookGrain.NextPage();
            var page1 = await bookGrain.CurrPage();
            await bookGrain.NextPage();
            var page2 = await bookGrain.CurrPage();
            await bookGrain.NextPage();
            var page = await bookGrain.CurrPage();

            cluster.StopAllSilos();
            
            Assert.Equal(page0, bookCatalog[0].Pages[0]);
            Assert.Equal(page1, bookCatalog[0].Pages[1]);
            Assert.Equal(page2, bookCatalog[0].Pages[2]);
            Assert.Equal(page, bookCatalog[0].Pages[2]);
        }
        
        [Fact]
        public async Task CurrPageOnNullBookReturnsNull()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();
            var bookGrain =
                cluster.GrainFactory.GetGrain<IBookGrain>(Guid.NewGuid(), "test");

            var currPage = await bookGrain.CurrPage();

            cluster.StopAllSilos();
            
            Assert.Null(currPage);
        }
    }
}
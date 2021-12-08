using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibrary.GrainInterfaces;
using BookLibrary.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;

namespace BookLibrary
{
    namespace Grains
    {
        public class LibraryGrain : Grain, ILibraryGrain
        {
            private readonly ILogger _logger;

            private readonly List<Book> _bookCatalog = new()
            {
                new Book(
                    guid: new Immutable<Guid>(Guid.NewGuid()),
                    pages: new List<string>
                    {
                        "Book 1, Page 1",
                        "Book 1, Page 2",
                        "Book 1, Page 3"
                    },
                    title: new Immutable<string>("Book 1")),
                new Book(
                    guid: new Immutable<Guid>(Guid.NewGuid()),
                    pages: new List<string>
                    {
                        "Book 2, Page 1",
                        "Book 2, Page 2",
                        "Book 2, Page 3"
                    },
                    title: new Immutable<string>("Book 2")),
                new Book(
                    guid: new Immutable<Guid>(Guid.NewGuid()),
                    pages: new List<string>
                    {
                        "Book 3, Page 1",
                        "Book 3, Page 2",
                        "Book 3, Page 3"
                    },
                    title: new Immutable<string>("Book 3"))
            };

            public LibraryGrain(ILogger<BookGrain> logger)
            {
                _logger = logger;
            }

            public Task<List<Book>> GetBookCatalog()
            {
                return Task.FromResult(_bookCatalog);
            }

            public Task<Book?> GetBook(Guid bookGuid)
            {
                Book? book = _bookCatalog.Find(book => book.Guid.Value == bookGuid);
                return Task.FromResult(book);
            }
        }
    }
}
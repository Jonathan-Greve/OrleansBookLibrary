using System;
using System.Threading.Tasks;
using BookLibrary.GrainInterfaces;
using BookLibrary.Models;
using Microsoft.Extensions.Logging;
using Orleans;

namespace BookLibrary
{
    namespace Grains
    {
        public class BookGrain : Grain, IBookGrain
        {
            private readonly ILogger _logger;
            private Book? _book;
            public int CurrPage { get; set; } = 0;

            public BookGrain(ILogger<BookGrain> logger)
            {
                this._logger = logger;
            }

            public Task<string?> NextPage()
            {
                _logger.LogInformation($"NextPage called. Book: {_book?.Pages[CurrPage]}");
                if (_book != null && CurrPage + 1 < _book.Pages.Count)
                {
                    CurrPage += 1;
                    return Task.FromResult<string?>(_book.Pages[CurrPage]);
                }

                return Task.FromResult<string?>(null);
            }

            public Task<string?> PrevPage()
            {
                _logger.LogInformation($"PrevPage called. Book: {_book?.Pages[CurrPage]}");
                if (_book != null && CurrPage - 1 >= 0)
                {
                    CurrPage -= 1;
                    return Task.FromResult<string?>(_book.Pages[CurrPage]);
                }

                return Task.FromResult<string?>(null);
            }

            public Task<string?> GoToPage(int pageIndex)
            {
                _logger.LogInformation($"GoToPage called. Book: {_book?.Pages[CurrPage]}");
                if (_book != null && 0 <= pageIndex && pageIndex < _book.Pages.Count)
                {
                    CurrPage = pageIndex;
                    return Task.FromResult<string?>(_book.Pages[pageIndex]);
                }

                return Task.FromResult<string?>(null);
            }

            Task<string?> IBookGrain.CurrPage()
            {
                _logger.LogInformation($"CurrPage called. Book: {_book?.Pages[CurrPage]}");
                if (_book != null && CurrPage >= 0 && CurrPage < _book.Pages.Count)
                {
                    return Task.FromResult<string?>(_book.Pages[CurrPage]);
                }

                return Task.FromResult<string?>(null);
            }

            public Task SetBook(Book book)
            {
                _logger.LogInformation($"SetBook called. Book: {_book?.Pages[CurrPage]}");
                _book = book;
                return Task.CompletedTask;
            }
        }
    }
}
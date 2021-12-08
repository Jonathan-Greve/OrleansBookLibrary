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
        public class BookManagerGrain : Grain, IBookManagerGrain
        {
            private readonly ILogger _logger;
            private int _availableCopies = 3;
            private int _totalCopiesGiven = 0;
            private Book? _book;

            public BookManagerGrain(ILogger<BookGrain> logger)
            {
                _logger = logger;
            }

            async Task<IBookGrain?> IBookManagerGrain.RequestBook()
            {
                _logger.LogInformation(
                    $"RequestBook called.");
                if (_availableCopies > 0)
                {
                    if (_book == null)
                    {
                        ILibraryGrain libraryGrain = GrainFactory.GetGrain<ILibraryGrain>(Guid.Empty);
                        _book = await libraryGrain.GetBook(this.GetPrimaryKey());
                    }

                    if (_book != null)
                    {
                        IBookGrain bookGrain = GrainFactory.GetGrain<IBookGrain>(
                            this.GetPrimaryKey(), _totalCopiesGiven.ToString());
                        await bookGrain.SetBook(_book);

                        _availableCopies -= 1;
                        _totalCopiesGiven += 1;

                        return bookGrain;
                    }
                }

                return null;
            }

            Task IBookManagerGrain.ReturnBook()
            {
                _logger.LogInformation(
                    $"ReturnBook called.");
                _availableCopies += 1;

                return Task.CompletedTask;
            }
        }
    }
}
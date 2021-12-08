using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibrary.Models;
using Orleans;

namespace BookLibrary
{
    namespace GrainInterfaces
    {
        public interface ILibraryGrain: IGrainWithGuidKey
        {
            Task<List<Book>> GetBookCatalog ();
            Task<Book?> GetBook(Guid bookGuid);
        }
    }
}
using System;
using System.Threading.Tasks;
using BookLibrary.Models;
using Orleans;

namespace BookLibrary
{
    namespace GrainInterfaces
    {
        public interface IBookGrain : IGrainWithGuidCompoundKey
        {
            /// <summary>
            /// Go to the next page. Nothing happens if already on the back cover.
            /// </summary>
            /// <returns></returns>
            Task<string?> NextPage();

            /// <summary>
            /// Go to the previous page. Nothing happens if already on the
            /// front cover.
            /// </summary>
            /// <returns></returns>
            Task<string?> PrevPage();

            /// <summary>
            /// Go to the zero indexed page, i.e. 0 is the cover.
            /// </summary>
            /// <param name="pageIndex"></param>
            /// <returns></returns>
            Task<string?> GoToPage(int pageIndex);
            
            Task<string?> CurrPage();

            /// <summary>
            /// Upload/set the book to be used by the grain
            /// </summary>
            /// <param name="book"></param>
            /// <returns></returns>
            Task SetBook(Book book);
        }
    }
}
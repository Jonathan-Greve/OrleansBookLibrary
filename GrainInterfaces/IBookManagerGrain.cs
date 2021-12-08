using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibrary.Models;
using Orleans;

namespace BookLibrary
{
    namespace GrainInterfaces
    {
        public interface IBookManagerGrain: IGrainWithGuidKey
        {
            Task<IBookGrain?> RequestBook();
            Task ReturnBook();
        }
    }
}
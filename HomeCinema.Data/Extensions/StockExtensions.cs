using System.Collections.Generic;
using System.Linq;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;

namespace HomeCinema.Data.Extensions
{
    public static class StockExtensions
    {
        public static IEnumerable<Stock> GetAvailableItems(this IEntityBaseRepository<Stock> stocksRepository,
            int movieId)
        {
            IEnumerable<Stock> _availableItems;

            _availableItems = stocksRepository.GetAll().Where(s => s.MovieId == movieId && s.IsAvailable);

            return _availableItems;
        }
    }
}
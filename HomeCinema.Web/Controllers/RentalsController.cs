using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using HomeCinema.Data.Extensions;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Web.Infrastructure.Core;
using HomeCinema.Web.Models;

namespace HomeCinema.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/rentals")]
    public class RentalsController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Customer> _customersRepository;
        private readonly IEntityBaseRepository<Movie> _moviesRepository;
        private readonly IEntityBaseRepository<Rental> _rentalsRepository;
        private readonly IEntityBaseRepository<Stock> _stocksRepository;

        public RentalsController(IEntityBaseRepository<Rental> rentalsRepository,
            IEntityBaseRepository<Customer> customersRepository, IEntityBaseRepository<Movie> moviesRepository,
            IEntityBaseRepository<Stock> stocksRepository,
            IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
            _rentalsRepository = rentalsRepository;
            _moviesRepository = moviesRepository;
            _customersRepository = customersRepository;
            _stocksRepository = stocksRepository;
        }

        [HttpPost]
        [Route("rent/{customerId:int}/{stockId:int}")]
        public HttpResponseMessage Rent(HttpRequestMessage request, int customerId, int stockId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var customer = _customersRepository.GetSingle(customerId);
                var stock = _stocksRepository.GetSingle(stockId);

                if (customer == null || stock == null)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid Customer or Stock");
                }
                else
                {
                    if (stock.IsAvailable)
                    {
                        var _rental = new Rental
                        {
                            CustomerId = customerId,
                            StockId = stockId,
                            RentalDate = DateTime.Now,
                            Status = "Borrowed"
                        };

                        _rentalsRepository.Add(_rental);

                        stock.IsAvailable = false;

                        _unitOfWork.Commit();

                        var rentalVm = Mapper.Map<Rental, RentalViewModel>(_rental);

                        response = request.CreateResponse(HttpStatusCode.Created, rentalVm);
                    }
                    else
                        response = request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            "Selected stock is not available anymore");
                }

                return response;
            });
        }

        [HttpPost]
        [Route("return/{rentalId:int}")]
        public HttpResponseMessage Return(HttpRequestMessage request, int rentalId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var rental = _rentalsRepository.GetSingle(rentalId);

                if (rental == null)
                    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid rental");
                else
                {
                    rental.Status = "Returned";
                    rental.Stock.IsAvailable = true;
                    rental.ReturnedDate = DateTime.Now;

                    _unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }

        [HttpGet]
        [Route("{id:int}/rentalhistory")]
        public HttpResponseMessage RentalHistory(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var _rentalHistory = GetMovieRentalHistory(id);

                response = request.CreateResponse(HttpStatusCode.OK, _rentalHistory);

                return response;
            });
        }

        [HttpGet]
        [Route("rentalhistory")]
        public HttpResponseMessage TotalRentalHistory(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var _totalMoviesRentalHistory = new List<TotalRentalHistoryViewModel>();

                var movies = _moviesRepository.GetAll();

                foreach (var movie in movies)
                {
                    var _totalRentalHistory = new TotalRentalHistoryViewModel
                    {
                        ID = movie.ID,
                        Title = movie.Title,
                        Image = movie.Image,
                        Rentals = GetMovieRentalHistoryPerDates(movie.ID)
                    };

                    if (_totalRentalHistory.TotalRentals > 0)
                        _totalMoviesRentalHistory.Add(_totalRentalHistory);
                }

                response = request.CreateResponse(HttpStatusCode.OK, _totalMoviesRentalHistory);

                return response;
            });
        }

        #region Private methods

        private List<RentalHistoryViewModel> GetMovieRentalHistory(int movieId)
        {
            var _rentalHistory = new List<RentalHistoryViewModel>();
            var rentals = new List<Rental>();

            var movie = _moviesRepository.GetSingle(movieId);

            foreach (var stock in movie.Stocks)
            {
                rentals.AddRange(stock.Rentals);
            }

            foreach (var rental in rentals)
            {
                var _historyItem = new RentalHistoryViewModel
                {
                    ID = rental.ID,
                    StockId = rental.StockId,
                    RentalDate = rental.RentalDate,
                    ReturnedDate = rental.ReturnedDate.HasValue ? rental.ReturnedDate : null,
                    Status = rental.Status,
                    Customer = _customersRepository.GetCustomerFullName(rental.CustomerId)
                };

                _rentalHistory.Add(_historyItem);
            }

            _rentalHistory.Sort((r1, r2) => r2.RentalDate.CompareTo(r1.RentalDate));

            return _rentalHistory;
        }

        private List<RentalHistoryPerDate> GetMovieRentalHistoryPerDates(int movieId)
        {
            var listHistory = new List<RentalHistoryPerDate>();
            var _rentalHistory = GetMovieRentalHistory(movieId);
            if (_rentalHistory.Count > 0)
            {
                var _distinctDates = new List<DateTime>();
                _distinctDates = _rentalHistory.Select(h => h.RentalDate.Date).Distinct().ToList();

                foreach (var distinctDate in _distinctDates)
                {
                    var totalDateRentals = _rentalHistory.Count(r => r.RentalDate.Date == distinctDate);
                    var _movieRentalHistoryPerDate = new RentalHistoryPerDate
                    {
                        Date = distinctDate,
                        TotalRentals = totalDateRentals
                    };

                    listHistory.Add(_movieRentalHistoryPerDate);
                }

                listHistory.Sort((r1, r2) => r1.Date.CompareTo(r2.Date));
            }

            return listHistory;
        }

        #endregion
    }
}
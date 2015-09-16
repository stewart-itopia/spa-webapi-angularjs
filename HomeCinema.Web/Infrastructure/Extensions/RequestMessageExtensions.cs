using System.Net.Http;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Services;

namespace HomeCinema.Web.Infrastructure.Extensions
{
    public static class RequestMessageExtensions
    {
        internal static IMembershipService GetMembershipService(this HttpRequestMessage request)
        {
            return request.GetService<IMembershipService>();
        }

        internal static IEntityBaseRepository<T> GetDataRepository<T>(this HttpRequestMessage request)
            where T : class, IEntityBase, new()
        {
            return request.GetService<IEntityBaseRepository<T>>();
        }

        private static TService GetService<TService>(this HttpRequestMessage request)
        {
            var dependencyScope = request.GetDependencyScope();
            var service = (TService) dependencyScope.GetService(typeof (TService));

            return service;
        }
    }
}
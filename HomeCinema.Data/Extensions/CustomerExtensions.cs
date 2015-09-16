using System.Linq;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;

namespace HomeCinema.Data.Extensions
{
    public static class CustomerExtensions
    {
        public static bool UserExists(this IEntityBaseRepository<Customer> customersRepository, string email,
            string identityCard)
        {
            var _userExists = false;

            _userExists = customersRepository.GetAll()
                .Any(c => c.Email.ToLower() == email ||
                          c.IdentityCard.ToLower() == identityCard);

            return _userExists;
        }

        public static string GetCustomerFullName(this IEntityBaseRepository<Customer> customersRepository,
            int customerId)
        {
            var _customerName = string.Empty;

            var _customer = customersRepository.GetSingle(customerId);

            _customerName = _customer.FirstName + " " + _customer.LastName;

            return _customerName;
        }
    }
}
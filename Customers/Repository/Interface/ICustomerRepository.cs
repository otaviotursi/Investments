using Customers.Command;
using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Repository.Interface
{
    internal interface ICustomerRepository
    {
        Task DeleteAsync(ulong id, CancellationToken cancellationToken);
        Task<List<CustomerDB>> GetAll(CancellationToken cancellationToken);
        Task<CustomerDB> GetBy(GetByCustomerCommand command, CancellationToken cancellationToken);
        Task InsertAsync(CustomerDB customer, CancellationToken cancellationToken);
        Task UpdateAsync(CustomerDB customer, CancellationToken cancellationToken);
    }
}

using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Repository.Interface
{
    public interface IPortfolioRepository
    {
        Task<List<PortfolioDB>> GetAll(CancellationToken cancellationToken);
        Task<PortfolioDB> GetByName(ulong customerId, CancellationToken cancellationToken);
        Task InsertAsync(PortfolioDB product, CancellationToken cancellationToken);
        Task RemoveAsync(PortfolioDB product, CancellationToken cancellationToken);
    }
}

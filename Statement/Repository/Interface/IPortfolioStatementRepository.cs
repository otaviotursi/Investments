using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statement.Repository.Interface
{
    internal interface IPortfolioStatementRepository
    {
        Task<List<PortfolioStatementDB>> GetByCustomerId(ulong customerId, CancellationToken cancellationToken);
        Task InsertAsync(PortfolioStatementDB? sellProduct, CancellationToken stoppingToken);
    }
}

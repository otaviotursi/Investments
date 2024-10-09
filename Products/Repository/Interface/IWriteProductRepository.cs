using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Repository.Interface
{
    public interface IWriteProductRepository
    {
        Task InsertAsync(ProductDB ProductDB, CancellationToken cancellationToken);
        Task UpdateAsync(ProductDB ProductDB, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<List<ProductDB>> GetStatementBy(string? name, string? user, DateTime? expirationDate,CancellationToken cancellationToken);
    }
}

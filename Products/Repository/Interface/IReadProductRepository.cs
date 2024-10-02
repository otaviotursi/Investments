﻿using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Repository.Interface
{
    public interface IReadProductRepository
    {
        Task<List<ProductDB>> GetAll(CancellationToken cancellationToken);
        Task InsertAsync(ProductDB ProductDB, CancellationToken cancellationToken);
        Task UpdateAsync(ProductDB ProductDB, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}

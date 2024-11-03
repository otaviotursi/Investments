using Users.Command;
using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Repository.Interface
{
    internal interface IUserRepository
    {
        Task DeleteAsync(ulong id, CancellationToken cancellationToken);
        Task<List<UserDB>> GetAll(CancellationToken cancellationToken);
        Task<UserDB> GetBy(string? user, string? fullName, ulong? id, CancellationToken cancellationToken);
        Task InsertAsync(UserDB User, CancellationToken cancellationToken);
        Task UpdateAsync(UserDB User, CancellationToken cancellationToken);
    }
}

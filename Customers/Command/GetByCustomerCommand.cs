using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Command
{
    public class GetByCustomerCommand : MediatR.IRequest<List<CustomerDB>>
    {
        public GetByCustomerCommand()
        {
        }
        public GetByCustomerCommand(ulong? id, string? fullName, string? user)
        {
            Id = id;
            FullName = fullName;
            User = user;
        }
        public ulong? Id { get; set; }
        public string? FullName { get; set; }
        public string? User { get; set; }

    }
}

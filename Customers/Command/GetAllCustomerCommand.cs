using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Command
{
    public class GetAllCustomerCommand : MediatR.IRequest<List<CustomerDB>>
    {
    }
}

using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command
{
    public class GetAllProductCommand : MediatR.IRequest<List<ProductDB>>
    {
        public GetAllProductCommand()
        {
        }


    }
}

using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command
{
    public class GetProductByNameCommand : MediatR.IRequest<ProductDB>
    {
        public GetProductByNameCommand()
        {
        }
        public GetProductByNameCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }


    }
}

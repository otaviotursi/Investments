using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command
{
    public class GetProductByNameQuery : MediatR.IRequest<ProductDB>
    {
        public GetProductByNameQuery()
        {
        }
        public GetProductByNameQuery(string name)
        {
            Name = name;
        }

        public string Name { get; set; }


    }
}

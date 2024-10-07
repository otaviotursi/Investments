using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command
{
    public class GetStatementByNameProductCommand : MediatR.IRequest<List<ProductDB>>
    {
        public GetStatementByNameProductCommand()
        {
        }
        public GetStatementByNameProductCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

    }
}

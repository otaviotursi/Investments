using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command
{
    public class GetStatementByProductCommand : MediatR.IRequest<List<ProductDB>>
    {
        public GetStatementByProductCommand()
        {
        }
        public GetStatementByProductCommand(string? name, string? user, DateTime? expirationDate)
        {
            Name = name;
            User = user;
            ExpirationDate = expirationDate;
        }

        public string? Name { get; set; }
        public string? User { get; set; }
        public DateTime? ExpirationDate { get; set; }

    }
}

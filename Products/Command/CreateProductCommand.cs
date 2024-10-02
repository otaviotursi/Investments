using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command
{
    public class CreateProductCommand : MediatR.IRequest<string>
    {
        public CreateProductCommand()
        {
        }

        public CreateProductCommand(Guid id, string name, string productType, ulong unitPrice)
        {
            Id = id;
            Name = name;
            ProductType = productType;
            UnitPrice = unitPrice;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string ProductType { get; set; }
        public ulong UnitPrice { get; set; }
        public ulong AvailableQuantity { get; set; }
    }
}

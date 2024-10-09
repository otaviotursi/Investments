using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command
{
    public class UpdateProductCommand : MediatR.IRequest<string>
    {
        public UpdateProductCommand()
        {
        }

        public UpdateProductCommand(Guid id, string name, string productType, ulong unitPrice, ulong availableQuantity, DateTime expirationDate, string user)
        {
            Id = id;
            Name = name;
            ProductType = productType;
            UnitPrice = unitPrice;
            AvailableQuantity = availableQuantity;
            ExpirationDate = expirationDate;
            User = user;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }
        public ulong UnitPrice { get; set; }
        public ulong AvailableQuantity { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string User { get; set; }

    }
}

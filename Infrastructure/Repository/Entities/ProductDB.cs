using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Entities
{
    public class ProductDB
    {
        public ProductDB(Guid id, string name, decimal unitPrice, decimal availableQuantity, string productType, DateTime expirationDate, string user)
        {
            Id = id;
            Name = name;
            UnitPrice = unitPrice;
            AvailableQuantity = availableQuantity;
            ProductType = productType;
            ExpirationDate = expirationDate;
            User = user;
        }

        public ProductDB() { }

        public ProductDB(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ProductType { get; set; }
        public decimal AvailableQuantity { get; set; }
        public string User { get; set; }
    }
}

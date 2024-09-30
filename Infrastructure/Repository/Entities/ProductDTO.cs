﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Entities
{
    public class ProductDTO
    {
        public ProductDTO(Guid id, string name, decimal unitPrice, decimal availableQuantity, string productType)
        {
            Id = id;
            Name = name;
            UnitPrice = unitPrice;
            AvailableQuantity = availableQuantity;
            ProductType = productType;
        }

        public ProductDTO() { }

        public ProductDTO(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductType { get; set; }
        public decimal AvailableQuantity { get; set; }
    }
}

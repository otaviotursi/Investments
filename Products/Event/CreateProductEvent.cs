using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Event
{
    public class CreateProductEvent : ProductDB, INotification
    {

        public CreateProductEvent()
        {

        }
        public CreateProductEvent(ProductDB product)
        {
            Id = product.Id;
            Name = product.Name;
            UnitPrice = product.UnitPrice;
            AvailableQuantity = product.AvailableQuantity;
            ProductType = product.ProductType;
            ExpirationDate = product.ExpirationDate;
        }
    }
}

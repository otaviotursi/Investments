using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Event
{
    public class UpdateProductEvent : ProductDB, INotification
    {

        public UpdateProductEvent()
        {

        }
        public UpdateProductEvent(ProductDB product)
        {
            Id = product.Id;
            Name = product.Name;
            UnitPrice = product.UnitPrice;
            AvailableQuantity = product.AvailableQuantity;
            ProductType = product.ProductType;
            ExpirationDate = product.ExpirationDate;
        }
        public UpdateProductEvent(Guid productId, decimal availableQuantity, string operationType, ulong userId)
        {
            Id = productId;
            AvailableQuantity = availableQuantity;
            OperationType = operationType;
            UserId = userId;
        }
        public string OperationType { get; set; }
    }
}

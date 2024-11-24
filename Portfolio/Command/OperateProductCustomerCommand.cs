using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Command
{
    public class OperateProductCustomerCommand : MediatR.IRequest<string>
    {
        public OperateProductCustomerCommand()
        {
        }

        public OperateProductCustomerCommand(Guid productId, ulong customerId, string productName, ulong amountNegotiated, string operationType)
        {
            ProductId = productId;
            CustomerId = customerId;
            ProductName = productName;
            AmountNegotiated = amountNegotiated;
            OperationType = operationType;
        }

        public Guid ProductId { get; set; }
        public ulong CustomerId { get; set; }
        public string ProductName { get; set; }
        public decimal AmountNegotiated { get; set; }
        public string OperationType { get; set; }
    }
}

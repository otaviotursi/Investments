using Customers.Repository.Interface;
using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Command.Handler
{
    internal class GetByCustomerCommandHandler : IRequestHandler<GetByCustomerCommand, CustomerDB>
    {
        private readonly IMediator _mediator;
        private readonly ICustomerRepository _repository;

        public GetByCustomerCommandHandler(IMediator mediator, ICustomerRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<CustomerDB> Handle(GetByCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetBy(command, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}

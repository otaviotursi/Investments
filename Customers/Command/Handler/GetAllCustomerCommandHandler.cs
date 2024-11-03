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
    internal class GetAllCustomerCommandHandler : IRequestHandler<GetAllCustomerCommand, List<CustomerDB>>
    {
        private readonly IMediator _mediator;
        private readonly ICustomerRepository _repository;

        public GetAllCustomerCommandHandler(IMediator mediator, ICustomerRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<List<CustomerDB>> Handle(GetAllCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetAll(cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
}

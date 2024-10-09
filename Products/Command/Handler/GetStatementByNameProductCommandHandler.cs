using Infrastructure.Repository.Entities;
using MediatR;
using Products.Event;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Command.Handler
{
    public class GetStatementByProductCommandHandler : IRequestHandler<GetStatementByProductCommand, List<ProductDB>>
    {
        private readonly IMediator _mediator;
        private readonly IWriteProductRepository _repository;

        public GetStatementByProductCommandHandler(IMediator mediator, IWriteProductRepository repositoryWrite)
        {
            _mediator = mediator;
            _repository = repositoryWrite;
        }

        public async Task<List<ProductDB>> Handle(GetStatementByProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetStatementBy(command.Name, command.User, command.ExpirationDate, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}

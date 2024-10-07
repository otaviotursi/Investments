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
    public class GetStatementByNameProductCommandHandler : IRequestHandler<GetStatementByNameProductCommand, List<ProductDB>>
    {
        private readonly IMediator _mediator;
        private readonly IWriteProductRepository _repository;

        public GetStatementByNameProductCommandHandler(IMediator mediator, IWriteProductRepository repositoryWrite)
        {
            _mediator = mediator;
            _repository = repositoryWrite;
        }

        public async Task<List<ProductDB>> Handle(GetStatementByNameProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetStatementByName(command.Name, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}

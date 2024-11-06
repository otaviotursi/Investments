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
    public class GetProductByNameQueryHandler : IRequestHandler<GetProductByNameQuery, ProductDB>
    {
        private readonly IMediator _mediator;
        private readonly IReadProductRepository _repository;

        public GetProductByNameQueryHandler(IMediator mediator, IReadProductRepository repositoryWrite)
        {
            _mediator = mediator;
            _repository = repositoryWrite;
        }

        public async Task<ProductDB> Handle(GetProductByNameQuery command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetByName(command.Name, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}

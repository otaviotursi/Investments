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
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly IWriteProductRepository _repositoryWrite;

        public CreateProductCommandHandler(IMediator mediator, IWriteProductRepository repositoryWrite)
        {
            _mediator = mediator;
            _repositoryWrite = repositoryWrite;
        }

        public async Task<string> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var product = new ProductDB(command.Id, command.Name, command.UnitPrice, command.AvailableQuantity, command.ProductType, command.ExpirationDate, command.User);//TODO: alterar para automapper
                await _repositoryWrite.InsertAsync(product, cancellationToken);

                await _mediator.Publish(new CreateProductEvent(product));


                return await Task.FromResult("Produto criado com sucesso");
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}

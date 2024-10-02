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
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly IWriteProductRepository _repositoryWrite;

        public UpdateProductCommandHandler(IMediator mediator, IWriteProductRepository repositoryWrite)
        {
            _mediator = mediator;
            _repositoryWrite = repositoryWrite;
        }

        public async Task<string> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var product = new ProductDB(command.Id, command.Name, command.UnitPrice, command.AvailableQuantity, command.ProductType);
                await _repositoryWrite.UpdateAsync(product, cancellationToken);
                await _mediator.Publish(new UpdateProductEvent(product));

                return await Task.FromResult("Produto alterado com sucesso");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}

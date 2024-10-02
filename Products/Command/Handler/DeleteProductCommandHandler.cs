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
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly IWriteProductRepository _repositoryWrite;

        public DeleteProductCommandHandler(IMediator mediator, IWriteProductRepository repositoryWrite)
        {
            _mediator = mediator;
            _repositoryWrite = repositoryWrite;
        }

        public async Task<string> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _repositoryWrite.DeleteAsync(command.Id, cancellationToken);
                await _mediator.Publish(new DeleteProductEvent(command.Id));

                return await Task.FromResult("Produto excluido com sucesso");
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}

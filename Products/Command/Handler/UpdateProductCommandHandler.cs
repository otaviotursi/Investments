using AutoMapper;
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
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(IMediator mediator, IWriteProductRepository repositoryWrite, IMapper mapper)
        {
            _mediator = mediator;
            _repositoryWrite = repositoryWrite;
            _mapper = mapper;
        }

        public async Task<string> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var product = _mapper.Map<ProductDB>(command);
                await _repositoryWrite.UpdateAsync(product, cancellationToken);

                var productEvent = _mapper.Map<UpdateProductEvent>(command);
                await _mediator.Publish(productEvent);

                return await Task.FromResult("Produto alterado com sucesso");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}

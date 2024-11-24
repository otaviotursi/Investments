using AutoMapper;
using Infrastructure.Repository.Entities;
using MediatR;
using Portfolio.Repository.Interface;
using Products.Command;
using Products.Command.Handler;
using Products.Event;
using Statement.Event;

namespace Portfolio.Command.Handler
{
    internal class OperateProductCustomerCommandHandler : IRequestHandler<OperateProductCustomerCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly IPortfolioRepository _repository;
        private readonly IMapper _mapper;
        public OperateProductCustomerCommandHandler(IMediator mediator, IPortfolioRepository repository, IMapper mapper)
        {
            _mediator = mediator;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<string> Handle(OperateProductCustomerCommand command, CancellationToken cancellationToken)
        {
            try
            {

                var productByQuery = await _mediator.Send(new GetProductByQuery(command.ProductId), cancellationToken);
                var hasQuantity = productByQuery.AvailableQuantity >= command.AmountNegotiated;

                if (!hasQuantity && string.Equals(command.OperationType,"BUY", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Quantidade disponivel para compra insuficiente");
                }


                var product = _mapper.Map<PortfolioRequest>(command);
                if (string.Equals(command.OperationType, "buy", StringComparison.OrdinalIgnoreCase)) {
                    product.ValueNegotiated = command.AmountNegotiated * productByQuery.UnitPrice;
                    await _repository.InsertAsync(product, cancellationToken);
                    var availableQuantity = productByQuery.AvailableQuantity - command.AmountNegotiated;
                    await _mediator.Publish(new UpdateProductEvent(command.ProductId, availableQuantity, command.OperationType, 0), cancellationToken);

                }
                else if (string.Equals(command.OperationType, "sell", StringComparison.OrdinalIgnoreCase))
                {
                    var availableQuantity = productByQuery.AvailableQuantity + command.AmountNegotiated;
                    await _repository.RemoveAsync(product, cancellationToken);
                    await _mediator.Publish(new UpdateProductEvent(command.ProductId, availableQuantity, command.OperationType, 0), cancellationToken);
                }

                //chama evento de statement portfolio
                var portfolioEvent= _mapper.Map<InsertPortfolioStatementByCustomerEvent>(command);
                await _mediator.Publish(portfolioEvent);

                return await Task.FromResult("trade realizado com sucesso");
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}

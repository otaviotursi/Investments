using AutoMapper;
using Infrastructure.Repository.Entities;
using MediatR;
using Portfolio.Repository.Interface;
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

                var product = _mapper.Map<PortfolioDB>(command);
                if (string.Equals(command.OperationType, "buy", StringComparison.OrdinalIgnoreCase)) { 
                    await _repository.InsertAsync(product, cancellationToken);
                }
                if (string.Equals(command.OperationType, "sell", StringComparison.OrdinalIgnoreCase))
                {
                    await _repository.RemoveAsync(product, cancellationToken);
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

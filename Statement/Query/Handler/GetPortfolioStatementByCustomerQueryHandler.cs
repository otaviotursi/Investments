﻿using Infrastructure.Repository.Entities;
using MediatR;
using Statement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statement.Command.Handler
{
    internal class GetPortfolioStatementByCustomerQueryHandler : IRequestHandler<GetPortfolioStatementByCustomerQuery, List<PortfolioStatementDB>>
    {
        private readonly IMediator _mediator;
        private readonly IPortfolioStatementRepository _repository;

        public GetPortfolioStatementByCustomerQueryHandler(IMediator mediator, IPortfolioStatementRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<List<PortfolioStatementDB>> Handle(GetPortfolioStatementByCustomerQuery command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetByCustomerId(command.CustomerId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}

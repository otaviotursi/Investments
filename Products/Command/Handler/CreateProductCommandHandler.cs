﻿using AutoMapper;
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
        private readonly IMapper _mapper;
        public CreateProductCommandHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<string> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var product = _mapper.Map<ProductDB>(command);

                var productEvent = _mapper.Map<CreateProductEvent>(command);
                await _mediator.Publish(productEvent);


                return await Task.FromResult("Produto criado com sucesso");
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}

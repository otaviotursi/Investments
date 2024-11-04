﻿using AutoMapper;
using Users.Repository.Interface;
using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Command.Handler
{
    internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _repositoryWrite;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IMediator mediator, IUserRepository repositoryWrite, IMapper mapper)
        {
            _mediator = mediator;
            _repositoryWrite = repositoryWrite;
            _mapper = mapper;
        }

        public async Task<string> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var User = _mapper.Map<UserDB>(command);
                await _repositoryWrite.UpdateAsync(User, cancellationToken);

                return await Task.FromResult("Cliente {id} alterado com sucesso");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
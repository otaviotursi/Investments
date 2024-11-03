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
    internal class GetByUserCommandHandler : IRequestHandler<GetByUserCommand, UserDB>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _repository;

        public GetByUserCommandHandler(IMediator mediator, IUserRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<UserDB> Handle(GetByUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetBy(command.User, command.FullName, command.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}

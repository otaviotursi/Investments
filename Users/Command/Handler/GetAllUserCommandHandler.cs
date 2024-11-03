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
    internal class GetAllUserCommandHandler : IRequestHandler<GetAllUserCommand, List<UserDB>>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _repository;

        public GetAllUserCommandHandler(IMediator mediator, IUserRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<List<UserDB>> Handle(GetAllUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetAll(cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}

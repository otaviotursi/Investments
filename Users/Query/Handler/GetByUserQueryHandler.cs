using Users.Repository.Interface;
using Infrastructure.Repository.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Command;

namespace Users.Query.Handler
{
    internal class GetByUserQueryHandler : IRequestHandler<GetByUserQuery, UserDB>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _repository;

        public GetByUserQueryHandler(IMediator mediator, IUserRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<UserDB> Handle(GetByUserQuery command, CancellationToken cancellationToken)
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

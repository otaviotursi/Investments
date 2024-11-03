﻿using Infrastructure.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Command
{
    public class GetByUserCommand : MediatR.IRequest<UserDB>
    {
        public GetByUserCommand()
        {
        }
        public GetByUserCommand(ulong? id, string? fullName, string? user)
        {
            Id = id;
            FullName = fullName;
            User = user;
        }
        public ulong? Id { get; set; }
        public string? FullName { get; set; }
        public string? User { get; set; }

    }
}

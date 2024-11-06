using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Repository.Entities;
using MongoDB.Bson;
using Products.Command;
using Products.Event;
using StackExchange.Redis;

namespace Investments.Mapp
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            //products
            CreateMap<ProductDB, CreateProductCommand>();
            CreateMap<CreateProductCommand, ProductDB>();
            CreateMap<ProductDB, CreateProductEvent>();
            CreateMap<CreateProductEvent, ProductDB>();
            CreateMap<CreateProductCommand, CreateProductEvent>();
            CreateMap<CreateProductEvent, CreateProductCommand>();


            CreateMap<ProductDB, UpdateProductCommand>();
            CreateMap<UpdateProductCommand, ProductDB>();
            CreateMap<ProductDB, UpdateProductEvent>();
            CreateMap<UpdateProductEvent, ProductDB>();
            CreateMap<UpdateProductCommand, UpdateProductEvent>();
            CreateMap<UpdateProductEvent, UpdateProductCommand>();


            CreateMap<UserDB, Users.Command.UpdateUserCommand>();
            CreateMap<Users.Command.UpdateUserCommand, UserDB > ();
            CreateMap<UserDB, Users.Command.CreateUserCommand>();
            CreateMap<Users.Command.CreateUserCommand, UserDB>();


            CreateMap<CustomerDB, Customers.Command.UpdateCustomerCommand>();
            CreateMap<Customers.Command.UpdateCustomerCommand, CustomerDB>();
            CreateMap<CustomerDB, Customers.Command.CreateCustomerCommand>();
            CreateMap<Customers.Command.CreateCustomerCommand, CustomerDB>();

        }
    }
}

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

namespace Products.Mapp
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
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
        }
    }
}

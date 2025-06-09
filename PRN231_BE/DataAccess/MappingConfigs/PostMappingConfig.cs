using DataAccess.Entities;
using DataAccess.Models.Posts;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MappingConfigs
{
    public class PostMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PostCreateDTO, Post>();
            config.NewConfig<PostUpdateDTO, Post>();
            config.NewConfig<Post, PostDTO>();
        }
    }
}

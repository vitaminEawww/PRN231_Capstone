using DataAccess.Entities;
using DataAccess.Models.Comments;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MappingConfigs
{
    public class CommentMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CommentCreateDTO, Comment>();
            config.NewConfig<CommentUpdateDTO, Comment>();
            config.NewConfig<Comment, CommentDTO>();
        }
    }
}

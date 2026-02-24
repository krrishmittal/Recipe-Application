using AutoMapper;
using Server.DTOs.Request;
using Server.DTOs.Response;
using Server.Models;

namespace Server.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Recipe, RecipeResponse>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : string.Empty));

            CreateMap<CreateRecipeRequest, Recipe>();
            CreateMap<UpdateRecipeRequest, Recipe>();

            // Auth mappings
            CreateMap<User, AuthResponse>();
        }
    }
}

using AutoMapper;

namespace RyderGyde.ShopNotes.RedisUpdateTrigger.MappingProfile
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}

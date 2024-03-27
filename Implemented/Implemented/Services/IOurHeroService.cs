using Implemented.Models;

namespace Implemented.Services
{
    public interface IOurHeroService
    {
        Task<IEnumerable<OurHero>> GetAllHeros(bool? isActive = null);
        Task<OurHero> GetHerosByID(int id);
        Task<OurHero> AddOurHero(OurHero heroObject);
        Task<OurHero> UpdateOurHero(int id, OurHero heroObject);
        Task<bool> DeleteHerosByID(int id);
    }
}

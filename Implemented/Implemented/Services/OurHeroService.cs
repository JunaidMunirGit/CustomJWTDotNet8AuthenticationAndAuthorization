using Implemented.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Implemented.Services
{
    public class OurHeroService
    {
        private readonly AppSettings _appSettings;
        private readonly OurHeroDbContext db;

        public OurHeroService(IOptions<AppSettings> appSettings, OurHeroDbContext _db)
        {
            _appSettings = appSettings.Value;
            db = _db;
        }

        public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model)
        {
            var user = await db.Users.SingleOrDefaultAsync(x => x.Username == model.Username && x.Password == model.Password);

            if (user == null) return null;


            var token = await generateJwtToken(user);


            return new AuthenticateResponse(user, token);
        }


        public async Task<string> generateJwtToken(User user)
        {
            //Generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await Task.Run(() =>
            {
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);
            });
            return tokenHandler.WriteToken(token);
        }

        public async Task<IEnumerable<OurHero>> GetAllHeros(bool? isActive = null)
        {
            return await db.OurHeros.Where(x => x.isActive == true).ToListAsync();
        }

        public async Task<OurHero> GetHerosByID(int id)
        {
            return await db.OurHeros.FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<OurHero> AddOurHero(OurHero heroObject)
        {
            bool isSuccess = false;
            if (heroObject.Id > 0)
            {
                var obj = await db.OurHeros.FirstOrDefaultAsync(c => c.Id == heroObject.Id);
                if (obj != null)
                {
                    // obj.Address = userObj.Address;
                    obj.FirstName = heroObject.FirstName;
                    obj.LastName = heroObject.LastName;
                    await db.OurHeros.AddAsync(obj);
                    isSuccess = await db.SaveChangesAsync() > 0;
                }
            }
            return isSuccess ? heroObject : null;
        }

        public async Task<OurHero> UpdateOurHero(int id, OurHero heroObject)
        {
            bool isSuccess = false;
            if (heroObject.Id > 0)
            {
                var obj = await db.OurHeros.FirstOrDefaultAsync(c => c.Id == heroObject.Id);
                if (obj != null)
                {
                    // obj.Address = userObj.Address;
                    obj.FirstName = heroObject.FirstName;
                    obj.LastName = heroObject.LastName;
                    db.OurHeros.Update(obj);
                    isSuccess = await db.SaveChangesAsync() > 0;
                }
            }
            return isSuccess ? heroObject : null;
        }

        public async Task<bool> DeleteHerosByID(int id)
        {
            var hero = await db.OurHeros.FindAsync(id);
            db.OurHeros.Remove(hero);
            await db.SaveChangesAsync();
            return true; // Replace with actual data access code
        }
    }
}

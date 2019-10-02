using IntegrationWS.Data;
using IntegrationWS.Models;
using IntegrationWS.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Utils
{
    public class AuthRepository : IAuthRepository
    {        
        public async Task<User> Login(string username, string password)
        {
            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.Username == username);

                if (user == null)
                    return null;

                if (!VerifyPasswordHash(password, user.Passwordhash, user.Passwordsalt))
                    return null;

                //Auth Succesful
                return user;
            }           

        }

        private bool VerifyPasswordHash(string password, byte[] passwordhash, byte[] passwordsalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordsalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordhash[i])
                        return false;
                }
            }

            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                byte[] passwordHash, passwordSalt;
                createPassworkHash(password, out passwordHash, out passwordSalt);

                user.Passwordhash = passwordHash;
                user.Passwordsalt = passwordSalt;

                db.Users.Add(user);
                await db.SaveChangesAsync();

                return user;
            }
        }

        private void createPassworkHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExist(string username)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (await db.Users.AnyAsync(x => x.Username == username))
                    return true;

                return false;
            }
        }
    }
}
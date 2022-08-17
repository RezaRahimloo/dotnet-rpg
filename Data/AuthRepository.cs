using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<string>> LoginAsync(string username, string password)
        {
            var response = new ServiceResponse<string>();
            
            var user  = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            
            if(user is null)// user not found
            {
                response.Message = "User doesn't exist.";
                response.Success = false;
            }
            else if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Message = "Wrong user creditentials.";
                response.Success = false;
            }
            else
            {
                response.Data = user.Id.ToString();
            }

            return response;
        }

        public async Task<ServiceResponse<int>> RegisterAsync(User user, string password)
        {
            ServiceResponse<int> response = new();
            if(await UserExistsAsync(user.Username))
            {
                response.Success = false;
                response.Message = "User with this username already exists!";
                return response;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Add(user);
            await _context.SaveChangesAsync();
            
            response.Data = user.Id;
            return response;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            if(await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                // compute the entered password's hash
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                //Check if the password hash byte array is equal to the password hash saved in the DB
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
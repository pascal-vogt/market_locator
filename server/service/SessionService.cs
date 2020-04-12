namespace Service
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Database;
    using Database.Entities;
    using Microsoft.EntityFrameworkCore;

    public class SessionService
    {
        private DatabaseContext DatabaseContext { get; set; }
        
        public SessionService(DatabaseContext databaseContext)
        {
            this.DatabaseContext = databaseContext;
        }
        
        private static string ComputeSha256Hash(string s)
        {
            using var sha256Hash = SHA256.Create();
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(s));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        public User TryLogin(string email, string password)
        {
            var user = this.DatabaseContext.User
                .AsQueryable()
                .Include(o => o.RoleAssignments)
                .ThenInclude(o => o.Role)
                .FirstOrDefault(o => o.Email.ToLower() == email.ToLower());

            if (user == null || ComputeSha256Hash(user.PasswordSalt + password) != user.PasswordHash)
            {
                return null;
            }

            return user;
        }
    }
}
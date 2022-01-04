using Pedigree.Core.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pedigree.Core.Extensions
{
    public static class ExtensionMethods
    {
        public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static User WithoutPassword(this User user)
        {
            user.Password = null;
            user.PasswordHash = null;
            return user;
        }

        public static string GenerateRandomOID(int length = 22)
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var randomString = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return "5c" + randomString;
        }
    }
}

using Store.Sts.Extensions;
using Store.STS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Store.STS.Extensions
{
    public static class UserExtensions
    {
        public const string DefaultPhonePattern = @"^(?:\(?)(?<AreaCode>\d{3})(?:[\).\s]?)(?<Prefix>\d{3})(?:[-\.\s]?)(?<Suffix>\d{4})(?!\d)";

        public static Match CheckPhone(this string phoneNumber, string phonePattern = DefaultPhonePattern)
        {
            return Regex.Match(phoneNumber, DefaultPhonePattern);
        }

        public static int GetAge(this DateTime dateofBirth)
        {
            var today = DateTime.Today;
            var firstValue = (today.Year * 100 + today.Month) * 100 + today.Day;
            var secondValue = (dateofBirth.Year * 100 + dateofBirth.Month) * 100 + dateofBirth.Day;

            return (firstValue - secondValue) / 1000;
        }

        public static ApplicationUser NewVerificationTokenUser(this string phoneNumber)
        {
            var password = Guid.NewGuid().ToString();
            return new ApplicationUser
            {
                UserName = phoneNumber,
                PhoneNumber = phoneNumber,
                LockoutEnabled = true,
                PhoneNumberConfirmed = false,
                EmailConfirmed = false,
                SecurityStamp = (password.GetHash(SecurityExtensions.HashType.SHA512) + phoneNumber
                .GetHash(SecurityExtensions.HashType.SHA256))
                .GetHash(SecurityExtensions.HashType.MD5)
            };
        }
    }
}

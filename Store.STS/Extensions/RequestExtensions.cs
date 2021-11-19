using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.STS.Extensions
{
    public static class RequestExtensions
    {
        public static bool IsVerificationTokenGrantType(this OpenIddictRequest request)
        {
            return request.GrantType == "verification_token";
        }
    }
}

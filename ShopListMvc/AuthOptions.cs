using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopListMvc
{
    public class AuthOptions
    {
        public const string ISSUER = "ShopListBackend";
        public const string AUDIENCE = "ShopListReactNative";
        const string KEY = "awwwwwwwwwwesome_shoplist_jwt_secret_key";
        public const int LIFETIME = 1440;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}

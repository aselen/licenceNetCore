using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace backend
{
    public class TokenHandler
    {
        //Token üretecek metot.
        public Token CreateAccessToken(int id)
        {

            Token tokenInstance = new Token();

            tokenInstance.Expiration = DateTime.Now.AddMinutes(1);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("alper_selen_test_projesi_kapsaminda_kullanilacak");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, id.ToString())
                }),
                Expires = tokenInstance.Expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            tokenInstance.AccessToken = tokenHandler.WriteToken(token);

            tokenInstance.RefreshToken = CreateRefreshToken();
            return tokenInstance;
        }

        //Refresh Token üretecek metot.
        public string CreateRefreshToken()
        {
            return "alper";
        }
    }
}
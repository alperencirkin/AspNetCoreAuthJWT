
using AspNetCoreAuthJWT.Helpers;
using AspNetCoreAuthJWT.Entities;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AspNetCoreAuthJWT.Services.User;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreAuthJWT.Services.User
{
    public class UserService : IUserService
    {
        private readonly Appsettings _appSettings;
        public UserService(IOptions<Appsettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public (string username, string token)? Authenticate(string username, string password)
        {
            //Kullanıcının gerçekten olup olmadığı kontrol ediyorum yoksa direk null dönüyorum.
            //var user = _dbContext.ApplicationUsers.SingleOrDefault(x => x.Username == username && x.Password == password);
            var user = new Users();
            user.Username = "alperen";
            user.Password = "cirkin";
            if (user.Username != username & user.Password != password)
                return null;

            // Token oluşturmak için önce JwtSecurityTokenHandler sınıfından instance alıyorum.
            var tokenHandler = new JwtSecurityTokenHandler();
            //İmza için gerekli gizli anahtarımı alıyorum.
            var key = Encoding.ASCII.GetBytes(_appSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Özel olarak şu Claimler olsun dersek buraya ekleyebiliriz.
                Subject = new ClaimsIdentity(new[]
                {
                    //İstersek string bir property istersek ClaimsTypes sınıfının sabitlerinden çağırabiliriz.
                    new Claim("userId", user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.Username)
                }),
                //Tokenın hangi tarihe kadar geçerli olacağını ayarlıyoruz.
                Expires = DateTime.UtcNow.AddMinutes(20),
                //Son olarak imza için gerekli algoritma ve gizli anahtar bilgisini belirliyoruz.
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //Token oluşturuyoruz.
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //Oluşturduğumuz tokenı string olarak bir değişkene atıyoruz.
            string generatedToken = tokenHandler.WriteToken(token);

            //Sonuçlarımızı tuple olarak dönüyoruz.
            return (user.Username, generatedToken);
        }
    }
}

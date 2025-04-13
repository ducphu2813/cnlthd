using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIApplication.Model;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace APIApplication.JWT;

public class TokenProvider(IConfiguration configuration)
{
    public string Create(Users user)
    {
        //lấy secret key từ appsettings.json
        string secretKey = configuration["Jwt:Secret"];
        
        //tạo security key từ secret key vì security key phải là kiểu byte
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        
        //tạo signing credentials từ security key và thuật toán mã hóa
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        //tạo các claims cho jwt
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            
            //thêm 1 claim tùy chỉnh
            //thêm claims thời gian tạo token
            new Claim("TokenCreatedTime", DateTime.UtcNow.ToString()),
            // new Claim("email_verified", "true"),
        };
        
        //thêm danh sách roles vào claims
        // if (user.Roles != null)
        // {
        //     claims.AddRange(user.Roles.Select(role => new Claim("roles", role)));
        // }
        
        //phần này là tạo token descriptor, nó chứa các thông tin cần thiết để tạo ra jwt
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        //tạo 1 instance của JsonWebTokenHandler để tạo ra jwt
        //ở đây cũng có thể sử dụng JwtSecurityTokenHandler
        var handler = new JsonWebTokenHandler();
        var tokenHandler = new JwtSecurityTokenHandler();
        
        //sự khác nhau giữa 2 class này là ở cách tạo jwt
        //đối với JsonWebTokenHandler thì ta tạo jwt từ token descriptor
        //còn đối với JwtSecurityTokenHandler thì ta tạo jwt từ 1 instance của JwtSecurityToken
        
        //tạo jwt từ token descriptor dùng JsonWebTokenHandler
        string token = handler.CreateToken(tokenDescriptor);
        
        return token;
    }
}
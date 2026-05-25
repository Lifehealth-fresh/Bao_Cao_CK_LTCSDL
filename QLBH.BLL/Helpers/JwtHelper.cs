using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QLBH.BLL.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QLBH.BLL.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(TaiKhoanDto taiKhoan)
        {
            var secretKey = _config["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = _config["JwtSettings:Issuer"] ?? "QLBH";
            var audience = _config["JwtSettings:Audience"] ?? "QLBHClient";
            var expiryMinutes = int.Parse(_config["JwtSettings:ExpiryMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, taiKhoan.TaiKhoanID.ToString()),
                new Claim(ClaimTypes.Email, taiKhoan.Email),
                new Claim(ClaimTypes.Role, taiKhoan.VaiTro)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
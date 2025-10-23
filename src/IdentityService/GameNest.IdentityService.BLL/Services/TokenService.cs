using GameNest.IdentityService.BLL.DTOs;
using GameNest.IdentityService.BLL.Interfaces;
using GameNest.IdentityService.DAL.Interfaces;
using GameNest.IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace GameNest.IdentityService.BLL.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<AuthResponseDto> GenerateTokensAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            var accessToken = await GenerateAccessTokenAsync(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id, cancellationToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<AuthResponseDto> RefreshTokensAsync(TokenRequestDto tokenRequestDto, CancellationToken cancellationToken = default)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(tokenRequestDto.RefreshToken, cancellationToken);

            if (storedToken == null)
                throw new AuthenticationException("Refresh token does not exist.");

            if (DateTime.UtcNow > storedToken.Expires)
                throw new AuthenticationException("Refresh token has expired.");

            if (storedToken.IsRevoked)
                throw new AuthenticationException("Refresh token has been revoked.");

            if (storedToken.IsUsed)
                throw new AuthenticationException("Refresh token has already been used.");

            storedToken.IsUsed = true;
            await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);

            var user = storedToken.User;
            return await GenerateTokensAsync(user, cancellationToken);
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

            if (storedToken == null || storedToken.IsRevoked)
                return false;

            storedToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);
            _logger.LogInformation("Revoked token {TokenId} for user {UserId}", storedToken.Id, storedToken.UserId);
            return true;
        }


        private async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(ClaimTypes.Name, user.UserName!)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            //claims.AddRange(roles.Select(role => new Claim("role", role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:AccessTokenDurationMinutes"]!)),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var token = Convert.ToBase64String(randomNumber);

            var refreshToken = new RefreshToken
            {
                Token = token,
                Expires = DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtSettings:RefreshTokenDurationDays"]!)),
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            return refreshToken;
        }
    }
}

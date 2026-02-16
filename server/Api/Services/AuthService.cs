using Api.DTOs.Request;
using Api.DTOs.Response;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class AuthService(PasswordService passwordService, TokenService tokenService, MusicDbContext context)
{
    public async Task<UserLoginResDto> Login(UserLoginReqDto userLoginReqDto)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.username == userLoginReqDto.username);
        
        if (user == null || !passwordService.VerifyHashedPassword(user.password, userLoginReqDto.password))
            throw new Exception("Invalid login credentials");
        
        var token = tokenService.GenerateToken(user);
        var refresh = tokenService.GenerateRefreshToken();

        user.refreshToken = passwordService.HashRefreshToken(refresh);
        user.refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await context.SaveChangesAsync();

        return new UserLoginResDto
        {
            id = user.id,
            username = user.username,
            isAdmin = user.isAdmin,
            token = token,
            refreshToken = refresh
        };
    }

    public async Task<(string token, string refresh)> RefreshToken(string refreshToken, double refreshTokenDays)
    {
        try
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.refreshToken == passwordService.HashRefreshToken(refreshToken));

            if (user == null)
            {
                throw new Exception("Invalid refresh token");
            }

            if (user.refreshTokenExpiry < DateTime.UtcNow)
            {
                throw new Exception("Refresh token expired");
            }
            
            var newRefresh = tokenService.GenerateRefreshToken();
            user.refreshToken = passwordService.HashRefreshToken(newRefresh);
            user.refreshTokenExpiry = DateTime.UtcNow.AddDays((int)refreshTokenDays);
   
            await context.SaveChangesAsync();
            
            var newToken = tokenService.GenerateToken(user);
        
            return (newToken, newRefresh);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task Logout(string refreshToken)
    {
        var user = await context.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.refreshToken == passwordService.HashRefreshToken(refreshToken));
        if (user == null)
            throw new Exception("User not found");
        
        user.refreshToken = null;
        user.refreshTokenExpiry = null;
        await context.SaveChangesAsync();
    }
}
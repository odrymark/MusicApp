using Api.DTOs.Request;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class UserService(PasswordService passwordService, AuthService authService, MusicDbContext context)
{
    public async Task CreateUser(UserCreateReqDto userCreateReqDto)
    {
        var username = userCreateReqDto.username;
        var email = userCreateReqDto.email;
        var password = userCreateReqDto.password;
        var passwordConfirm = userCreateReqDto.passwordConfirm;
        
        if (password != passwordConfirm)
            throw new Exception("Passwords do not match.");
        
        var existingUser = await context.Users
            .FirstOrDefaultAsync(u =>
                u.username == username ||
                u.email == email);

        if (existingUser != null)
        {
            if (existingUser.username == username)
                throw new Exception("Username already exists.");

            if (existingUser.email == email)
                throw new Exception("Email already exists.");
        }

        var hashedPassword = passwordService.HashPassword(userCreateReqDto.password);

        var newUser = new User
        {
            username = username,
            email = email,
            password = hashedPassword,
            isAdmin = false
        };

        context.Users.Add(newUser);
        await context.SaveChangesAsync();
    }
}
using GestionStock.Context;
using GestionStock.Controllers;
using GestionStock.DTOs;
using GestionStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionStock.Tests
{
    public class AuthControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private IConfiguration GetFakeConfiguration()
        {
            var settings = new Dictionary<string, string>
            {
                {"Jwt:Key", "THIS_IS_A_SUPER_SECRET_KEY_123456789"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"},
                {"Jwt:DurationInMinutes", "60"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        // ✅ REGISTER SUCCESS
        [Fact]
        public async Task Register_ReturnsOk_WhenUserIsValid()
        {
            var context = GetInMemoryDbContext();
            var config = GetFakeConfiguration();
            var controller = new AuthController(context, config);

            var dto = new RegisterDto
            {
                Nom = "Test",
                Email = "test@mail.com",
                Password = "123456",
                Role = "Admin",
                Telephone = "12345678",
                Adresse = "Tunis"
            };

            var result = await controller.Register(dto);

            Assert.IsType<OkObjectResult>(result);
            Assert.Single(context.Utilisateurs);
        }

        // ❌ REGISTER EMAIL EXISTS
        [Fact]
        public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
        {
            var context = GetInMemoryDbContext();
            var config = GetFakeConfiguration();

            context.Utilisateurs.Add(new Utilisateur
            {
                Nom = "Existing",
                Email = "exist@mail.com",
                Password = "hashed",
                Role = Role.Admin
            });
            await context.SaveChangesAsync();

            var controller = new AuthController(context, config);

            var dto = new RegisterDto
            {
                Nom = "Test",
                Email = "exist@mail.com",
                Password = "123456",
                Role = "Admin"
            };

            var result = await controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ❌ LOGIN WRONG PASSWORD
        [Fact]
        public void Login_ReturnsUnauthorized_WhenPasswordIsWrong()
        {
            var context = GetInMemoryDbContext();
            var config = GetFakeConfiguration();

            var user = new Utilisateur
            {
                Nom = "User",
                Email = "user@mail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                Role = Role.Admin
            };

            context.Utilisateurs.Add(user);
            context.SaveChanges();

            var controller = new AuthController(context, config);

            var dto = new LoginDto
            {
                Email = "user@mail.com",
                Password = "wrongpassword"
            };

            var result = controller.Login(dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // ✅ LOGIN SUCCESS
        [Fact]
        public void Login_ReturnsToken_WhenCredentialsAreValid()
        {
            var context = GetInMemoryDbContext();
            var config = GetFakeConfiguration();

            var user = new Utilisateur
            {
                Nom = "User",
                Email = "user@mail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = Role.Admin
            };

            context.Utilisateurs.Add(user);
            context.SaveChanges();

            var controller = new AuthController(context, config);

            var dto = new LoginDto
            {
                Email = "user@mail.com",
                Password = "123456"
            };

            var result = controller.Login(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}

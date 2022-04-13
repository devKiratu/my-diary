using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using my_diary.Api.Controllers;
using my_diary.Api.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BC = BCrypt.Net;


namespace my_diary.Api.Tests
{
    public class UsersControllerSetupFixture : IDisposable
    {
        public DbContextOptions<MainDbContext> options = new DbContextOptionsBuilder<MainDbContext>().UseInMemoryDatabase("mockDb").Options;
        public UsersControllerSetupFixture()
        {
            using var context = new MainDbContext(options);
            var hashedPwd = BC.BCrypt.EnhancedHashPassword("1234");
            context.Users.Add(new User { Email = "admin@example.com", Id = "admin1", Password = hashedPwd, UserName = "admin" });
            context.SaveChanges();

        }
        public void Dispose()
        {
        }
    }
    public class UsersControllerTests : IClassFixture<UsersControllerSetupFixture>
    {
        private DbContextOptions<MainDbContext> options;
        private IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        public UsersControllerTests(UsersControllerSetupFixture fixture)
        {
            options = fixture.options;
        }
        [Fact]
        public async Task SignupAsync_UseRegisteredEmail_ReturnsBadRequestAsync() 
        {
            using var context = new MainDbContext(options);
            
            var usersController = new UsersController(context, config);

            var user = new SignupDto { Email = "admin@example.com", Password = "12345", UserName = "admin" };

            var result = await usersController.SignupAsync(user);
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task SignupAsync_NewUser_ReturnsOkObjectResultAsync() 
        {
            using var context = new MainDbContext(options);
            var usersController = new UsersController(context, config);
            var user = new SignupDto { Email = "manager@example.com", Password = "12345", UserName = "manager" };

            var result = await usersController.SignupAsync(user);

            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Fact]
        public async Task LoginAsync_InvalidEmail_ReturnsBadRequestAsync() 
        {
            using var context = new MainDbContext(options);
            var usersController = new UsersController(context, config);
            var user = new User { Email = "admin22@example.com", Password = "54321" };

            var result = await usersController.LoginAsync(user);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsBadRequestAsync() 
        {
            using var context = new MainDbContext(options);
            var usersController = new UsersController(context, config);
            var user = new User { Email = "admin@example.com", Password = "54321" };

            var result = await usersController.LoginAsync(user);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task LoginAsync_ValidCredentials_ProvidesToken() 
        {
            using var context = new MainDbContext(options);
            var usersController = new UsersController(context, config);
            var user = new User { Email = "admin@example.com", Password = "1234"};

            var result = await usersController.LoginAsync(user);

            var responseObj = Assert.IsAssignableFrom<OkObjectResult>(result);
        }
    }
}

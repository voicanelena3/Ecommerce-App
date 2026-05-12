using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using BusinessLogic.Services;
using BusinessLogic.DTOs;
using Repository.Models;
using Repository.Repositories;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace EcommerceAPI.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration
                .Setup(c => c["Jwt:Secret"])
                .Returns("your-secret-key-min-32-characters-long!!!");
            _mockConfiguration
                .Setup(c => c["Jwt:ExpirationMinutes"])
                .Returns("60");

            _authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);
        }

        [Fact]
        public void Register_WithValidInput_CreatesUserAndReturnsToken()
        {
            var registerRequest = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUserRepository
                .Setup(r => r.UserExists("test@example.com"))
                .Returns(false);

            _mockUserRepository
                .Setup(r => r.CreateUser(It.IsAny<User>()))
                .Returns(true);

            var createdUser = new User
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository
                .Setup(r => r.GetUserByEmail("test@example.com"))
                .Returns(createdUser);

            var response = _authService.Register(registerRequest);

            Assert.NotNull(response);
            Assert.NotEmpty(response.Token);
            Assert.Equal("test@example.com", response.User.Email);
            Assert.Equal("John", response.User.FirstName);
            Assert.Equal("Doe", response.User.LastName);
            _mockUserRepository.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void Register_WithExistingUser_ReturnsNull()
        {
            var registerRequest = new RegisterRequest
            {
                Email = "existing@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUserRepository
                .Setup(r => r.UserExists("existing@example.com"))
                .Returns(true);

            var response = _authService.Register(registerRequest);

            Assert.Null(response);
            _mockUserRepository.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsTokenAndUser()
        {
            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository
                .Setup(r => r.GetUserByEmail("test@example.com"))
                .Returns(user);

            var response = _authService.Login(loginRequest);

            Assert.NotNull(response);
            Assert.NotEmpty(response.Token);
            Assert.Equal(1, response.User.Id);
            Assert.Equal("test@example.com", response.User.Email);
        }

        [Fact]
        public void Login_WithInvalidEmail_ReturnsNull()
        {
            var loginRequest = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            _mockUserRepository
                .Setup(r => r.GetUserByEmail("nonexistent@example.com"))
                .Returns((User)null!);

            var response = _authService.Login(loginRequest);

            Assert.Null(response);
        }

        [Fact]
        public void Login_WithInvalidPassword_ReturnsNull()
        {
            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository
                .Setup(r => r.GetUserByEmail("test@example.com"))
                .Returns(user);

            var response = _authService.Login(loginRequest);

            Assert.Null(response);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Register_WithEmptyEmail_ReturnsNull(string email)
        {
            var registerRequest = new RegisterRequest
            {
                Email = email,
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };

            var response = _authService.Register(registerRequest);

            Assert.Null(response);
        }
    }
}

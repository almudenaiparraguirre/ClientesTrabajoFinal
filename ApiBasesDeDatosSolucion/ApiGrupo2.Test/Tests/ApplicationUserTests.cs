using System;
using Xunit;
using FluentAssertions;
using ApiBasesDeDatosProyecto.Models;

namespace ApiBasesDeDatosProyecto.Tests.Models
{
    public class ApplicationUserTests
    {
        [Fact]
        public void ApplicationUser_Should_Have_Default_Values()
        {
            // Arrange & Act
            var user = new ApplicationUser();

            // Assert
            user.FullName.Should().BeNull();
            user.Address.Should().BeNull();
            user.DateOfBirth.Should().BeNull();
            user.Rol.Should().BeNull();
            user.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void ApplicationUser_Should_Allow_Setting_Properties()
        {
            // Arrange
            var user = new ApplicationUser();
            var testName = "John Doe";
            var testAddress = "123 Main St";
            var testDateOfBirth = new DateTime(1990, 1, 1);
            var testRol = "Admin";

            // Act
            user.FullName = testName;
            user.Address = testAddress;
            user.DateOfBirth = testDateOfBirth;
            user.Rol = testRol;
            user.IsDeleted = false;

            // Assert
            user.FullName.Should().Be(testName);
            user.Address.Should().Be(testAddress);
            user.DateOfBirth.Should().Be(testDateOfBirth);
            user.Rol.Should().Be(testRol);
            user.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void ApplicationUser_Should_Extend_IdentityUser()
        {
            // Arrange & Act
            var user = new ApplicationUser();

            // Assert
            user.Should().BeAssignableTo<IdentityUser>();
        }
    }
}

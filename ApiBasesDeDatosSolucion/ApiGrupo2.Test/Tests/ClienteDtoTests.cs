using System;
using System.ComponentModel.DataAnnotations;
using Xunit;
using ApiBasesDeDatosProyecto.Models;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace ApiBasesDeDatosProyecto.Tests
{
    public class ClienteDtoTests
    {
        [Fact]
        public void ClienteDto_ValidModel_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var clienteDto = new ClienteDto
            {
                ClienteId = 1,
                UserId = "user123",
                Nombre = "Juan",
                Apellido = "Perez",
                dateOfBirth = new DateTime(1990, 1, 1),
                Empleo = "Desarrollador",
                NombrePais = "España",
                Email = "juan.perez@example.com",
                PaisId = 1
            };

            // Act
            var validationResults = ValidateModel(clienteDto);

            // Assert
            Assert.Empty(validationResults);
        }

        [Fact]
        public void ClienteDto_MissingRequiredFields_ShouldHaveValidationErrors()
        {
            // Arrange
            var clienteDto = new ClienteDto
            {
                ClienteId = 1,
                // Missing UserId
                Nombre = "Juan",
                Apellido = "Perez",
                dateOfBirth = new DateTime(1990, 1, 1),
                // Missing Email
                PaisId = 1
            };

            // Act
            var validationResults = ValidateModel(clienteDto);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains("UserId"));
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Email"));
        }

        [Fact]
        public void ClienteDto_InvalidStringLength_ShouldHaveValidationErrors()
        {
            // Arrange
            var clienteDto = new ClienteDto
            {
                ClienteId = 1,
                UserId = "user123",
                Nombre = new string('A', 26), // Exceeds max length of 25
                Apellido = "Perez",
                dateOfBirth = new DateTime(1990, 1, 1),
                Empleo = "Desarrollador",
                NombrePais = "España",
                Email = "juan.perez@example.com",
                PaisId = 1
            };

            // Act
            var validationResults = ValidateModel(clienteDto);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Nombre"));
        }

        private static System.Collections.Generic.IEnumerable<ValidationResult> ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new System.Collections.Generic.List<ValidationResult>();

            Validator.TryValidateObject(model, context, results, true);

            return results;
        }
    }
}

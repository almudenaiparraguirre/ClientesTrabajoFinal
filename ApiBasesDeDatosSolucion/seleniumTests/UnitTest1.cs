using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace MiProyecto.Tests
{
    public class SeleniumTests : IDisposable
    {
        private readonly IWebDriver driver;

        public SeleniumTests()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://localhost:4200/login"); // Cambia esto si es necesario
        }

        [Fact]
        public void TestIniciarSesion()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            // Esperar a que el campo de nombre de usuario sea visible y completarlo
            var usernameField = wait.Until(drv => drv.FindElement(By.Name("username")));
            usernameField.SendKeys("aminprobandolo12@gmail.com"); // Reemplaza con un nombre de usuario válido

            // Esperar a que el campo de contraseña sea visible y completarlo
            var passwordField = wait.Until(drv => drv.FindElement(By.Name("password")));
            passwordField.SendKeys("677304550Aa"); // Reemplaza con una contraseña válida

            // Hacer clic en el botón de iniciar sesión usando CSS Selector
            var loginButton = wait.Until(drv => drv.FindElement(By.CssSelector("button[type='submit']"))); // Asegúrate de que el selector es correcto
            loginButton.Click();

            // Validar que la interacción fue exitosa (cambia este valor según tu aplicación)
            var successMessage = wait.Until(drv => drv.FindElement(By.XPath("//*[contains(text(), 'Gestión de usuarios y clientes')]"))); // Cambia según el mensaje esperado
            Assert.NotNull(successMessage);
        }


        public void Dispose()
        {
            // Cerrar el navegador
            driver.Quit();
        }
    }
}

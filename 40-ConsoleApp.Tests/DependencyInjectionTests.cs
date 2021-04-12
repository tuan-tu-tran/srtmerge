using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.ConsoleApp.Tests
{
    [TestClass]
    public class DependencyInjectionTests
    {
        [TestMethod]
        public void ItCanResolveMainProgram()
        {
            //Arrange
            var services = MainProgram.ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            //Act
            Action resolvingTheMainProgram = () =>
                serviceProvider.GetRequiredService<MainProgram>();

            //Assert
            resolvingTheMainProgram.Should().NotThrow();
        }
    }
}

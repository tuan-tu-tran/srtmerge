using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace srtmerge.Merging.Tests
{
    [TestFixture]
    public class DependencyInjectionTests
    {
        [Test]
        public void ItCanResolveSubtitleFileMerger()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            //Act
            serviceCollection.AddSubtitlesMerging();

            //Assert
            var serviceProvider = serviceCollection.BuildServiceProvider();
            Action resolvingSubtitleFileMerger = () =>
                serviceProvider.GetRequiredService<ISubtitleFileMerger>();
            resolvingSubtitleFileMerger.Should().NotThrow();
        }
    }
}

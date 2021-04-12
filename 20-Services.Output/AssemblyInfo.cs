using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ConsoleApp.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] //needed to allow Moq to mock internal classes
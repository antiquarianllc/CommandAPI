
using System;
using Xunit;

namespace CommandAPI.Tests
{
    using CommandAPI.Models;

    public class CommandTests : IDisposable
    {
        private Command _testCommand;

        public CommandTests( )
        {
           _testCommand = new Command
            {
                HowTo = "Do something",
                Platform = "Some platform",
                CommandLine = "Some commandline"
            };

        }

        public void Dispose( )
        {
            _testCommand = null;
        }

        [Fact]
        public void CanChangeHowTo( )
        {

            // Arrange
 
            // Act
            _testCommand.HowTo = "Execute Unit Tests";

            // Assert
            Assert.Equal("Execute Unit Tests", _testCommand.HowTo);
        }

        [Fact]
        public void CanChangePlatform( )
        {

            // Arrange
 
            // Act
            _testCommand.Platform = "xUnit";

            // Assert
            Assert.Equal("xUnit", _testCommand.Platform);
        }

        [Fact]
        public void CanChangeCommandLine( )
        {

            // Arrange
 
            // Act
            _testCommand.CommandLine = "dotnet test";

            // Assert
            Assert.Equal("dotnet test", _testCommand.CommandLine);
        }

    }

}
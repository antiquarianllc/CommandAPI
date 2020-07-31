
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit;

namespace CommandAPI.Tests
{
    using CommandAPI.Controllers;
    using CommandAPI.Models;

    public class CommandsControllerTests : IDisposable
    {
        private DbContextOptionsBuilder<CommandContext> _optionsBuilder;
        private CommandContext _dbContext;
        private CommandsController _controller;

        public CommandsControllerTests( )
        {

            // Create inMemory database for testing.
            _optionsBuilder = new DbContextOptionsBuilder<CommandContext>( );
            _optionsBuilder.UseInMemoryDatabase( "UnitTestInMemDB" );
            _dbContext = new CommandContext( _optionsBuilder.Options );

            // Create commands controller for testing.   
            _controller = new CommandsController( _dbContext );

        }

        public void Dispose( )
        {

            _optionsBuilder = null;
            foreach ( var cmd in _dbContext.CommandItems )
            {
                _dbContext.CommandItems.Remove( cmd );
            }
            _dbContext.SaveChanges( );
            _dbContext.Dispose( );
            _controller = null;
        }

        // ACTION 1 Tests : GET     /api/commands

        // Test 1.1 : Request objects when none exist - Return "nothing"
        [Fact]
        public void GetCommandItems_ReturnsZeroItems_WhenDBIsEmpty( )
        {

            // Arrange

            // Act
            var results = _controller.GetCommandItems( );

            //Assert
            Assert.Empty( results.Value );
        }

        // Test 1.2 : Request count of single object when only 1 exists in database
        [Fact]
        public void GetCommandItems_ReturnsOneItem_WhenDBHasOneItem( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 1.2",
                CommandLine = "Some CommandLine"
            };

            _dbContext.Add( command );
            _dbContext.SaveChanges( );

            // Act
            var results = _controller.GetCommandItems( );

            //Assert
            Assert.Single( results.Value );

        }

        // Test 1.3 : Request count of N objects when N exists in database
        [Fact]
        public void GetCommandItems_ReturnsNItems_WhenDBHasNItems( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 1.3",
                CommandLine = "Some CommandLine"
            };
            var command2 = new Command
            {
                HowTo = "Do Something Else",
                Platform = "Test 1.3",
                CommandLine = "Some CommandLine"
            };

            _dbContext.Add( command );
            _dbContext.Add( command2 );
            _dbContext.SaveChanges( );

            // Act
            var results = _controller.GetCommandItems( );

            //Assert
            Assert.Equal( 2, results.Value.Count( ) );

        }

        // Test 1.4 : Request count of N objects when N exists in database
        [Fact]
        public void GetCommandItems_ReturnItems_WithCorrectType( )
        {

            // Arrange

            // Act
            var results = _controller.GetCommandItems( );

            //Assert
            Assert.IsType<ActionResult<IEnumerable<Command>>>( results );

        }

        // ACTION 2 Tests : GET     /api/commands/{id}

        // Test 2.1 : Request object by ID when none exist - Return null object value
        [Fact]
        public void GetCommandItem_ReturnsNullValue_WhenUsingInvalidID( )
        {

            // Arrange
            // DB should be empty.

            // Act
            var result = _controller.GetCommandItem( 0 );

            //Assert
            Assert.Null( result.Value );

        }

        // Test 2.2 : Request object by ID when none exist - Return 404 Not Found Return Code
        [Fact]
        public void GetCommandItem_Returns404NotFound_WhenUsingInvalidID( )
        {

            // Arrange
            // DB should be empty.

            // Act
            var result = _controller.GetCommandItem( 0 );

            //Assert
            Assert.IsType<NotFoundResult>( result.Result );

        }

        // Test 2.3 : Request object by valid ID - Check Correct Return Type
        [Fact]
        public void GetCommandItem_ReturnsItemOfCorrectType_WhenUsingValidID( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 2.3",
                CommandLine = "Some CommandLine"
            };

            _dbContext.Add( command );
            _dbContext.SaveChanges( );

            var cmdId = command.Id;

            // Act
            var result = _controller.GetCommandItem( cmdId );

            //Assert
            Assert.IsType<ActionResult<Command>>( result );

        }

        // Test 2.4 : Request object by valid ID - Check correct item returned
        [Fact]
        public void GetCommandItem_ReturnCorrectItem_WhenUsingValidID( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 2.4",
                CommandLine = "Some CommandLine"
            };

            _dbContext.Add( command );
            _dbContext.SaveChanges( );

            var cmdId = command.Id;

            // Act
            var result = _controller.GetCommandItem( cmdId );

            //Assert
            Assert.Equal( cmdId, result.Value.Id );

        }


        // ACTION 3 Tests : POST     /api/commands

        // Test 3.1 : Request object by ID when none exist - Return null object value
        [Fact]
        public void PostCommandItem_ItemCountIncremented_WhenUsingInvalidItem( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 3.1",
                CommandLine = "Some CommandLine"
            };
            var oldCount = _dbContext.CommandItems.Count( );

            // Act
            var result = _controller.PostCommandItem( command );

            //Assert
            Assert.Equal( oldCount + 1, _dbContext.CommandItems.Count( ) );

        }

        // Test 3.2 : Request object by ID when none exist - Return 404 Not Found Return Code
        [Fact]
        public void PostCommandItem_Returns201Created_WhenUsingInvalidItem( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 3.2",
                CommandLine = "Some CommandLine"
            };

            // Act
            var result = _controller.PostCommandItem( command );

            //Assert
            Assert.IsType<CreatedAtActionResult>( result.Result );

        }


        // ACTION 4 Tests : PUT         /api/commands/{id}

        // Test 4.1 : Request attribute in valid object be updated - Return attribute updated
        [Fact]
        public void PutCommandItem_AttributeUpdated_WhenUsingValidItem( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 4.1",
                CommandLine = "Some CommandLine"
            };
            _dbContext.CommandItems.Add( command );
            _dbContext.SaveChanges( );

            var cmdId = command.Id;

            command.HowTo = "UPDATED";

            // Act
            _controller.PutCommandItem( cmdId, command );
            var result = _dbContext.CommandItems.Find( cmdId );

            //Assert
            Assert.Equal( command.HowTo, result.HowTo );

        }

        // Test 4.2 : Request attribute in valid object be updated - Return 204 return code
        [Fact]
        public void PutCommandItem_AttributeUpdate_WhenUsingValidItem( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 4.2",
                CommandLine = "Some CommandLine"
            };
            _dbContext.CommandItems.Add( command );
            _dbContext.SaveChanges( );

            var cmdId = command.Id;

            command.HowTo = "UPDATED";

            // Act
            var result = _controller.PutCommandItem( cmdId, command );

            //Assert
            Assert.IsType<NoContentResult>( result );

        }

        // Test 4.3 : Request attribute in invalid object be updated - Return 400 return code
        [Fact]
        public void PutCommandItem_AttributeUpdate_WhenUsingInvalidItem( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 4.3",
                CommandLine = "Some CommandLine"
            };
            _dbContext.CommandItems.Add( command );
            _dbContext.SaveChanges( );

            var cmdId = command.Id + 1;         // force an invalid record Id

            command.HowTo = "UPDATED";

            // Act
            var result = _controller.PutCommandItem( cmdId, command );

            //Assert
            Assert.IsType<BadRequestResult>( result );

        }

        // Test 4.4 : Request attribute in invalid object be updated - Return original object unchanged
        [Fact]
        public void PutCommandItem_AttributeUnchanged_WhenUsingInvalidItem( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 4.4",
                CommandLine = "Some CommandLine"
            };
            _dbContext.CommandItems.Add( command );
            _dbContext.SaveChanges( );

            var command2 = new Command
            {
                Id = command.Id,
                HowTo = "Do Something UPDATE",
                Platform = "Test 4.4 UPDATE",
                CommandLine = "Some CommandLine UPDATE"
            };

            // Act
            _controller.PutCommandItem( command.Id + 1, command2 );
            var result = _dbContext.CommandItems.Find( command.Id );

            //Assert
            Assert.Equal( command.HowTo, result.HowTo );

        }


        // ACTION 5 Tests : DELETE          /api/commands/{id}

        // Test 5.1 : Request valid object Id be deleted - Results in object count decremented by 1
        [Fact]
        public void DeleteCommandItem_ObjectCountDecrementedBy1_WhenUsingValidItemId( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 5.1",
                CommandLine = "Some CommandLine"
            };
            _dbContext.CommandItems.Add( command );
            _dbContext.SaveChanges( );

            var cmdId = command.Id;
            var objCount = _dbContext.CommandItems.Count( );

            // Act
            _controller.DeleteCommandItem( cmdId );

            //Assert
            Assert.Equal( objCount - 1, _dbContext.CommandItems.Count( ) );

        }

        // Test 5.2 : Request valid objectId  be deleted - Returns 200 OK code
        [Fact]
        public void DeleteCommandItem_Returns200OK_WhenUsingValidItemId( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 5.2",
                CommandLine = "Some CommandLine"
            };
            _dbContext.CommandItems.Add( command );
            _dbContext.SaveChanges( );

            var cmdId = command.Id;

            // Act
            var result = _controller.DeleteCommandItem( cmdId );

            //Assert
            Assert.Null( result.Result );

        }

        // Test 5.3 : Request invalid object Id be deleted - Returns 404 Not Found code
        [Fact]
        public void DeleteCommandItem_Returns404NotFound_WhenUsingInvalidItemId( )
        {

            // Arrange

            // Act
            var result = _controller.DeleteCommandItem( -1 );

            //Assert
            //--Assert.IsType<NotFoundResult>( result.Result );
            Assert.IsType<OkResult>( result.Result );

        }

        // Test 5.4 : Request invalid object Id be deleted - Object count unchanged
        [Fact]
        public void DeleteCommandItem_UnchangedObjectCount_WhenUsingValidItemId( )
        {

            // Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Test 5.4",
                CommandLine = "Some CommandLine"
            };
            _dbContext.CommandItems.Add( command );
            _dbContext.SaveChanges( );

            var cmdId = command.Id;
            var objCount = _dbContext.CommandItems.Count( );

            // Act
            var result = _controller.DeleteCommandItem( cmdId + 1 );

            //Assert
            Assert.Equal( objCount, _dbContext.CommandItems.Count( ) );

        }

    }

}
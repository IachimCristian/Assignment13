using System.Collections.Generic;
using InfraSim.Models.Server;
using Moq;
using Xunit;

namespace InfraSim.Tests
{
    public class CommandManagerTests // Tests for Command Manager 
    {
        [Fact]
        public void Execute_CallsDo() // Tests for Do
        {
            var mockCommand = new Mock<ICommand>();
            var manager = new CommandManager();
            manager.Execute(mockCommand.Object);
            mockCommand.Verify(c => c.Do(), Times.Once);
        }

        [Fact]
        public void Undo_CallsUndoOnLastCommand() // Tests for Undo
        {
            var mockCommand = new Mock<ICommand>();
            var manager = new CommandManager();
            manager.Execute(mockCommand.Object);
            manager.Undo();
            mockCommand.Verify(c => c.Undo(), Times.Once);
        }

        [Fact]
        public void Redo_CallsRedoOnNextCommand() // Tests for Redo
        {
            var mockCommand = new Mock<ICommand>();
            var manager = new CommandManager();
            manager.Execute(mockCommand.Object);
            manager.Undo();
            manager.Redo();
            mockCommand.Verify(c => c.Redo(), Times.Once);
        }
    }
} 
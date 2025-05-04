using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InfraSim.Models.Server
{
    public class CommandManager : ICommandManager
    {
        private readonly List<ICommand> Commands = new List<ICommand>();
        private int Position = 0;

        public bool HasUndo => Position > 0;
        public bool HasRedo => Position < Commands.Count;

        public void Execute(ICommand command)
        {
            if (HasRedo)
            {
                Commands.RemoveRange(Position, Commands.Count - Position);
            }
            Commands.Add(command);
            command.Do();
            Position++;
            
            Debug.WriteLine($"Command executed. Position: {Position}, Commands count: {Commands.Count}");
        }

        public void Undo()
        {
            if (HasUndo)
            {
                Position--;
                Commands[Position].Undo();
                Debug.WriteLine($"Command undone. Position: {Position}, Commands count: {Commands.Count}");
            }
        }

        public void Redo()
        {
            if (HasRedo)
            {
                try
                {
                    var currentPosition = Position;
                    var command = Commands[currentPosition];
                    
                    command.Redo();
                    
                    Position++;
                    
                    Debug.WriteLine($"Command redone. Position: {Position}, Commands count: {Commands.Count}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error during Redo in CommandManager: {ex.Message}");
                    
                    Debug.WriteLine($"Redo failed, position unchanged: {Position}");
                }
            }
        }
    }
} 
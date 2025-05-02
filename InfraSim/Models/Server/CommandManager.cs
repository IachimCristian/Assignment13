using System.Collections.Generic;

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
        }

        public void Undo()
        {
            if (HasUndo)
            {
                Position--;
                Commands[Position].Undo();
            }
        }

        public void Redo()
        {
            if (HasRedo)
            {
                Commands[Position].Redo();
                Position++;
            }
        }
    }
} 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InfraSim.Models.Db;

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

            if (command is AddServerCommand addCmd)
            {
                SaveToDb(addCmd);
            }
        }

        private void SaveToDb(AddServerCommand cmd)
        {
            using (var context = new InfraSimContext())
            {
                var existing = context.DbServers.Find(cmd.GetServerId());
                if (existing == null)
                {
                    context.DbServers.Add(new DbServer
                    {
                        Id = cmd.GetServerId(),
                        ServerType = cmd.GetServerType(),
                        ParentId = Guid.Empty
                    });

                    context.SaveChanges();
                }
            }
        }

        public void Load(ICommand command)
        {
            Commands.Add(command);
            Position++;
        }

        public void LoadCommands(List<ICommand> commands)
        {
            Commands.Clear();
            Position = 0;
            
            foreach (var command in commands)
            {
                Commands.Add(command);
                Position++;
            }
            
            Debug.WriteLine($"Loaded {Commands.Count} commands. Position: {Position}");
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
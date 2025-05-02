namespace InfraSim.Models.Server
{
    public interface ICommand
    {
        void Do();
        void Undo();
        void Redo();
    }
} 
using API.Model;

namespace API.Data
{

    public interface ICommandRepo
    {
        Task SaveChanges();
        Task<Command?> GetCommandById(int id);
        Task<IEnumerable<Command>> GetAllCommands();
        Task CreateCommand(Command cmd);
        //teacher want this as void, due to EF internal code
        void DeleteCommand(Command cmd);
    }
}
using Company.Model;
using System.Linq.Expressions;

namespace Company.Service
{
    public interface IUser
    {
        Task<List<User>>GetUsers(Expression<Func<User>> fillter = null);
        Task Create(User user);
        Task Login(User user);
        Task Save();
        
    }
}

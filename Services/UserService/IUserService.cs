using System.Collections.Generic;
using System.Threading.Tasks;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.Services;

public interface IUserObserver
{
    void OnUsersChanged();
}

public interface IUserService
{
    IEnumerable<User> GetUsers();
    Task AddUser(string username);
    Task DeleteUser(int userId);
    void Subscribe(IUserObserver observer);
} 
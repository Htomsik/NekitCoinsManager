using System.Threading.Tasks;
using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Services;

public interface IAppSettingsService
{
    public AppSettings Settings { get; }
    
    public Task LoadSettings();
    
    public Task SaveSettings();
    
    public Task DeleteSettings();
}
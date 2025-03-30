using NekitCoinsManager.Core.Models;

namespace NekitCoinsManager.Core.Services.AppSettingsService;

public interface IAppSettingsService
{
    public AppSettings Settings { get; }
    
    public Task LoadSettings();
    
    public Task SaveSettings();
    
    public Task DeleteSettings();
}
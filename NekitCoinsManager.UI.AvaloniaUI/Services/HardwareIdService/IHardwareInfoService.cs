using System.Threading.Tasks;

namespace NekitCoinsManager.Services;

public interface IHardwareInfoService
{
    Task<string> GetHardwareIdAsync();
} 
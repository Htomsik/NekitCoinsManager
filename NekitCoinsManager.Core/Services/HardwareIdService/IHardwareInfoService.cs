using System.Threading.Tasks;

namespace NekitCoinsManager.Core.Services;

public interface IHardwareInfoService
{
    Task<string> GetHardwareIdAsync();
} 
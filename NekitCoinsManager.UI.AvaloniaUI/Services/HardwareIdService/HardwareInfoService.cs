using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace NekitCoinsManager.Services;

public class HardwareInfoService : IHardwareInfoService
{
    private string GetRegistryValue(string path, string valueName)
    {
        try
        {
            using (var key = Registry.LocalMachine.OpenSubKey(path))
            {
                return key?.GetValue(valueName)?.ToString() ?? "";
            }
        }
        catch
        {
            return "";
        }
    }

    private string GetProcessorInfo()
    {
        return GetRegistryValue(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString");
    }

    private string GetMotherboardInfo()
    {
        return GetRegistryValue(@"SYSTEM\CurrentControlSet\Control\SystemInformation", "SystemManufacturer");
    }

    private string GetBiosInfo()
    {
        return GetRegistryValue(@"HARDWARE\DESCRIPTION\System\BIOS", "BIOSVendor");
    }

    private string GetSystemDriveInfo()
    {
        try
        {
            return Environment.GetEnvironmentVariable("SystemDrive") ?? "";
        }
        catch
        {
            return "";
        }
    }

    private string ComputeHash(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    public async Task<string> GetHardwareIdAsync()
    {
        return await Task.Run(() =>
        {
            var hardwareInfo = new StringBuilder();

            hardwareInfo.Append(GetProcessorInfo());
            hardwareInfo.Append(GetMotherboardInfo());
            hardwareInfo.Append(GetBiosInfo());
            hardwareInfo.Append(GetSystemDriveInfo());

            return ComputeHash(hardwareInfo.ToString());
        });
    }
} 
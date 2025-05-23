using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using NekitCoinsManager.Services;
using NekitCoinsManager.ViewModels;
using NekitCoinsManager.Views;

namespace NekitCoinsManager
{
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Конфигурируем маппинг
            MappingConfig.ConfigureMapping();
            
            var services = new ServiceCollection();
            services.AddServices();
            Services = services.BuildServiceProvider();
            
            // Загружаем настройки приложения при старте
            Task.Run(async () => 
            {
                var appSettingsService = Services.GetRequiredService<IAppSettingsService>();
                await appSettingsService.LoadSettings();
            }).GetAwaiter().GetResult();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = Services.GetRequiredService<MainWindowViewModel>(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
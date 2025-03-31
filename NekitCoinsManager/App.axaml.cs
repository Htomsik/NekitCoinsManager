using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using NekitCoinsManager.Core.Data;
using NekitCoinsManager.Core.Services.AppSettingsService;
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
            var services = new ServiceCollection();
            services.AddServices();
            Services = services.BuildServiceProvider();

            // Инициализируем базу данных в рамках отдельной области видимости
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DbInitializer.Initialize(dbContext);
            }

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
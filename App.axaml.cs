using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using NekitCoinsManager.Data;
using NekitCoinsManager.ViewModels;
using NekitCoinsManager.Views;

namespace NekitCoinsManager
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(new AppDbContext()),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
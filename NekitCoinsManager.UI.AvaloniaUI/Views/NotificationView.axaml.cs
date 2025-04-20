using Avalonia.Controls;
using Avalonia.Input;
using NekitCoinsManager.ViewModels;

namespace NekitCoinsManager.Views;

public partial class NotificationView : UserControl
{
    public NotificationView()
    {
        InitializeComponent();
    }

    private void OnBorderTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is NotificationViewModel viewModel)
        {
            viewModel.Close();
        }
    }
}
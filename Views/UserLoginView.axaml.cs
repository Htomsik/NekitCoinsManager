using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NekitCoinsManager.Views;

public partial class UserLoginView : UserControl
{
    public UserLoginView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
} 
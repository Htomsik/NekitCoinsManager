using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NekitCoinsManager.Views;

public partial class TransactionConversionView : UserControl
{
    public TransactionConversionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
} 
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NekitCoinsManager.Views;

public partial class TransactionDepositView : UserControl
{
    public TransactionDepositView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
} 
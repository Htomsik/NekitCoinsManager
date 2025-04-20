using System;

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NekitCoinsManager.ViewModels;


namespace NekitCoinsManager;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var typeName = param.GetType().FullName!;
        
        // Для наследников TransactionViewModel возвращаем TransactionView
        if (param is TransactionViewModel)
        {
            typeName = typeof(TransactionViewModel).FullName!;
        }
        
        var name = typeName.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
using System;
using System.Threading.Tasks;
using NekitCoinsManager.ViewModels;

namespace NekitCoinsManager.Services;

/// <summary>
/// Сервис, который отвечает за навигацию между представлениями в приложении
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Текущее представление
    /// </summary>
    IViewModel CurrentView { get; }
    
    /// <summary>
    /// Событие, возникающее при изменении текущего представления
    /// </summary>
    event EventHandler<IViewModel> CurrentViewChanged;

    /// <summary>
    /// Навигация к указанному типу представления
    /// </summary>
    void NavigateTo(ViewType viewType);
    
    
    /// <summary>
    /// Возвращение к предыдущему представлению
    /// </summary>
    void NavigateBack();
} 
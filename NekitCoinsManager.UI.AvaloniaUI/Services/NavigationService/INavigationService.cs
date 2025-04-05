using System.Threading.Tasks;
using NekitCoinsManager.Models;
using NekitCoinsManager.ViewModels;

namespace NekitCoinsManager.Services;


/// <summary>
///     Обсервер для отслеживания изменения текущего представления
/// </summary>
public interface INavigationObserver
{
    public void OnViewChanged(IViewModel viewModel);
}

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
    /// Навигация к указанному типу представления
    /// </summary>
    void NavigateTo(ViewType viewType);

    /// <summary>
    /// Навигация к указанному представлению с параметрами
    /// </summary>
    Task NavigateToWithParamsAsync<TParams>(ViewType viewType, TParams parameters, ViewType returnViewType);
    
    /// <summary>
    /// Возвращение к предыдущему представлению
    /// </summary>
    void NavigateBack();
    
    /// <summary>
    /// Подписка на обновления
    /// </summary>
    void Subscribe(INavigationObserver observer);
} 
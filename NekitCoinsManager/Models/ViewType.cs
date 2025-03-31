namespace NekitCoinsManager.ViewModels;

/// <summary>
/// Типы представлений в системе, сгруппированные по категориям
/// </summary>
public enum ViewType
{
    // Авторизация и регистрация
    Login,
    Registration,
    
    // Пользовательские
    UserCard,             // Профиль пользователя
    UserTokens,           // Токены пользователя
    
    // Финансовые операции
    TransactionTransfer,          // Перевод между пользователями
    TransactionDeposit,   // Пополнение баланса
    TransactionHistory,   // История операций
    
    // Административные
    UserManagement,       // Управление пользователями
    CurrencyManagement    // Управление валютами
} 
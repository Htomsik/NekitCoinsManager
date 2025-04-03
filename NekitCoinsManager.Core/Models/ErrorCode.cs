namespace NekitCoinsManager.Core.Models
{
    /// <summary>
    /// Коды ошибок, используемые в системе
    /// </summary>
    public enum ErrorCode
    {
        // Общие ошибки
        CommonNone,
        CommonEntityNotFound,
        
        // Ошибки пользователя
        UserUsernameEmpty,
        UserPasswordHashEmpty,
        UserUsernameTooShort,
        UserUsernameTooLong,
        UserUsernameInvalidCharacters,
        UserUsernameAlreadyExists,
        UserNotFound,
        UserCannotDeleteBankAccount,
        UserHasBalances,
        UserPasswordEmpty,
        UserPasswordTooShort,
        UserPasswordTooLong,
        UserPasswordNotComplex,
        
        // Ошибки транзакций
        TransactionFromUserIdInvalid,
        TransactionToUserIdInvalid,
        TransactionCurrencyIdInvalid,
        TransactionAmountMustBePositive,
        TransactionInvalidType,
        TransactionSelfTransactionNotAllowed,
        TransactionDepositFromNonBank,
        TransactionParentNotFound,
        TransactionNotFound,
        TransactionCannotBeModified,
        TransactionHasChildTransactions,
        TransactionCannotBeDeleted,
        TransactionInsufficientFunds,
        
        // Ошибки валют
        CurrencyNameEmpty,
        CurrencyCodeEmpty,
        CurrencySymbolEmpty,
        CurrencyCodeNotUnique,
        CurrencyNotFound,
        CurrencyInactive,
        CurrencyNameTooLong,
        CurrencySymbolTooLong,
        CurrencyRateMustBePositive,
        CurrencyRateTooHigh,
        CurrencyHasTransactions,
        CurrencyHasBalances,
        
        // Ошибки баланса пользователя
        BalanceUserIdInvalid,
        BalanceAmountNegative,
        BalanceAlreadyExists,
        BalanceNotFound,
        
        // Ошибки аутентификационных токенов
        AuthTokenUserIdInvalid,
        AuthTokenEmpty,
        AuthTokenHardwareIdEmpty,
        AuthTokenCreatedAtInvalid,
        AuthTokenExpirationDateInvalid,
        AuthTokenAlreadyExists,
        AuthTokenNotFound,
        AuthTokenInactive,
        AuthTokenHardwareIdMismatch,
        AuthTokenExpired
    }
} 
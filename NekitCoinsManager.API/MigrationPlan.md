# План миграции NekitCoinsManager на API-архитектуру

## 1. Подготовка API-проекта (завершено)
- ✅ Создание проекта NekitCoinsManager.API
- ✅ Настройка базовой инфраструктуры (DI, маппинг)
- ✅ Настройка обработки ошибок через Middleware

## 2. Создание контроллеров API (завершено)
- ✅ **AuthTokenController** - авторизация и работа с токенами
- ✅ **UserController** - управление пользователями
- ✅ **CurrencyController** - управление валютами
- ✅ **UserBalanceController** - управление балансами пользователей
- ✅ **TransactionController** - работа с транзакциями
- ✅ **CurrencyConversionController** - конвертация валют
- ✅ **MoneyOperationsController** - управление денежными операциями

## 3. Создание HTTP-клиентов для API (частично завершено)
- ✅ Интерфейсы клиентов в Shared проекте
- ✅ Модели DTO в Shared проекте (перенесены из API/Models)
- ⬜ Создание реальных клиентов в UI проекте для работы с API
- ⬜ Замена локальных клиентов на API-клиенты

## 4. Миграция функциональности (предстоит)
- ⬜ Авторизация и аутентификация
- ⬜ Управление пользователями
- ⬜ Управление валютами
- ⬜ Управление балансами
- ⬜ Управление транзакциями

## 5. Тестирование и отладка (предстоит)
- ⬜ Юнит-тестирование
- ⬜ Интеграционное тестирование
- ⬜ End-to-end тестирование

## 6. Переход на API (предстоит)
- ⬜ Переключение UI проекта на использование API
- ⬜ Удаление устаревшего кода из UI-проекта

## Следующие шаги по приоритету

1. ✅ **TransactionController** (сложность: 4) - работа с транзакциями
2. ✅ **CurrencyConversionController** (сложность: 3) - конвертация валют
3. ✅ **MoneyOperationsController** (сложность: 5) - сложные финансовые операции
4. **HTTP-клиенты для API** (сложность: 4) - создание и интеграция клиентов

## Ключевые принципы реализации
- Контроллеры должны быть простыми и только перенаправлять вызовы на сервисы
- Бизнес-логика должна находиться в сервисах, а не в контроллерах
- Использовать модели DTO с валидацией для входных данных
- Обеспечить единообразие API-ответов и обработки ошибок
- Следовать RESTful-стилю в наименованиях эндпоинтов (camelCase)
- Хранить все DTO модели в проекте Shared для повторного использования
- HTTP-клиенты должны быть тонкими и четко соответствовать контроллерам API

## Детали по контроллерам

### AuthTokenController (✅ Реализовано)
- **Эндпоинты:**
  - POST `/api/AuthToken/create` - создание токена
  - POST `/api/AuthToken/validate` - проверка токена
  - POST `/api/AuthToken/deactivate/{tokenId}` - деактивация токена
  - POST `/api/AuthToken/deactivateAll/{userId}` - деактивация всех токенов пользователя
  - GET `/api/AuthToken/user/{userId}` - получение токенов пользователя
  - POST `/api/AuthToken/restoreSession` - восстановление сессии по токену

### UserController (✅ Реализовано)
- **Эндпоинты:**
  - GET `/api/User` - получение всех пользователей
  - GET `/api/User/{id}` - получение пользователя по ID
  - GET `/api/User/byUsername/{username}` - получение пользователя по имени
  - POST `/api/User/register` - регистрация пользователя
  - DELETE `/api/User/{id}` - удаление пользователя
  - POST `/api/User/verifyPassword` - проверка пароля
  - POST `/api/User/login` - вход пользователя

### CurrencyController (✅ Реализовано)
- **Эндпоинты:**
  - GET `/api/Currency` - получение всех валют
  - GET `/api/Currency/{id}` - получение валюты по ID
  - GET `/api/Currency/byCode/{code}` - получение валюты по коду
  - POST `/api/Currency` - добавление валюты
  - PUT `/api/Currency` - обновление валюты
  - DELETE `/api/Currency/{id}` - удаление валюты
  - PATCH `/api/Currency/rate` - обновление обменного курса

### UserBalanceController (✅ Реализовано)
- **Эндпоинты:**
  - GET `/api/UserBalance/user/{userId}` - получение всех балансов пользователя
  - GET `/api/UserBalance/user/{userId}/currency/{currencyId}` - получение баланса по валюте
  - POST `/api/UserBalance` - создание баланса
  - PUT `/api/UserBalance` - обновление баланса
  - POST `/api/UserBalance/transfer` - перевод средств между пользователями
  - POST `/api/UserBalance/getOrCreate` - получение баланса или создание, если не существует
  - POST `/api/UserBalance/validate` - проверка доступности суммы

### TransactionController (✅ Реализовано)
- **Эндпоинты:**
  - GET `/api/Transaction` - получение всех транзакций
  - GET `/api/Transaction/{id}` - получение транзакции по ID
  - POST `/api/Transaction` - добавление транзакции
  - POST `/api/Transaction/validate` - валидация транзакции

### CurrencyConversionController (✅ Реализовано)
- **Эндпоинты:**
  - POST `/api/CurrencyConversion/convert` - конвертация суммы
  - GET `/api/CurrencyConversion/rate` - получение курса обмена между валютами
  - GET `/api/CurrencyConversion/rates` - получение всех курсов обмена

### MoneyOperationsController (✅ Реализовано)
- **Эндпоинты:**
  - POST `/api/MoneyOperations/transfer` - перевод денег
  - POST `/api/MoneyOperations/deposit` - пополнение баланса
  - POST `/api/MoneyOperations/convert` - конвертация валют
  - POST `/api/MoneyOperations/welcomeBonus` - выдача приветственного бонуса

## Детали по HTTP-клиентам для API

### AuthTokenServiceClient (⬜ В планах)
- **Методы:**
  - `CreateTokenAsync` - создание токена
  - `ValidateTokenAsync` - проверка токена
  - `DeactivateTokenAsync` - деактивация токена
  - `DeactivateAllUserTokensAsync` - деактивация всех токенов пользователя
  - `GetUserTokensAsync` - получение токенов пользователя
  - `RestoreSessionAsync` - восстановление сессии по токену

### UserServiceClient (⬜ В планах)
- **Методы:**
  - `GetAllUsersAsync` - получение всех пользователей
  - `GetUserByIdAsync` - получение пользователя по ID
  - `GetUserByUsernameAsync` - получение пользователя по имени
  - `RegisterUserAsync` - регистрация пользователя
  - `DeleteUserAsync` - удаление пользователя
  - `VerifyPasswordAsync` - проверка пароля
  - `LoginAsync` - вход пользователя

### CurrencyServiceClient (⬜ В планах)
- **Методы:**
  - `GetAllCurrenciesAsync` - получение всех валют
  - `GetCurrencyByIdAsync` - получение валюты по ID
  - `GetCurrencyByCodeAsync` - получение валюты по коду
  - `AddCurrencyAsync` - добавление валюты
  - `UpdateCurrencyAsync` - обновление валюты
  - `DeleteCurrencyAsync` - удаление валюты
  - `UpdateExchangeRateAsync` - обновление обменного курса

### UserBalanceServiceClient (⬜ В планах)
- **Методы:**
  - `GetUserBalancesAsync` - получение всех балансов пользователя
  - `GetUserBalanceAsync` - получение баланса по валюте
  - `CreateBalanceAsync` - создание баланса
  - `UpdateBalanceAsync` - обновление баланса
  - `TransferBalanceAsync` - перевод средств между пользователями
  - `GetOrCreateBalanceAsync` - получение баланса или создание, если не существует
  - `ValidateUserBalanceAsync` - проверка доступности суммы

### TransactionServiceClient (⬜ В планах)
- **Методы:**
  - `GetTransactionsAsync` - получение всех транзакций
  - `GetTransactionByIdAsync` - получение транзакции по ID
  - `AddTransactionAsync` - добавление транзакции
  - `ValidateTransactionAsync` - валидация транзакции

### CurrencyConversionServiceClient (⬜ В планах)
- **Методы:**
  - `ConvertAsync` - конвертация суммы
  - `GetExchangeRateAsync` - получение курса обмена между валютами
  - `GetAllExchangeRatesAsync` - получение всех курсов обмена

### MoneyOperationsServiceClient (⬜ В планах)
- **Методы:**
  - `TransferAsync` - перевод денег
  - `DepositAsync` - пополнение баланса
  - `ConvertAsync` - конвертация валют
  - `GrantWelcomeBonusAsync` - выдача приветственного бонуса

## Ключевые аспекты реализации HTTP-клиентов
- Использование `HttpClient` для взаимодействия с API
- Обработка ошибок и исключений HTTP
- Сериализация и десериализация DTO-объектов
- Управление токенами аутентификации
- Обработка повторных попыток и тайм-аутов
- Кэширование результатов для часто используемых операций
- Логирование запросов и ответов

## Текущий прогресс
- Контроллеры: 7 из 7 реализованы (100%)
- HTTP-клиенты: 0 из 7 реализованы (0%)
- Общий прогресс миграции: примерно 50% 
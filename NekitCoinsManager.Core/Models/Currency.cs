using System;

namespace NekitCoinsManager.Core.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Признак того, что валюта начисляется новым пользователям при регистрации
        /// </summary>
        public bool IsDefaultForNewUsers { get; set; }
        
        /// <summary>
        /// Количество валюты, начисляемое при регистрации
        /// </summary>
        public decimal DefaultAmount { get; set; }
        
        /// <summary>
        /// Процент комиссии при конвертации из этой валюты
        /// </summary>
        public decimal ConversionFeePercentage { get; set; } = 0.01m; // 1% по умолчанию
    }
} 
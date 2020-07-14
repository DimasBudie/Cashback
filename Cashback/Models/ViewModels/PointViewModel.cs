using System;
using Cashback.Enum;

namespace Cashback.Models.ViewModel
{
    public class PurchaseViewModel 
    {   
        public string Code { get; set; }

        public string Cpf { get; set; }

        public decimal Value { get; set; }

        public DateTimeOffset Date { get; set; }

        public decimal CashbackPercent { get; set; }

        public decimal CashbackValue { get; set; }

        public EnumStatus Status { get; set; }

    }
}
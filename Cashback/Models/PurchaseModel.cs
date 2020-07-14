using System;
using Cashback.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cashback.Models
{
    public class Purchase
    {   
         public Purchase(string code, decimal value, string email, string cpf)
        {
            this.Code = code;
            this.Value = value;
            this.Cpf = cpf;
            this.Email = email;
            //Irá verificar o CPF para setar o satus            
            this.Status = GlobalSettings.Configuration["DefautCpf:Cpf"] == cpf ? EnumStatus.Approved : EnumStatus.OnChecking;
            this.CalcCashback();
        }        

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get;set;}

        public DateTimeOffset? CreatedAt { get; set; }

        public string Email { get; set; }

        public string Cpf { get; set; }

        public string Code { get; set; }

        public decimal Value { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnumStatus Status { get; set; }

        public decimal CashbackPercent { get; set; }

        public decimal CashbackValue { get; set; }


        private void CalcCashback()
        {
            if (Value < 1000M)
            {
                CashbackPercent = 10;
            }
            else if (Value <= 1500M)
            {
                CashbackPercent = 15;
            }
            else
            {
                CashbackPercent = 20;
            }

            CashbackValue = Math.Round( (CashbackPercent / 100) * Value, 2 );
        }

        public void UpdateValues(string code, decimal value,  string cpf)
        {
            Code = code;
            Value = value;
            Cpf = cpf;
            this.Status = GlobalSettings.Configuration["DefautCpf:Cpf"] == cpf ? EnumStatus.Approved : EnumStatus.OnChecking;
            CalcCashback();
        }
    }
}
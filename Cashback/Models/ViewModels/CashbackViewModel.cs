namespace Cashback.Models.ViewModel
{
    public class CashbackApiViewModel
    {
        public int StatusCode { get; set; }
        public Body Body { get; set; }
    }

    public class Body
    {
        public decimal Credit { get; set; }
    }
}
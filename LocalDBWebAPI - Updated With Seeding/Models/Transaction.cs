namespace LocalDBWebAPI.Models
{
    public class Transaction
    {
        public string? date { get; set; }
        public string? sender { get; set; }
        public string? receiver { get; set; }
        public string? type { get; set; }
        public double amount { get; set; }
        public string? description { get; set; }
    }
}

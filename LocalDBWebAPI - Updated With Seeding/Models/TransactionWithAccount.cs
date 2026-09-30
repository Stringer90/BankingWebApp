namespace LocalDBWebAPI.Models
{
    public class TransactionWithAccount
    {
        public string? date { get; set; }
        public string? type { get; set; }
        public string? counterparty { get; set; }
        public double amount { get; set; }
        public string? description { get; set; }


    }
}

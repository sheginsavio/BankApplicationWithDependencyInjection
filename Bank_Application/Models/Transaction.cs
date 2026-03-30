namespace Bank_Application.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        // Foreign key reference
        public Account Account { get; set; }

        // Navigation property
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}

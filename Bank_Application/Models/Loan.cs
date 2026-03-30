namespace Bank_Application.Models
{
    public class Loan
    {
        public int LoanId { get; set; }
        public int CustomerId { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public string LoanStatus { get; set; }

        // Foreign key reference
        public Customer Customer { get; set; }

        // Navigation property
        public ICollection<Repayment> Repayments { get; set; } = new List<Repayment>();
    }
}

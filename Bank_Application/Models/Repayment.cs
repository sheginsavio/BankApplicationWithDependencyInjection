namespace Bank_Application.Models
{
    public class Repayment
    {
        public int RepaymentId { get; set; }
        public int LoanId { get; set; }
        public DateTime RepaymentDate { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceRemaining { get; set; }

        // Foreign key reference
        public Loan Loan { get; set; }
    }
}

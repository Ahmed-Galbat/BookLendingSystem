using BookLendingSystem.Application.Interfaces;

namespace BookLendingSystem.Api.Jobs
{
    public class OverdueLoanJob
    {
        private readonly ILoanService _loanService;
        private readonly ILogger<OverdueLoanJob> _logger;

        public OverdueLoanJob(ILoanService loanService, ILogger<OverdueLoanJob> logger)
        {
            _loanService = loanService;
            _logger = logger;
        }
        public async Task CheckOverdueLoansAsync()
        {
            try
            {
                var overdueLoans =await _loanService.GetOverdueLoansAsync();
                foreach (var loan in overdueLoans)
                {
                    _logger.LogWarning(
                        "Overdue loan detected: LoanId={LoanId}, BookId={BookId}, UserId={UserId}, DueDate={DueDate}",
                        loan.Id, loan.BookId, loan.UserId, loan.DueDate);
                }
            }
            catch
            {

            }
        }
    }
}

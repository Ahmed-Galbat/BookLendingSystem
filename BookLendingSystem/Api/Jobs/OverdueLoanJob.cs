using BookLendingSystem.Application.Features.Loans.Queries.GetOverdueLoans;
using BookLendingSystem.Application.Interfaces;
using MediatR;

namespace BookLendingSystem.Api.Jobs
{
    public class OverdueLoanJob
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OverdueLoanJob> _logger;

        public OverdueLoanJob(IMediator mediator, ILogger<OverdueLoanJob> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task CheckOverdueLoansAsync()
        {
            var query = new GetOverdueLoansQuery();
            var overdueLoans = await _mediator.Send(query);

            foreach (var loan in overdueLoans)
            {
                _logger.LogWarning(
                    "Overdue loan detected: LoanId={LoanId}, BookId={BookId}, UserId={UserId}, DueDate={DueDate}",
                    loan.Id, loan.BookId, loan.UserId, loan.DueDate);
            }
        }
    }
}

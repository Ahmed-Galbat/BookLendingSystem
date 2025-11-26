using BookLendingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Interfaces
{
    public interface ILoanService
    {
        // Member operations
        Task<LoanDto> BorrowBookAsync(Guid bookId, string userId);
        Task<LoanDto> ReturnBookAsync(Guid loanId, string userId);
        Task<IEnumerable<LoanDto>> GetUserLoansAsync(string userId);

        // Admin operations
        Task<IEnumerable<LoanDto>> GetAllLoansAsync();
        Task<IEnumerable<LoanDto>> GetOverdueLoansAsync();
    }
}

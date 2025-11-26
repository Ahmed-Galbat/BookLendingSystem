using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly IRepository<Book, Guid> _bookRepository;
        private readonly IRepository<Loan, Guid> _loanRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly IMapper _mapper;

        public LoanService(
            IRepository<Book, Guid> bookRepository,
            IRepository<Loan, Guid> loanRepository,
            IDateTimeService dateTimeService,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
            _dateTimeService = dateTimeService;
            _mapper = mapper;
        }

        public async Task<LoanDto> BorrowBookAsync(Guid bookId, string userId)
        {
            // Check if user has any active loans (one at a time rule)
            var activeLoans = await _loanRepository.FindAsync(l => l.UserId == userId && l.ReturnDate == null);
            if (activeLoans.Any())
            {
                throw new InvalidOperationException("Member can only borrow one book at a time.");
            }

            // Check if book is available
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null || !book.IsAvailable)
            {
                throw new KeyNotFoundException($"Book with ID {bookId} is not available for borrowing.");
            }

            // Create loan
            var now = _dateTimeService.Now;
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                UserId = userId,
                BorrowDate = now,
                DueDate = now.AddDays(7), 
                Book = book 
            };

            // Update book availability
            book.AvailableCopies--;
            await _bookRepository.UpdateAsync(book);

            // Save loan
            await _loanRepository.AddAsync(loan);

            return _mapper.Map<LoanDto>(loan);
        }

        public async Task<LoanDto> ReturnBookAsync(Guid loanId, string userId)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);

            if (loan == null || loan.UserId != userId)
            {
                throw new KeyNotFoundException($"Loan with ID {loanId} not found or does not belong to user.");
            }

            if (loan.IsReturned)
            {
                throw new InvalidOperationException("Book has already been returned.");
            }

            // Update loan
            loan.ReturnDate = _dateTimeService.Now;
            await _loanRepository.UpdateAsync(loan);

            // Update book availability
            var book = await _bookRepository.GetByIdAsync(loan.BookId);
            if (book != null)
            {
                book.AvailableCopies++;
                await _bookRepository.UpdateAsync(book);
            }

            // Attach book for mapping
            loan.Book = book;

            return _mapper.Map<LoanDto>(loan);
        }

        public async Task<IEnumerable<LoanDto>> GetUserLoansAsync(string userId)
        {
            var loans = await _loanRepository.FindAsync(l => l.UserId == userId);
            
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
        {
            var now = _dateTimeService.Now;
            var overdueLoans = await _loanRepository.FindAsync(l => l.ReturnDate == null && l.DueDate < now);
            return _mapper.Map<IEnumerable<LoanDto>>(overdueLoans);
        }
    }
}

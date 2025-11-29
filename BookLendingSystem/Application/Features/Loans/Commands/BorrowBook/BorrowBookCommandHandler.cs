using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Features.Loans.Commands.BorrowBook
{
    public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, LoanDto>
    {
        private readonly IRepository<Book, Guid> _bookRepository;
        private readonly IRepository<Loan, Guid> _loanRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly IMapper _mapper;

        public BorrowBookCommandHandler(
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
        async Task<LoanDto> IRequestHandler<BorrowBookCommand, LoanDto>.Handle(BorrowBookCommand request, CancellationToken cancellationToken)
        {
            // Check if user has any active loans (one at a time rule)
            var activeLoans = await _loanRepository.FindAsync(l => l.UserId == request.UserId && l.ReturnDate == null);
            if (activeLoans.Any())
            {
                throw new InvalidOperationException("Member can only borrow one book at a time.");
            }

            // Check if book is available
            var book = await _bookRepository.GetByIdAsync(request.BookId);
            if (book == null || !book.IsAvailable)
            {
                throw new KeyNotFoundException($"Book with ID {request.BookId} is not available for borrowing.");
            }

            // Create loan
            var now = _dateTimeService.Now;
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                BookId = request.BookId,
                UserId = request.UserId,
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
    }
}

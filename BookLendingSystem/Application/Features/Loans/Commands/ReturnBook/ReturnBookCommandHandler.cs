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

namespace BookLendingSystem.Application.Features.Loans.Commands.ReturnBook
{
    public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, LoanDto>
    {
        private readonly IRepository<Book, Guid> _bookRepository;
        private readonly IRepository<Loan, Guid> _loanRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly IMapper _mapper;

        public ReturnBookCommandHandler(
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

        public async Task<LoanDto> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {
            var loan = await _loanRepository.GetByIdAsync(request.LoanId);

            if (loan == null || loan.UserId != request.UserId)
            {
                throw new KeyNotFoundException($"Loan with ID {request.LoanId} not found or does not belong to user.");
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
    }
}

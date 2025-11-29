using BookLendingSystem.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Features.Loans.Commands.BorrowBook
{
    public class BorrowBookCommand : IRequest<LoanDto>
    {
        public Guid BookId { get; set; }
        public string UserId { get; set; }
    }
}

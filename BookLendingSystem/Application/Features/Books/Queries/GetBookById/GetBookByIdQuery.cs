using BookLendingSystem.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Features.Books.Queries.GetBookById
{
    public class GetBookByIdQuery : IRequest<BookDto>
    {
        public Guid Id { get; set; }
    }
}

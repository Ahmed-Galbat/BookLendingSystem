using BookLendingSystem.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Features.Books.Queries.GetAllBooks
{
    public class GetAllBooksQuery : IRequest<IEnumerable<BookDto>>
    {
    }
}

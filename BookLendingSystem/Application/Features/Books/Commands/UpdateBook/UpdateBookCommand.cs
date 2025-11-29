using BookLendingSystem.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommand : IRequest<BookDto>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
    }
}

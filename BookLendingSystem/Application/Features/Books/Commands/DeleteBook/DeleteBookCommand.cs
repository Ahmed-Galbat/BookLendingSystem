using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Features.Books.Commands.DeleteBook
{
    public class DeleteBookCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}

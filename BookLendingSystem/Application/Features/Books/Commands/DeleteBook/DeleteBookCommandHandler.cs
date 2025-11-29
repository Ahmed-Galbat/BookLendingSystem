using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Features.Books.Commands.DeleteBook
{
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Unit>
    {
        private readonly IRepository<Book, Guid> _bookRepository;

        public DeleteBookCommandHandler(IRepository<Book, Guid> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Unit> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {request.Id} not found.");
            }

            await _bookRepository.DeleteAsync(book);
            return Unit.Value;
        }
    }
}

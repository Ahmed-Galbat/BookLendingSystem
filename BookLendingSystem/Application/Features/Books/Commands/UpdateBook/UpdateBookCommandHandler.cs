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

namespace BookLendingSystem.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
    {
        private readonly IRepository<Book, Guid> _bookRepository;
        private readonly IMapper _mapper;

        public UpdateBookCommandHandler(IRepository<Book, Guid> bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {request.Id} not found.");
            }

            // Check for duplicate ISBN (excluding current book)
            var existingBooks = await _bookRepository.FindAsync(b => b.ISBN == request.ISBN && b.Id != request.Id);
            if (existingBooks.Any())
            {
                throw new InvalidOperationException($"A book with ISBN '{request.ISBN}' already exists.");
            }

            // Calculate the change in available copies
            int copiesDifference = request.TotalCopies - book.TotalCopies;
            book.AvailableCopies += copiesDifference;

            // Update properties
            book.Title = request.Title;
            book.Author = request.Author;
            book.ISBN = request.ISBN;
            book.TotalCopies = request.TotalCopies;

            await _bookRepository.UpdateAsync(book);
            return _mapper.Map<BookDto>(book);
        }
    }
}

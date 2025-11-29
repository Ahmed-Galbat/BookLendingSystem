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

namespace BookLendingSystem.Application.Features.Books.Commands.CreateBook
{
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
    {
        private readonly IRepository<Book, Guid> _bookRepository;
        private readonly IMapper _mapper;

        public CreateBookCommandHandler(IRepository<Book, Guid> bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            // Check for duplicate ISBN
            var existingBooks = await _bookRepository.FindAsync(b => b.ISBN == request.ISBN);
            if (existingBooks.Any())
            {
                throw new InvalidOperationException($"A book with ISBN '{request.ISBN}' already exists.");
            }

            var book = _mapper.Map<Book>(request);
            book.Id = Guid.NewGuid();
            book.AvailableCopies = book.TotalCopies;

            await _bookRepository.AddAsync(book);
            return _mapper.Map<BookDto>(book);
        }
    }
}

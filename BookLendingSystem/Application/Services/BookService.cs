using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IRepository<Book, Guid> _bookRepository;
        private readonly IMapper _mapper;

        public BookService(IRepository<Book, Guid> bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<BookDto> AddBookAsync(CreateBookDto bookDto)
        {
            // Check for duplicate ISBN
            var existingBooks = await _bookRepository.FindAsync(b => b.ISBN == bookDto.ISBN);
            if (existingBooks.Any())
            {
                throw new InvalidOperationException($"A book with ISBN '{bookDto.ISBN}' already exists.");
            }

            var book = _mapper.Map<Book>(bookDto);
            book.Id = Guid.NewGuid();
            book.AvailableCopies = book.TotalCopies; 

            await _bookRepository.AddAsync(book);
            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> UpdateBookAsync(Guid id, UpdateBookDto bookDto)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }

            // Check for duplicate ISBN (excluding current book)
            var existingBooks = await _bookRepository.FindAsync(b => b.ISBN == bookDto.ISBN && b.Id != id);
            if (existingBooks.Any())
            {
                throw new InvalidOperationException($"A book with ISBN '{bookDto.ISBN}' already exists.");
            }

            // Calculate the change in available copies
            int copiesDifference = bookDto.TotalCopies - book.TotalCopies;
            book.AvailableCopies += copiesDifference;

            // Update properties
            _mapper.Map(bookDto, book);

            await _bookRepository.UpdateAsync(book);
            return _mapper.Map<BookDto>(book);
        }

        public async Task DeleteBookAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }
            
            await _bookRepository.DeleteAsync(book);
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto> GetBookByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }
            return _mapper.Map<BookDto>(book);
        }
    }
}

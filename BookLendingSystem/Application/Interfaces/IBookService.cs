using BookLendingSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Interfaces
{
    public interface IBookService
    {
        // Admin operations
        Task<BookDto> AddBookAsync(CreateBookDto bookDto);
        Task<BookDto> UpdateBookAsync(Guid id, UpdateBookDto bookDto);
        Task DeleteBookAsync(Guid id);

        // Member operations
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto> GetBookByIdAsync(Guid id);
    }
}

using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Application.MappingProfiles;
using BookLendingSystem.Application.Services;
using BookLendingSystem.Domain.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookLendingSystem.Tests
{
    public class BookServiceTests
    {
        private readonly IRepository<Book, Guid> _mockRepository;
        private readonly IMapper _mapper;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockRepository = Substitute.For<IRepository<Book, Guid>>();
            
            // Setup AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BookLendingProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _bookService = new BookService(_mockRepository, _mapper);
        }

        [Fact]
        public async Task AddBookAsync_ShouldCallRepositoryAddAndReturnDto()
        {
            // Arrange
            var createDto = new CreateBookDto
            {
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                TotalCopies = 5
            };

            // Act
            var result = await _bookService.AddBookAsync(createDto);

            // Assert
            await _mockRepository.Received(1).AddAsync(Arg.Is<Book>(b => 
                b.Title == createDto.Title && 
                b.AvailableCopies == createDto.TotalCopies));
            Assert.NotNull(result);
            Assert.Equal(createDto.Title, result.Title);
            Assert.Equal(createDto.TotalCopies, result.TotalCopies);
            Assert.True(result.IsAvailable);
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnBookDto_WhenBookExists()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Existing Book", AvailableCopies = 1, TotalCopies = 1 };
            _mockRepository.GetByIdAsync(bookId).Returns(book);

            // Act
            var result = await _bookService.GetBookByIdAsync(bookId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.Title, result.Title);
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldThrowKeyNotFoundException_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _mockRepository.GetByIdAsync(bookId).Returns((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _bookService.GetBookByIdAsync(bookId));
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldUpdateBookAndAvailableCopies()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var existingBook = new Book { Id = bookId, Title = "Old Title", TotalCopies = 5, AvailableCopies = 3 };
            var updateDto = new UpdateBookDto { Title = "New Title", TotalCopies = 10, Author = "A", ISBN = "B" };
            _mockRepository.GetByIdAsync(bookId).Returns(existingBook);

            // Act
            var result = await _bookService.UpdateBookAsync(bookId, updateDto);

            // Assert
            await _mockRepository.Received(1).UpdateAsync(Arg.Is<Book>(b => 
                b.Title == "New Title" && 
                b.TotalCopies == 10 && 
                b.AvailableCopies == 8)); 
            Assert.Equal("New Title", result.Title);
            Assert.Equal(10, result.TotalCopies);
            Assert.Equal(8, result.AvailableCopies);
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldCallRepositoryDelete()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "To Delete" };
            _mockRepository.GetByIdAsync(bookId).Returns(book);

            // Act
            await _bookService.DeleteBookAsync(bookId);

            // Assert
            await _mockRepository.Received(1).DeleteAsync(book);
        }
    }
}

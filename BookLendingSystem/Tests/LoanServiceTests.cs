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
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace BookLendingSystem.Tests
{
    public class LoanServiceTests
    {
        private readonly IRepository<Book, Guid> _mockBookRepository;
        private readonly IRepository<Loan, Guid> _mockLoanRepository;
        private readonly IDateTimeService _mockDateTimeService;
        private readonly IMapper _mapper;
        private readonly LoanService _loanService;
        private readonly Guid _bookId = Guid.NewGuid();
        private readonly string _userId = "test-user-id";
        private readonly DateTime _fixedNow = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);

        public LoanServiceTests()
        {
            _mockBookRepository = Substitute.For<IRepository<Book, Guid>>();
            _mockLoanRepository = Substitute.For<IRepository<Loan, Guid>>();
            _mockDateTimeService = Substitute.For<IDateTimeService>();
            _mockDateTimeService.Now.Returns(_fixedNow);

            // Setup AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BookLendingProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _loanService = new LoanService(_mockBookRepository, _mockLoanRepository, _mockDateTimeService, _mapper);
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldCreateLoanAndUpdateBook_WhenAvailableAndNoActiveLoans()
        {
            // Arrange
            var book = new Book { Id = _bookId, Title = "Available Book", TotalCopies = 5, AvailableCopies = 5 };
            _mockBookRepository.GetByIdAsync(_bookId).Returns(book);
            _mockLoanRepository.FindAsync(Arg.Any<Expression<Func<Loan, bool>>>()).Returns(new List<Loan>());

            // Act
            var result = await _loanService.BorrowBookAsync(_bookId, _userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_bookId, result.BookId);
            Assert.Equal(_userId, result.UserId);
            Assert.Equal(_fixedNow, result.BorrowDate);
            Assert.Equal(_fixedNow.AddDays(7), result.DueDate);
            Assert.Equal(4, book.AvailableCopies); // Available copies should decrease
            await _mockBookRepository.Received(1).UpdateAsync(book);
            await _mockLoanRepository.Received(1).AddAsync(Arg.Any<Loan>());
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldThrowException_WhenUserHasActiveLoan()
        {
            // Arrange
            var book = new Book { Id = _bookId, Title = "Available Book", TotalCopies = 5, AvailableCopies = 5 };
            var activeLoan = new Loan { Id = Guid.NewGuid(), UserId = _userId, ReturnDate = null };
            _mockBookRepository.GetByIdAsync(_bookId).Returns(book);
            _mockLoanRepository.FindAsync(Arg.Any<Expression<Func<Loan, bool>>>()).Returns(new List<Loan> { activeLoan });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _loanService.BorrowBookAsync(_bookId, _userId));
            await _mockBookRepository.DidNotReceive().UpdateAsync(Arg.Any<Book>());
            await _mockLoanRepository.DidNotReceive().AddAsync(Arg.Any<Loan>());
        }

        [Fact]
        public async Task ReturnBookAsync_ShouldUpdateLoanAndIncreaseBookAvailability()
        {
            // Arrange
            var loanId = Guid.NewGuid();
            var book = new Book { Id = _bookId, Title = "Borrowed Book", TotalCopies = 5, AvailableCopies = 4 };
            var loan = new Loan { Id = loanId, BookId = _bookId, UserId = _userId, ReturnDate = null, Book = book };
            _mockLoanRepository.GetByIdAsync(loanId).Returns(loan);
            _mockBookRepository.GetByIdAsync(_bookId).Returns(book);

            // Act
            var result = await _loanService.ReturnBookAsync(loanId, _userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsReturned);
            Assert.Equal(_fixedNow, loan.ReturnDate);
            Assert.Equal(5, book.AvailableCopies); // Available copies should increase
            await _mockLoanRepository.Received(1).UpdateAsync(loan);
            await _mockBookRepository.Received(1).UpdateAsync(book);
        }


        [Fact]
        public async Task GetOverdueLoansAsync_ShouldReturnOnlyOverdueLoans()
        {
            // Arrange
            var overdueLoan = new Loan
            {
                Id = Guid.NewGuid(),
                BookId = _bookId,
                UserId = _userId,
                ReturnDate = null,
                DueDate = _fixedNow.AddDays(-1)
            };
            var onTimeLoan = new Loan
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                UserId = _userId,
                ReturnDate = null,
                DueDate = _fixedNow.AddDays(1)
            };

            var loans = new List<Loan> { overdueLoan, onTimeLoan };

            _mockLoanRepository
                .FindAsync(Arg.Any<Expression<Func<Loan, bool>>>())
                .Returns(ci =>
                {
                    var predicate = ci.Arg<Expression<Func<Loan, bool>>>().Compile();
                    return loans.Where(predicate).ToList();
                });

            // Act
            var result = await _loanService.GetOverdueLoansAsync();

            // Assert
            Assert.Single(result);
            Assert.Contains(result, l => l.Id == overdueLoan.Id);
        }
    }
}

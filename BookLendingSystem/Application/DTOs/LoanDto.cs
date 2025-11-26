using System;
using System.ComponentModel.DataAnnotations;

namespace BookLendingSystem.Application.DTOs
{
    public class LoanDto
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class BorrowBookDto
    {
        [Required(ErrorMessage = "Book ID is required")]
        public Guid BookId { get; set; }
        
    }

    public class ReturnBookDto
    {
        [Required(ErrorMessage = "Loan ID is required")]
        public Guid LoanId { get; set; }
        
    }
}

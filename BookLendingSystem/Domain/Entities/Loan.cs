using System;

namespace BookLendingSystem.Domain.Entities
{
    public class Loan : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string UserId { get; set; } 
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned => ReturnDate.HasValue;
        public bool IsOverdue => !IsReturned && DateTime.UtcNow > DueDate;

       
        public Book Book { get; set; }
        
    }
}

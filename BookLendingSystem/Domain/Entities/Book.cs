using System;

namespace BookLendingSystem.Domain.Entities
{
    public class Book : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public bool IsAvailable => AvailableCopies > 0;
    }
}

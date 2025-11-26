using System;
using System.ComponentModel.DataAnnotations;

namespace BookLendingSystem.Application.DTOs
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class CreateBookDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 50 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Author must be between 1 and 50 characters")]
        public string Author { get; set; }

        [Required(ErrorMessage = "ISBN is required")]
        [RegularExpression(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$",
            ErrorMessage = "Invalid ISBN format")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Total copies is required")]
        [Range(1, 100, ErrorMessage = "Total copies must be between 1 and 100")]
        public int TotalCopies { get; set; }
    }

    public class UpdateBookDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 50 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Author must be between 1 and 50 characters")]
        public string Author { get; set; }

        [Required(ErrorMessage = "ISBN is required")]
        [RegularExpression(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$",
            ErrorMessage = "Invalid ISBN format")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Total copies is required")]
        [Range(1, 100, ErrorMessage = "Total copies must be between 1 and 100")]
        public int TotalCopies { get; set; }
    }
}

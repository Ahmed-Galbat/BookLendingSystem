using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Domain.Entities;

namespace BookLendingSystem.Application.MappingProfiles
{
    public class BookLendingProfile : Profile
    {
        public BookLendingProfile()
        {
            // Book Mappings
            CreateMap<Book, BookDto>();
            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();

            // Loan Mappings
            CreateMap<Loan, LoanDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.IsReturned, opt => opt.MapFrom(src => src.ReturnDate.HasValue))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue));
            
        }
    }
}

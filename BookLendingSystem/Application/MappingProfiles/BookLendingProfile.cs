using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Features.Books.Commands.CreateBook;
using BookLendingSystem.Application.Features.Books.Commands.UpdateBook;
using BookLendingSystem.Domain.Entities;

namespace BookLendingSystem.Application.MappingProfiles
{
    public class BookLendingProfile : Profile
    {
        public BookLendingProfile()
        {
            // Book mappings
            CreateMap<Book, BookDto>();
            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();
            CreateMap<CreateBookCommand, Book>();
            CreateMap<UpdateBookCommand, Book>();

            // Loan mappings
            CreateMap<Loan, LoanDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : string.Empty))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserId));
        }
    }
}

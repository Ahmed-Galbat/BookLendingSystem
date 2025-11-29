using AutoMapper;
using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Features.Books.Queries.GetAllBooks
{
    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookDto>>
    {
        private readonly IRepository<Book, Guid> _bookRepository;
        private readonly IMapper _mapper;

        public GetAllBooksQueryHandler(IRepository<Book, Guid> bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
    }
}

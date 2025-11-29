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

namespace BookLendingSystem.Application.Features.Books.Queries.GetBookById
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto>
    {
        private readonly IRepository<Book, Guid> _bookRepository;
        private readonly IMapper _mapper;

        public GetBookByIdQueryHandler(IRepository<Book, Guid> bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {request.Id} not found.");
            }
            return _mapper.Map<BookDto>(book);
        }
    }
}

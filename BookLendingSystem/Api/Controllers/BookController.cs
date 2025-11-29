using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Features.Books.Commands.CreateBook;
using BookLendingSystem.Application.Features.Books.Commands.DeleteBook;
using BookLendingSystem.Application.Features.Books.Commands.UpdateBook;
using BookLendingSystem.Application.Features.Books.Queries.GetAllBooks;
using BookLendingSystem.Application.Features.Books.Queries.GetBookById;
using BookLendingSystem.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLendingSystem.Api.Controllers
{
    [Authorize]
    public class BookController : BaseApiController
    {
        private readonly IMediator _mediator;

        public BookController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var query = new GetAllBooksQuery();
            var books = await _mediator.Send(query);
            return Ok(books);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BookDto>> GetById(Guid id)
        {
            try
            {
                var query = new GetBookByIdQuery { Id = id };
                var book = await _mediator.Send(query);
                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new CreateBookCommand
                {
                    Title = dto.Title,
                    Author = dto.Author,
                    ISBN = dto.ISBN,
                    TotalCopies = dto.TotalCopies
                };

                var book = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> Update(Guid id, [FromBody] UpdateBookDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new UpdateBookCommand
                {
                    Id = id,
                    Title = dto.Title,
                    Author = dto.Author,
                    ISBN = dto.ISBN,
                    TotalCopies = dto.TotalCopies
                };

                var book = await _mediator.Send(command);
                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var command = new DeleteBookCommand { Id = id };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

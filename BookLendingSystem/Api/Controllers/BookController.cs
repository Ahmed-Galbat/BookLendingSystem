using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Interfaces;
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
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BookDto>> GetById(Guid id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
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
            var book = await _bookService.AddBookAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> Update(Guid id, [FromBody] UpdateBookDto dto)
        {
            try
            {
                var book = await _bookService.UpdateBookAsync(id, dto);
                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _bookService.DeleteBookAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

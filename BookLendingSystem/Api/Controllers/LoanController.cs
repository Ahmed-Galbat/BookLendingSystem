using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Features.Loans.Commands.BorrowBook;
using BookLendingSystem.Application.Features.Loans.Commands.ReturnBook;
using BookLendingSystem.Application.Features.Loans.Queries.GetAllLoans;
using BookLendingSystem.Application.Features.Loans.Queries.GetUserLoans;
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
    public class LoanController : BaseApiController
    {
        private readonly IMediator _mediator;

        public LoanController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("borrow")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<LoanDto>> Borrow([FromBody] BorrowBookDto dto)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                var command = new BorrowBookCommand
                {
                    BookId = dto.BookId,
                    UserId = CurrentUserId
                };

                var loan = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetUserLoans), loan);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("return/{loanId}")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<LoanDto>> Return(Guid loanId)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                var command = new ReturnBookCommand
                {
                    LoanId = loanId,
                    UserId = CurrentUserId
                };

                var loan = await _mediator.Send(command);
                return Ok(loan);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-loans")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetUserLoans()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var query = new GetUserLoansQuery { UserId = CurrentUserId };
            var loans = await _mediator.Send(query);
            return Ok(loans);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetAllLoans()
        {
            var query = new GetAllLoansQuery();
            var loans = await _mediator.Send(query);
            return Ok(loans);
        }
    }
}

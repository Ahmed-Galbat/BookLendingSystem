using BookLendingSystem.Application.DTOs;
using BookLendingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLendingSystem.Api.Controllers
{
    [Authorize(Roles = "Member")]
    public class LoanController : BaseApiController
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpPost("borrow")]
        public async Task<ActionResult<LoanDto>> Borrow([FromBody] BorrowBookDto dto)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                var loan = await _loanService.BorrowBookAsync(dto.BookId, CurrentUserId);
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
        public async Task<ActionResult<LoanDto>> Return(Guid loanId)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                var loan = await _loanService.ReturnBookAsync(loanId, CurrentUserId);
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
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetUserLoans()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var loans = await _loanService.GetUserLoansAsync(CurrentUserId);
            return Ok(loans);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetAllLoans()
        {
            var loans = await _loanService.GetAllLoansAsync();
            return Ok(loans);
        }
    }
}

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

namespace BookLendingSystem.Application.Features.Loans.Queries.GetOverdueLoans
{
    public class GetOverdueLoansQueryHandler : IRequestHandler<GetOverdueLoansQuery, IEnumerable<LoanDto>>
    {
        private readonly IRepository<Loan, Guid> _loanRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly IMapper _mapper;

        public GetOverdueLoansQueryHandler(
            IRepository<Loan, Guid> loanRepository,
            IDateTimeService dateTimeService,
            IMapper mapper)
        {
            _loanRepository = loanRepository;
            _dateTimeService = dateTimeService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LoanDto>> Handle(GetOverdueLoansQuery request, CancellationToken cancellationToken)
        {
            var now = _dateTimeService.Now;
            var overdueLoans = await _loanRepository.FindAsync(l => l.ReturnDate == null && l.DueDate < now);
            return _mapper.Map<IEnumerable<LoanDto>>(overdueLoans);
        }
    }
}

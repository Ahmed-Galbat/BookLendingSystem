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

namespace BookLendingSystem.Application.Features.Loans.Queries.GetUserLoans
{
    public class GetUserLoansQueryHandler : IRequestHandler<GetUserLoansQuery, IEnumerable<LoanDto>>
    {
        private readonly IRepository<Loan, Guid> _loanRepository;
        private readonly IMapper _mapper;

        public GetUserLoansQueryHandler(IRepository<Loan, Guid> loanRepository, IMapper mapper)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LoanDto>> Handle(GetUserLoansQuery request, CancellationToken cancellationToken)
        {
            var loans = await _loanRepository.FindAsync(l => l.UserId == request.UserId);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }
    }
}

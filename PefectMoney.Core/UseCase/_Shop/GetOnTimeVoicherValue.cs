using MediatR;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase._Shop
{
    public record GetOnTimeVoicherValueRequest : IRequest<ResultOperation<VoicherValueDto>>
    {
    }
    public class GetOnTimeVoicherValueHandler : IRequestHandler<GetOnTimeVoicherValueRequest, ResultOperation<VoicherValueDto>>
    {
        public async Task<ResultOperation<VoicherValueDto>> Handle(GetOnTimeVoicherValueRequest request, CancellationToken cancellationToken)
        {
            return new VoicherValueDto
            {
                Dollars = 1000,
                Rials = 500000
            }.ToSuccessResult();
        }
    }
}

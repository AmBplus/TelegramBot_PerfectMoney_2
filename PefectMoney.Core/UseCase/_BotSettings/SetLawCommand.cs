using MediatR;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Settings;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase._BotSettings
{
    public record SetLawCommandRequest( string Law) : IRequest<ResultOperation>
    {
         
    }
    public class SetLawCommandHandler : IRequestHandler<SetLawCommandRequest, ResultOperation>
    {
        public SetLawCommandHandler(IWritableOptions<BotSettings> writableOptions,
            IMediator mediator 
            , ILogger<SetLawCommandHandler> logger )
        {
            WritableOptions = writableOptions;
            Mediator = mediator;
            Logger = logger;
        }

        public IWritableOptions<BotSettings> WritableOptions { get; }
        public IMediator Mediator { get; }
        public ILogger<SetLawCommandHandler> Logger { get; }

        public async Task<ResultOperation> Handle(SetLawCommandRequest request, CancellationToken cancellationToken)
        {
            WritableOptions.Update(x => x.RuleText = new List<string>() { request.Law });
           
            
            return ResultOperation.ToSuccessResult();            
           
        }
    }
}

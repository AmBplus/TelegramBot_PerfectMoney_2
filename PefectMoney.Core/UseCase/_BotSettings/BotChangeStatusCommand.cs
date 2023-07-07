using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PefectMoney.Core.Settings;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase._BotSettings
{
    public record BotChangeStatusCommandRequest(bool Status) : IRequest<ResultOperation>
    {
    }

    public class BotChangeStatusCommandHandler : IRequestHandler<BotChangeStatusCommandRequest, ResultOperation>
    {
        public BotChangeStatusCommandHandler(ILogger<BotChangeStatusCommandHandler> logger , IWritableOptions<BotSettings> writableOptions, IMediator mediator)
        {
            Logger = logger;
            WritableOptions = writableOptions;
           
            Mediator = mediator;
        }

        public ILogger<BotChangeStatusCommandHandler> Logger { get; }
        public IWritableOptions<BotSettings> WritableOptions { get; }
      
        public IMediator Mediator { get; }

        public async Task<ResultOperation> Handle(BotChangeStatusCommandRequest request, CancellationToken cancellationToken)
        {
            //  var botSettingsSection = Configuration.GetSection(BotSettings.Configuration);
            WritableOptions.Update(x => x.StopBot = request.Status);
          
            var botSettings = await Mediator.Send(new GetBotSettingsRequest());
            botSettings.StopBot = request.Status;
            return ResultOperation.ToSuccessResult();
        }
    }
}

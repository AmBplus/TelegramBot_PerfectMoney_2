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
    public record ChangeSellStatusRequest(bool Status) : IRequest<ResultOperation>
    {
    }
    public class ChangeSellStatusHandler : IRequestHandler<ChangeSellStatusRequest, ResultOperation>
    {

        public ILogger<BotChangeStatusCommandHandler> Logger { get; }

        public ChangeSellStatusHandler(ILogger<BotChangeStatusCommandHandler> logger, IWritableOptions<BotSettings> writableOptions, IMediator mediator)
        {
            Logger = logger;
            WritableOptions = writableOptions;
            Mediator = mediator;
        }

        public IWritableOptions<BotSettings> WritableOptions { get; }

        public IMediator Mediator { get; }
        public async Task<ResultOperation> Handle(ChangeSellStatusRequest request, CancellationToken cancellationToken)
        {
            //  var botSettingsSection = Configuration.GetSection(BotSettings.Configuration);
            WritableOptions.Update(x => x.StopSelling = request.Status);

            var botSettings = await Mediator.Send(new GetBotSettingsRequest());
            botSettings.StopSelling = request.Status;
            return ResultOperation.ToSuccessResult();
        }
    }
}

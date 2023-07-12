using MediatR;
using Microsoft.Extensions.Options;
using PefectMoney.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase._BotSettings
{
    public class GetBotSettingsRequest : IRequest<BotSettings>
    {
    }
    public class GetBotSettingsHandler : IRequestHandler<GetBotSettingsRequest, BotSettings>
    {
        public GetBotSettingsHandler(IOptionsSnapshot<BotSettings> options)
        {
            BotSettings = options.Value;
        }

        public BotSettings BotSettings { get; }

        public async Task<BotSettings> Handle(GetBotSettingsRequest request, CancellationToken cancellationToken)
        {
            return BotSettings;
        }
    }
}

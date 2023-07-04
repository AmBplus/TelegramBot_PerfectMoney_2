using MediatR;
using Microsoft.Extensions.Options;

using PerfectMonney_ConnectorToExternalService.Settings;

namespace PerfectMonney_ConnectorToExternalService.UseCase._BotSettings
{
    public class GetBotSettingsRequest : IRequest<BotSettings>
    {
    }
    public class GetBotSettingsHandler : IRequestHandler<GetBotSettingsRequest, BotSettings>
    {
        public GetBotSettingsHandler(IOptions<BotSettings> options)
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

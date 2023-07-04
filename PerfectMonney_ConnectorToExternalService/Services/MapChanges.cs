using Microsoft.Extensions.Options;
using PerfectMonney_ConnectorToExternalService.Settings;

namespace PerfectMonney_ConnectorToExternalService.Services
{
    public class MapChanges : IMapChanges
    {
        public MapChanges(IOptionsSnapshot<BotSettings> optionsSnapshot)
        {

        }
    }
    public interface IMapChanges
    {

    }
}

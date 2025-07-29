using IslandCaller.Services.NotificationProvidersNew;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared.Enums;
using Microsoft.Extensions.Hosting;

namespace IslandCaller.Services.IslandCallerHostService
{
    public class IslandCallerHostService : IHostedService
    {
        private ILessonsService LessonsService { get; }
        public IUriNavigationService UriNavigationService { get; }

        public IslandCallerHostService(Plugin plugin, IUriNavigationService uriNavigationService, ILessonsService lessonsService)
        {
            LessonsService = lessonsService;
            UriNavigationService = uriNavigationService;
            UriNavigationService.HandlePluginsNavigation(
                "IslandCaller/Run",
                args =>
                {
                    if (plugin.Settings.IsBreakProofEnabled & LessonsService.CurrentState == TimeState.Breaking) return;
                    new IslandCallerNotificationProviderNew().RandomCall(1);
                }
            );
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}

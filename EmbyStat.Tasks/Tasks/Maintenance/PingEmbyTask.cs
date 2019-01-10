using System;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Hubs;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Tasks.Tasks.Maintenance
{
    public class PingEmbyTask : TaskConnector, IPingEmbyTask
    {
        private readonly IEmbyStatusRepository _embyStatusRepository;
        private readonly IEmbyService _embyService;

        public PingEmbyTask(IHubHelper hubHelper, IEmbyStatusRepository embyStatusRepository, IEmbyService embyService) : base(hubHelper)
        {
            _embyStatusRepository = embyStatusRepository;
            _embyService = embyService;
        }

        public override string Name => "TASKS.PINGEMBYSERVERTITLE";
        public override string Key => "PingEmbyServer";
        public override string Description => "TASKS.PINGEMBYSERVERDESCRIPTION";
        public override string Category => "Emby";
        public override Guid Id => new Guid("ce1fbc9e-21ee-450b-9cdf-58a0e17ea98e");

        public async Task Execute()
        {
            //_progress.Report(0);
            var result = await _embyService.PingEmbyAsync(new CancellationToken(false));
            //_progress.Report(50);
            if (result == "Emby Server")
            {
                LogInformation(Constants.LogPrefix.PingEmbyTask, "We found your Emby server");
                _embyStatusRepository.ResetMissedPings();
            }
            else
            {
                LogInformation(Constants.LogPrefix.PingEmbyTask, "We could not ping your Emby server. Might be because it's turned off or dns is wrong");
                _embyStatusRepository.IncreaseMissedPings();
            }
            //_progress.Report(100);
        }

        public void Dispose()
        {
        }
    }
}
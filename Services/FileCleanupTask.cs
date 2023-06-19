using System.Threading;

using FileShare.Models;

namespace FileShare.Services
{
    public class FileCleanupTask : BackgroundService, IDisposable
    {
        private Timer _timer;
        private readonly DatabaseContext _context;
        private readonly Deleter _deleter;

        public FileCleanupTask(DatabaseContext context,
                                  Deleter deleter)
        {
            _context = context;
            _deleter = deleter;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(new TimerCallback(DeleteObsoleteData), null, 0, 10000);
            await Task.CompletedTask;
        }
        // public Task StartAsync(CancellationToken cancellationToken)
        // {
        //     _timer = new Timer(new TimerCallback(DeleteObsoleteData), null, 0, 10000);
        //     return Task.CompletedTask;
        // }

        public void DeleteObsoleteData(object? state)
        {
            var obsoleteData = _context.Files.Where(file => file.CreatedAt < file.DelTime);
            obsoleteData.Select(file => _deleter.DeleteFileAsync(file));
        }

        // public Task StopAsync(CancellationToken cancellationToken)
        // {
        //     _timer?.Change(Timeout.Infinite, 0);
        //     return Task.CompletedTask;
        // }

        // public void Dispose()
        // {
        //     _timer?.Dispose();
        // }
    }
}
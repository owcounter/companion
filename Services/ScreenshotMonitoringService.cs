using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Owcounter.Services
{
    public class ScreenshotMonitoringService : IDisposable
    {
        private readonly string folderToWatch = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Overwatch", "ScreenShots", "Overwatch");
        private FileSystemWatcher? watcher;
        private readonly ApiService apiService;
        private readonly SynchronizationContext synchronizationContext;

        public ScreenshotMonitoringService(ApiService apiService)
        {
            this.apiService = apiService;
            this.synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        public void StartMonitoring()
        {
            watcher = new FileSystemWatcher(folderToWatch)
            {
                NotifyFilter = NotifyFilters.FileName,
                Filter = "*.jpg",
                EnableRaisingEvents = true
            };

            watcher.Created += OnFileCreated;
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            synchronizationContext.Post(async _ =>
            {
                await Task.Delay(200); // Wait for the file to be fully written
                try
                {
                    using (var image = Image.FromFile(e.FullPath))
                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        var imageBytes = ms.ToArray();
                        var base64String = Convert.ToBase64String(imageBytes);

                        await apiService.SendScreenshotToServer(base64String);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error processing screenshot: {ex.Message}");
                }
            }, null);
        }

        public void Dispose()
        {
            watcher?.Dispose();
        }
    }
}
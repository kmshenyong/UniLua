using System;
using System.IO;
using System.Threading.Tasks;
using GrinderApp.Configuration.Helper;
using Serilog;

namespace GrinderApp.Configuration.StreamProvider
{
    /// <summary>
    /// 文件流提供者
    /// </summary>
    public class FileStreamProvider : IStreamProviderWithNotification
    {
        private readonly string _filePath;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">文件名，包含文件全路径</param>
        /// <param name="watchForChanged">监视文件更改</param>
        public FileStreamProvider(string filePath, bool watchForChanged = false)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (watchForChanged)
            {
                Task.Run(() => WatchForChanged(filePath));
            }

            _filePath = filePath;
        }

        /// <summary>
        /// 监视文件更改
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        private async Task WatchForChanged(string filePath)
        {
            var watcher    = new FileSystemWatcher(filePath);
            int errorCount = 0;

            while (true)
            {
                try
                {
                    watcher.WaitForChanged(WatcherChangeTypes.All);

                    await Task.Delay(1000);
                    StreamSourceChanged?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    errorCount += 1;
                    if (errorCount > 100)
                    {
                        Log.Error(ex, "Configuration file watching ERR, terminal.");
                        return;
                    }

                    Log.Error(ex, "Configuration file watching ERR, retry after 5 seconds.");
                    await Task.Delay(5000);
                }
            }
        }

        /// <summary>
        /// 流更新事件。
        /// 注意：OpenWrite 写入流不会激发该事件，该事件由程序外部激发调用
        /// </summary>
        public event AsyncEventHandler StreamSourceChanged;

        #region Implementation of IStreamProvider

        /// <summary>
        /// 以读取的方式打开流
        /// </summary>
        /// <returns></returns>
        public Stream OpenRead()
        {
            if (File.Exists(_filePath))
                return File.OpenRead(_filePath);

            // 文件不存在, 返回空流
            return new MemoryStream();
        }

        /// <summary>
        /// 为写入打开流
        /// </summary>
        /// <returns></returns>
        public Stream OpenWrite()
        {
            var fullPath = Path.GetFullPath(_filePath);
            var dir      = Path.GetDirectoryName(fullPath);

            if (string.IsNullOrEmpty(dir))
                throw new Exception($"Invalid file path: {_filePath}");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return File.Create(_filePath);
        }

        #endregion
    }
}

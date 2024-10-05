using Megacorp.AutoWebpConverter.Configuration;
using Microsoft.Extensions.Options;

namespace Megacorp.AutoWebpConverter.Services
{
    public class AutoWebpConverterService
    {
        private readonly IOptions<AutoWebpConverterConfiguration> _configuration;
        private readonly ILogger<AutoWebpConverterBackgroundService> _logger;
        private readonly FileSystemWatcher _watcher;
        private readonly int _maxTries;
        private readonly string _outputFormat;

        private readonly string[] _supportedInputFormats = { "webp" };
        private readonly string[] _supportedOutputFormats = { "png", "jpg" };

        public AutoWebpConverterService(ILogger<AutoWebpConverterBackgroundService> logger, IOptions<AutoWebpConverterConfiguration> configuration)
        {
            this._logger = logger;

            this.ValidateConfiguration(configuration);

            this._configuration = configuration;

            this._watcher = new FileSystemWatcher();
            this._watcher.Path = this._configuration.Value.MonitorPath;
            this._watcher.IncludeSubdirectories = this._configuration.Value.IncludeSubdirectories;
            this._watcher.Filter = "*.webp";
            this._watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.CreationTime | NotifyFilters.LastWrite;
            this._watcher.EnableRaisingEvents = false;

            this._maxTries = this._configuration.Value.MaximumTries;           

            this._outputFormat = this._configuration.Value.OutputFormat;
        }

        /// <summary>
        /// Basic validation of configuration options
        /// </summary>
        /// <param name="configuration"></param>
        /// <exception cref="DirectoryNotFoundException">Thrown if the specified folder does not exist</exception>
        /// <exception cref="ArgumentException">Thrown if the specified number of maximum tries is less than one</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified output format is not supported by the service</exception>
        private void ValidateConfiguration(IOptions<AutoWebpConverterConfiguration> configuration)
        {
            if (!Directory.Exists(configuration.Value.MonitorPath))
            {
                throw new DirectoryNotFoundException($"Invalid directory: {configuration.Value.MonitorPath}");
            }

            if (configuration.Value.MaximumTries < 1)
            {
                throw new ArgumentException($"Invalid try count: {configuration.Value.MaximumTries}");
            }

            if (!this._supportedOutputFormats.Contains(configuration.Value.OutputFormat))
            {
                throw new ArgumentOutOfRangeException($"Unsupported output format: {configuration.Value.OutputFormat}");
            }
        }

        /// <summary>
        /// Start the service
        /// </summary>
        public void Start()
        {
            this._watcher.EnableRaisingEvents = true;
            this._watcher.Created += OnFileCreated;
            _logger.LogInformation($"{nameof(AutoWebpConverterService)} has been started.");
        }


        /// <summary>
        /// Stop the service
        /// </summary>
        public void Stop()
        {
            this._watcher.EnableRaisingEvents = false;
            this._watcher.Created -= OnFileCreated;
            _logger.LogInformation($"{nameof(AutoWebpConverterService)} has been stopped.");
        }

        /// <summary>
        /// Main event handler, fires when a file is detected in the monitored directory
        /// </summary>
        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            string filePath = e.FullPath;

            string sourcePath = filePath;
            FileInfo fi = new FileInfo(sourcePath);

            if (!this._supportedInputFormats.Any(extension => fi.FullName.EndsWith($".{extension}")))
            {
                _logger.LogInformation($"Unsupported format, skipping file: '{fi.Name}'");
                return;
            }

            string destinationFileName = String.Join(string.Empty, fi.FullName.Split(fi.Extension).SkipLast(1));

            int currentTry = 1;

            while (currentTry <= this._maxTries)
            {
                try
                {
                    switch (_configuration.Value.OutputFormat.ToLower())
                    {
                        case "png":
                            this.WebpToPng(sourcePath, $"{destinationFileName}.png");
                            File.Delete(sourcePath);
                            break;
                        case "jpg":
                            this.WebpToJpg(sourcePath, $"{destinationFileName}.jpg");
                            File.Delete(sourcePath);
                            break;
                        default:
                            break;
                    }

                    _logger.LogInformation($"Successfully processed file: '{fi.Name}'");

                    return;
                }
                catch (Exception ex)
                {
                    if (currentTry == this._maxTries)
                    {
                        _logger.LogError($"Could not convert file '{e.FullPath}'. Exception message: {ex.Message}");
                        throw;
                    }

                    // wait a second before trying again
                    currentTry++;
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Converts a webp image to a PNG image
        /// </summary>
        /// <param name="source">Source file path</param>
        /// <param name="destination">Destination file path</param>
        private void WebpToPng(string source, string destination)
        {
            Moraba.Images.Webp.Convert.WebPToPng(source, destination, compress: false);
        }

        /// <summary>
        /// Converts a webp image to a JPG image
        /// </summary>
        /// <param name="source">Source file path</param>
        /// <param name="destination">Destination file path</param>
        private void WebpToJpg(string source, string destination)
        {
            Moraba.Images.Webp.Convert.WebPToJpeg(source, destination, compress: false);
        }
    }
}

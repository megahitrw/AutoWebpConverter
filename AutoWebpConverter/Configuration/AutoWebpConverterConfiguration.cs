using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megacorp.AutoWebpConverter.Configuration
{
    /// <summary>
    /// Configuration object for the service
    /// </summary>
    public class AutoWebpConverterConfiguration
    {
        /// <summary>
        /// The full path to the directory that is being monitored
        /// </summary>
        public string MonitorPath { get; set; }
        
        /// <summary>
        /// Flag to indicate that subdirectories should be monitored recursively
        /// </summary>
        public bool IncludeSubdirectories { get; set; }

        /// <summary>
        /// The target output format
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// The maximum number of tries to convert files before giving up
        /// </summary>
        public int MaximumTries { get; set; }
    }
}

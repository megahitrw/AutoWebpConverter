# AutoWebpConverter

AutoWebpConverter is a Windows service that monitors a specified folder and automatically converts every WEBP image files that is placed inside it to either JPG or PNG.

# Requirements

* Windows 10+
* .NET 8.0+
* libwebp DLLs (included in solution)

# Usage

1. Build and publish the project to any location (or download the release ZIP)
2. Create the service from command line: \
`sc create Megacorp.AutoWebpConverterService binPath="<path_to_executable>"`
3. Configure the service by editing the **AutoWebpConverter** section in `appsettings.json` file. \
Example:
```
"AutoWebpConverter": {
  "MonitorPath": "C:\\Download",
  "IncludeSubdirectories":  true,
  "OutputFormat": "png",
  "MaximumTries": 5
}
```

> Note: Make sure the service user has appropriate access rights to the specified directory.

4. Open **Services** and start the service

To remove the service, stop it, and delete it from command line: \
`sc delete Megacorp.AutoWebpConverterService`

# Notes

* JPG output files are saved as standard encoding and uncompressed
* PNG ouput files are saved as 8-bit uncompressed without alpha channel

# Contribution

If you'd like to add anything feel free to send a pull request.

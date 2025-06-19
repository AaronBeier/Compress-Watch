using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Compression;
using ZstdSharp;

namespace CompressWatch {

  internal sealed class FileWatcherCommand : Command<FileWatcherCommand.Settings> {

    public sealed class Settings : CommandSettings {

      [Description("Whether to watch folders recursively")]
      [CommandOption("-r|--recursive")]
      [DefaultValue(true)]
      public bool Recursive { get; set; }

      [Description("The file extension(s) to watch, separated by commas")]
      [CommandOption("-f|--filter")]
      [DefaultValue("*.*")]
      public string Filter { get; set; } = "*.*";

      [Description("The path to watch for changes. Defaults to the current directory")]
      [CommandArgument(2, "<Path>")]
      public string Path { get; set; } = ".";
    }

    private readonly ManualResetEvent ExitEvent = new(false);

    public override int Execute(CommandContext Context, Settings Settings) {
      IEnumerable<string> Filters = Settings.Filter.Contains(',', StringComparison.Ordinal)
        ? Settings.Filter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        : [Settings.Filter.Trim()];

      using FileSystemWatcher Watcher = new(Settings.Path) {
        IncludeSubdirectories = Settings.Recursive,
        NotifyFilter = NotifyFilters.LastWrite
      };

      Watcher.Filters.Clear();
      foreach (string Filter in Filters) {
        Watcher.Filters.Add(Filter);
      }

      Watcher.Error += (_, Error) => {
        AnsiConsole.WriteException(Error.GetException());
        this.ExitEvent.Set();
      };

      Watcher.Created += this.OnFileChange;
      Watcher.Changed += this.OnFileChange;

      Watcher.EnableRaisingEvents = true;
      this.ExitEvent.WaitOne();

      return 0;
    }

    private void OnFileChange(object _, FileSystemEventArgs Event) {
      switch (Path.GetExtension(Event.FullPath).ToLower()) {
        case ".gz":
        case ".zst":
        case ".br":
          return; // Ignore already compressed files
      }

      using (FileStream OriginalFile = File.Open(Event.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
        #region GZip Compression
        string GzPath = $"{Event.FullPath}.gz";
        using (FileStream GzFile = File.Create(GzPath)) {
          using GZipStream GzStream = new(GzFile, CompressionLevel.SmallestSize);

          OriginalFile.CopyTo(GzStream);
          OriginalFile.Seek(0, SeekOrigin.Begin);
        }
        #endregion

        #region ZStd Compression
        string ZstPath = $"{Event.FullPath}.zst";
        using (FileStream ZstFile = File.Create(ZstPath)) {
          using CompressionStream ZstStream = new(ZstFile, Compressor.MaxCompressionLevel);

          OriginalFile.CopyTo(ZstStream);
          OriginalFile.Seek(0, SeekOrigin.Begin);
        }
        #endregion

        #region Brotli Compression
        string BrPath = $"{Event.FullPath}.br";
        using (FileStream BrFile = File.Create(BrPath)) {
          using BrotliStream BrStream = new(BrFile, CompressionLevel.SmallestSize);

          OriginalFile.CopyTo(BrStream);
        }
        #endregion
      }
    }
  }
}

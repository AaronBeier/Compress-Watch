using Spectre.Console.Cli;

namespace CompressWatch {

  internal sealed class Program {

    static int Main(string[] Arguments) {
      CommandApp<FileWatcherCommand> Application = new();
      return Application.Run(Arguments);
    }
  }
}

using Spectre.Console;

namespace CompressWatch {

  internal sealed class BinarySpinner : Spinner {

    // Spectre.Console's Docs show this one off, but they dont actually include it.
    // Animation frames "stolen" from https://rawgit.com/sindresorhus/cli-spinners/master/spinners.json

    public override TimeSpan Interval => TimeSpan.FromMilliseconds(80);
    public override bool IsUnicode => false;
    public override IReadOnlyList<string> Frames => [
      "010010",
      "001100",
      "100101",
      "111010",
      "111101",
      "010111",
      "101011",
      "111000",
      "110011",
      "110101"
    ];
  }
}

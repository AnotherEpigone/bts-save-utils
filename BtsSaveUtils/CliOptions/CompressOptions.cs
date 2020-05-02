using CommandLine;

namespace BtsSaveUtils.CliOptions
{
    [Verb("compress", HelpText = "Compresses the given file")]
    public class CompressOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input file path")]
        public string InputFile { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file path")]
        public string OutputFile { get; set; }
    }
}

using CommandLine;

namespace BtsSaveUtils.CliOptions
{
    [Verb("dump", HelpText = "Dumps the content of the zlib compressed section")]
    public class DumpOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input file path")]
        public string InputFile { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file path")]
        public string OutputFile { get; set; }
    }
}

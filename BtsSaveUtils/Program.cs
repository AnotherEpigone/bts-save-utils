using BtsSaveUtils.CliOptions;
using CommandLine;
using System;
using System.Collections.Generic;

namespace BtsSaveUtils
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: btssaveutils [input_path] [output_path]");
                return;
            }

            Parser.Default.ParseArguments<DumpOptions, CompressOptions>(args)
                .WithParsed<DumpOptions>(RunWithDumpOptions)
                .WithParsed<CompressOptions>(RunWithCompressOptions)
                .WithNotParsed(RunWithFailedParse);
        }

        static void RunWithDumpOptions(DumpOptions options) => CompressedSection.Decompress(options.InputFile, options.OutputFile);

        static void RunWithCompressOptions(CompressOptions options) => CompressedSection.Compress(options.InputFile, options.OutputFile);

        static void RunWithFailedParse(IEnumerable<Error> errors)
        {
        }
    }
}

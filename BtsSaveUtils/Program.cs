using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;
using System.Text;

namespace BtsSaveUtils
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("usage: btssaveutils [input_path] [output_path]");
                return;
            }

            var inPath = args[0];
            string inFileString;
            using (var inFile = File.OpenRead(inPath))
            using (var reader = new StreamReader(inFile))
            {
                if (inFile.Length > 1024 * 1024 * 1024)
                {
                    Console.WriteLine("Failed: That's way too big to be a BTS save.");
                    return;
                }

                inFileString = reader.ReadToEnd();

                var zlibHeader = Encoding.UTF8.GetString(new byte[] { 0x78, 0x9C });
                var deflateIndex = inFileString.IndexOf(zlibHeader);
                if (deflateIndex < 0)
                {
                    Console.WriteLine("Failed: No compressed data found.");
                    return;
                }

                inFile.Seek(deflateIndex, SeekOrigin.Begin);

                var outPath = args[1];
                using (var outFile = File.Create(outPath))
                using (var inflateStream = new InflaterInputStream(inFile))
                {
                    try
                    {
                        inflateStream.CopyTo(outFile);
                    }
                    catch (SharpZipBaseException)
                    {
                        // it's going to throw once it gets to the end of the compressed data
                    }
                }
            }
        }
    }
}

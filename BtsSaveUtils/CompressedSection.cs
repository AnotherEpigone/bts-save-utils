using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BtsSaveUtils
{
    public class CompressedSection
    {
        public static void Decompress(string inPath, string outPath)
        {
            string inFileString;
            using (var inFile = File.OpenRead(inPath))
            using (var reader = new StreamReader(inFile))
            {
                Console.WriteLine("Starting decompress.");
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

                var zlibFooter = Encoding.UTF8.GetString(new byte[] { 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00 });
                var deflateEndIndex = inFileString.LastIndexOf(zlibFooter);
                if (deflateEndIndex < 0)
                {
                    Console.WriteLine("Failed: No footer found.");
                    return;
                }

                // FIXUP TIME
                deflateEndIndex += 4;

                var compressedData = new byte[deflateEndIndex - deflateIndex];

                inFile.Seek(deflateIndex, SeekOrigin.Begin);
                inFile.Read(compressedData, 0, compressedData.Length);

                using (var outStream = new MemoryStream())
                using (var compressedDataStream = new MemoryStream(compressedData))
                using (var inflateStream = new InflaterInputStream(compressedDataStream))
                {
                    try
                    {
                        inflateStream.CopyTo(outStream);
                    }
                    catch (SharpZipBaseException ex)
                    {
                        Console.WriteLine("Errors while decompressing: ", ex);
                    }

                    outStream.Seek(0, SeekOrigin.Begin);
                    using (var outFile = File.Create(outPath))
                    {
                        outStream.CopyTo(outFile);
                    }

                    Console.WriteLine($"Decompressed {outStream.Length} bytes to {Path.GetFileName(outPath)}");

                    outStream.Seek(0, SeekOrigin.Begin);
                    using (var md5 = MD5.Create())
                    {
                        var hash = md5.ComputeHash(outStream);
                        var hashStringBuilder = new StringBuilder();

                        foreach (var hashByte in hash)
                        {
                            hashStringBuilder.Append(hashByte.ToString("x2"));
                        }

                        Console.WriteLine($"MD5 hash: {hashStringBuilder.ToString()}");
                    }
                }
            }

            Console.WriteLine("Decompress finished.");
        }

        public static void Compress(string inPath, string outPath)
        {
            Console.WriteLine("Compress started.");

            using (var inFile = File.OpenRead(inPath))
            using (var outFile = File.Create(outPath))
            using (var compressor = new DeflaterOutputStream(outFile))
            {
                inFile.CopyTo(compressor);
            }

            Console.WriteLine("Decompress finished.");
        }
    }
}

using System;
using System.IO;
using Ionic.Zip;

namespace CryptAByte.Domain.Utilities
{
    public static class FileUtilities
    {
        /// <summary>
        /// Compresses a file using ZIP and returns the Base64-encoded result.
        /// </summary>
        /// <param name="fileName">The name to use for the file inside the ZIP archive.</param>
        /// <param name="fileData">The raw file data to compress.</param>
        /// <returns>Base64-encoded string of the zipped file.</returns>
        public static string CompressAndEncodeFile(string fileName, byte[] fileData)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));

            if (fileData == null || fileData.Length == 0)
                throw new ArgumentException("File data cannot be empty.", nameof(fileData));

            using (var compressedStream = new MemoryStream(1024))
            {
                using (var zip = new ZipFile())
                {
                    zip.AddEntry(fileName, fileData);
                    zip.Save(compressedStream);
                }

                byte[] zippedFile = ReadStreamFully(compressedStream);
                return Convert.ToBase64String(zippedFile);
            }
        }

        /// <summary>
        /// Reads all bytes from a stream, resetting position to the beginning first.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>Array of all bytes in the stream.</returns>
        public static byte[] ReadStreamFully(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            const int bufferSize = 16 * 1024; // 16 KB buffer
            byte[] buffer = new byte[bufferSize];

            using (var memoryStream = new MemoryStream())
            {
                stream.Position = 0;
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Decompresses and extracts the first file from a Base64-encoded ZIP archive.
        /// </summary>
        /// <param name="base64ZippedData">Base64-encoded ZIP file data.</param>
        /// <param name="fileName">Output parameter for the extracted file name.</param>
        /// <returns>The extracted file data as a byte array.</returns>
        public static byte[] DecodeAndDecompressFile(string base64ZippedData, out string fileName)
        {
            if (string.IsNullOrWhiteSpace(base64ZippedData))
                throw new ArgumentException("Zipped data cannot be empty.", nameof(base64ZippedData));

            byte[] zippedData = Convert.FromBase64String(base64ZippedData);

            using (var zippedStream = new MemoryStream(zippedData))
            using (var zip = ZipFile.Read(zippedStream))
            {
                if (zip.Count == 0)
                    throw new InvalidOperationException("ZIP archive is empty.");

                var firstEntry = zip[0];
                fileName = firstEntry.FileName;

                using (var extractedStream = new MemoryStream())
                {
                    firstEntry.Extract(extractedStream);
                    return ReadStreamFully(extractedStream);
                }
            }
        }
    }
}

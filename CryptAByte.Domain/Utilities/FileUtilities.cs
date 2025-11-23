using System;
using System.IO;
using CryptAByte.Domain.Functional;
using Ionic.Zip;

namespace CryptAByte.Domain.Utilities
{
    /// <summary>
    /// Immutable result of file decompression.
    /// </summary>
    public sealed class DecompressedFile
    {
        public string FileName { get; }
        public byte[] Data { get; }

        public DecompressedFile(string fileName, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));
            if (data == null || data.Length == 0)
                throw new ArgumentException("File data cannot be empty.", nameof(data));

            FileName = fileName;
            Data = data;
        }
    }

    /// <summary>
    /// Pure functional utilities for file compression and decompression.
    /// All functions are stateless with explicit inputs and outputs.
    /// </summary>
    public static class FileUtilities
    {
        /// <summary>
        /// Compresses a file using ZIP and returns the Base64-encoded result.
        /// Pure function: same inputs always produce same outputs.
        /// </summary>
        /// <param name="fileName">The name to use for the file inside the ZIP archive.</param>
        /// <param name="fileData">The raw file data to compress.</param>
        /// <returns>Result containing Base64-encoded string of the zipped file, or error message.</returns>
        public static Result<string, string> CompressAndEncodeFile(string fileName, byte[] fileData)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return Result.Failure<string, string>("File name cannot be empty.");

            if (fileData == null || fileData.Length == 0)
                return Result.Failure<string, string>("File data cannot be empty.");

            try
            {
                using (var compressedStream = new MemoryStream(1024))
                {
                    using (var zip = new ZipFile())
                    {
                        zip.AddEntry(fileName, fileData);
                        zip.Save(compressedStream);
                    }

                    var zippedFileResult = ReadStreamFully(compressedStream);
                    return zippedFileResult.Map(zippedFile => Convert.ToBase64String(zippedFile));
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<string, string>($"File compression failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Reads all bytes from a stream, resetting position to the beginning first.
        /// Pure function for a given stream state.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>Result containing array of all bytes in the stream, or error message.</returns>
        public static Result<byte[], string> ReadStreamFully(Stream stream)
        {
            if (stream == null)
                return Result.Failure<byte[], string>("Stream cannot be null.");

            try
            {
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
                    return Result.Success<byte[], string>(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<byte[], string>($"Stream reading failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Decompresses and extracts the first file from a Base64-encoded ZIP archive.
        /// Pure function: returns immutable result object instead of using output parameters.
        /// </summary>
        /// <param name="base64ZippedData">Base64-encoded ZIP file data.</param>
        /// <returns>Result containing decompressed file information, or error message.</returns>
        public static Result<DecompressedFile, string> DecodeAndDecompressFile(string base64ZippedData)
        {
            if (string.IsNullOrWhiteSpace(base64ZippedData))
                return Result.Failure<DecompressedFile, string>("Zipped data cannot be empty.");

            try
            {
                byte[] zippedData = Convert.FromBase64String(base64ZippedData);

                using (var zippedStream = new MemoryStream(zippedData))
                using (var zip = ZipFile.Read(zippedStream))
                {
                    if (zip.Count == 0)
                        return Result.Failure<DecompressedFile, string>("ZIP archive is empty.");

                    var firstEntry = zip[0];
                    var fileName = firstEntry.FileName;

                    using (var extractedStream = new MemoryStream())
                    {
                        firstEntry.Extract(extractedStream);
                        var dataResult = ReadStreamFully(extractedStream);

                        return dataResult.Map(data => new DecompressedFile(fileName, data));
                    }
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<DecompressedFile, string>($"File decompression failed: {ex.Message}");
            }
        }

        #region Legacy Compatibility Methods

        /// <summary>
        /// Legacy method - preserved for backward compatibility.
        /// New code should use CompressAndEncodeFile returning Result type.
        /// </summary>
        [Obsolete("Use CompressAndEncodeFile returning Result<string, string> for explicit error handling")]
        public static string CompressAndEncodeFileLegacy(string fileName, byte[] fileData)
        {
            var result = CompressAndEncodeFile(fileName, fileData);
            return result.Match(
                onSuccess: encoded => encoded,
                onFailure: error => throw new InvalidOperationException(error)
            );
        }

        /// <summary>
        /// Legacy method with output parameter - preserved for backward compatibility.
        /// New code should use DecodeAndDecompressFile returning Result type.
        /// </summary>
        [Obsolete("Use DecodeAndDecompressFile returning Result<DecompressedFile, string> instead")]
        public static byte[] DecodeAndDecompressFile(string base64ZippedData, out string fileName)
        {
            var result = DecodeAndDecompressFile(base64ZippedData);
            string tempFileName = string.Empty;
            byte[] fileData = null;

            result.Match(
                onSuccess: file =>
                {
                    tempFileName = file.FileName;
                    fileData = file.Data;
                    return file.Data;
                },
                onFailure: error =>
                {
                    tempFileName = string.Empty;
                    throw new InvalidOperationException(error);
                }
            );

            fileName = tempFileName;
            return fileData;
        }

        /// <summary>
        /// Legacy method - preserved for backward compatibility.
        /// New code should use ReadStreamFully returning Result type.
        /// </summary>
        [Obsolete("Use ReadStreamFully returning Result<byte[], string> for explicit error handling")]
        public static byte[] ReadStreamFullyLegacy(Stream stream)
        {
            var result = ReadStreamFully(stream);
            return result.Match(
                onSuccess: bytes => bytes,
                onFailure: error => throw new InvalidOperationException(error)
            );
        }

        #endregion
    }
}

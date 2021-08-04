using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Utility.Log.Infrastructure
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/42430559/progress-bar-not-available-for-zipfile-how-to-give-feedback-when-program-seems"></a>
    /// </summary>
    public static class ZipHelper
    {
        public static IEnumerable<(bool success, FileInfo fileInfo, Exception exception)> ArchiveToFile(
            FileInfo[] sourceFiles,
            string destinationArchiveFileName,
            IProgress<double> progress)
        {
            double totalBytes = sourceFiles.Sum(f => f.Length);
            long currentBytes = 0;

            using (ZipArchive archive = ZipFile.Open(destinationArchiveFileName, ZipArchiveMode.Create))
            {
                foreach (FileInfo file in sourceFiles)
                {
                    _ = CurrentBytes(file, archive, out Exception exception);
                    yield return (exception == null, file, exception);
                }
            }

            long CurrentBytes(FileInfo file, ZipArchive archive, out Exception exception)
            {
                try
                {
                    // NOTE: naive method to get sub-path from file name, relative to
                    // input directory. Production code should be more robust than this.
                    // Either use Path class or similar to parse directory separators and
                    // reconstruct output file name, or change this entire method to be
                    // recursive so that it can follow the sub-directories and include them
                    // in the entry name as they are processed.
                    string entryName = file.FullName.Substring(file.DirectoryName.Length + 1);
                    ZipArchiveEntry entry = archive.CreateEntry(entryName);

                    entry.LastWriteTime = file.LastWriteTime;

                    using (Stream inputStream = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (Stream outputStream = entry.Open())
                    {
                        Stream progressStream = new StreamWithProgress(inputStream,
                            new BasicProgress<int>(i =>
                            {
                                currentBytes += i;
                                progress.Report(currentBytes / totalBytes);
                            }), null);

                        progressStream.CopyTo(outputStream);
                    }

                    exception = null;
                }
                catch (Exception e)
                {
                    exception = e;
                    currentBytes += file.Length;
                    progress.Report(currentBytes / totalBytes);
                }

                return currentBytes;
            }
        }

        public static void ExtractFromDirectory(string sourceArchiveFileName, string destinationDirectoryName, IProgress<double> progress)
        {
            using (ZipArchive archive = ZipFile.OpenRead(sourceArchiveFileName))
            {
                double totalBytes = archive.Entries.Sum(e => e.Length);
                long currentBytes = 0;

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string fileName = Path.Combine(destinationDirectoryName, entry.FullName);

                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    using (Stream inputStream = entry.Open())
                    using (Stream outputStream = File.OpenWrite(fileName))
                    {
                        Stream progressStream = new StreamWithProgress(outputStream, null,
                            new BasicProgress<int>(i =>
                            {
                                currentBytes += i;
                                progress.Report(currentBytes / totalBytes);
                            }));

                        inputStream.CopyTo(progressStream);
                    }

                    File.SetLastWriteTime(fileName, entry.LastWriteTime.LocalDateTime);
                }
            }
        }
    }

    internal class StreamWithProgress : Stream
    {
        // NOTE: for illustration purposes. For production code, one would want to
        // override *all* of the virtual methods, delegating to the base _stream object,
        // to ensure performance optimizations in the base _stream object aren't
        // bypassed.

        private readonly Stream stream;
        private readonly IProgress<int> readProgress;
        private readonly IProgress<int> writeProgress;

        public StreamWithProgress(Stream stream, IProgress<int> readProgress, IProgress<int> writeProgress)
        {
            this.stream = stream;
            this.readProgress = readProgress;
            this.writeProgress = writeProgress;
        }

        public override bool CanRead => stream.CanRead;
        public override bool CanSeek => stream.CanSeek;
        public override bool CanWrite => stream.CanWrite;
        public override long Length => stream.Length;

        public override long Position
        {
            get => stream.Position;
            set => stream.Position = value;
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = stream.Read(buffer, offset, count);

            readProgress?.Report(bytesRead);
            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
            writeProgress?.Report(count);
        }
    }

    internal class BasicProgress<T> : IProgress<T>
    {
        private readonly Action<T> handler;

        public BasicProgress(Action<T> handler)
        {
            this.handler = handler;
        }

        void IProgress<T>.Report(T value)
        {
            handler(value);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CopyMostRecent
{
    /// <summary>
    /// State of the Copy action
    /// </summary>
    public enum CopyState
    {
        NotStarted,
        Started,
        Complete,
        Cancelled
    }

    /// <summary>
    /// Object tracking the progress of the Copy action
    /// </summary>
    public class DirectoryCopyProgress
    {
        public CopyState CopyState { get; set; } = CopyState.NotStarted;
        public DirectoryCompareResultsFlag Flag { get; set; } 
        public string CurrentFileName { get; set; } = "";
        public int CopyCount { get; set; } = 0;
        public long CopySize { get; set; } = 0;

        public override string ToString()
        {
            return string.Format("Flag: {0}, CopyState={1}, CurrentFileName={2}, CopyCount={3:n0}, CopySize={4:n0}", Flag, CopyState, CurrentFileName, CopyCount, CopySize);
        }
    }

    /// <summary>
    /// Object per Error encountered during the Copy action
    /// </summary>
    public class Errors
    {
        public DirectoryCompareResultsFlag Flag { get; set; }
        public string FileName { get; set; }
        public string ErrorMessage { get; set; }
        public string InnerExceptionMessage { get; set; }
        public Errors() { }
        public Errors(DirectoryCompareResultsFlag flag, string fileName, string errorMessage, string innerExceptionMessage)
        {
            Flag = flag;
            FileName = fileName; 
            ErrorMessage = errorMessage;
            InnerExceptionMessage = innerExceptionMessage;
        }
    }

    /// <summary>
    /// Object summarising the final results of the last Copy action
    /// </summary>
    public class DirectoryCopyResults
    {
        public CopyState CopyState { get; set; } = CopyState.NotStarted;
        public int TotalCopyCount = 0;
        public long TotalCopySize = 0;
        public List<Errors> Errors = new List<Errors>();
    }

    /// <summary>
    /// Class for handling the Copy operation on the results of a Compare action
    /// </summary>
    public class DirectoryCopy
    {
        private DirectoryCopyProgress progress;
        private DirectoryCompareResults compare;

        public int ProgressRefreshMilliseconds { get; set; } = 200;

        public DirectoryCopy() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryCopy"/> class.
        /// This constructor supplies the results of a Compare action.
        /// </summary>
        /// <param name="compare">The compare.</param>
        public DirectoryCopy(DirectoryCompareResults compare)
        {
            this.compare = compare;
            this.progress = new DirectoryCopyProgress();
        }

        /// <summary>
        /// Performs the Copy action asynchronously.
        /// This class must use the constructor to supply the compare results.
        /// Only selections that have been selected for inclusion in the copy operation are processed.
        /// </summary>
        /// <param name="progressReporter">The progress reporter. This is updated every 200ms, rather than after a set number of bytes/files.</param>
        /// <param name="token">A cancellation token. If a cancellation is invoked, the copy will terminate at a suitable point, rather than throw a <see cref="OperationCanceledException">.</param>
        /// <returns>A promise of a <see cref="DirectoryCopyResults"/> object.</returns>
        /// <exception cref="Exception">DirectoryCompareResults has not been supplied - use the constructor to initialise it.</exception>
        public async Task<DirectoryCopyResults> CopyAsync(IProgress<DirectoryCopyProgress> progressReporter, CancellationToken token)
        {
            var results = new DirectoryCopyResults();

            int taskIX = 0;
            Task[] copyTasks = new Task[4]; //  Do 4 copy-operations simultaneously

            if (this.compare == null)
            {
                throw new Exception("DirectoryCompareResults has not been supplied - use the constructor to initialise it.");
            }

            Timer timer = new Timer((x) => { 
                progressReporter.Report(progress);
            }, null, 0, ProgressRefreshMilliseconds);

            progress.CopyState = CopyState.Started;

            foreach (DirectoryCompareSelection selection in compare.Selections)
            {
                progress.Flag = selection.Flag;
                foreach (DirectoryCompareResultsEntry entry in compare.Entries)
                {

                    if (entry.Flag == selection.Flag 
                        && selection.CopySelected)
                    {
                        if (token.IsCancellationRequested)
                            break;
                        progress.CurrentFileName = entry.FileName;
                        progress.CopyCount++;
                        results.TotalCopyCount++;
                        try
                        {
                            FileInfo fromFile = new FileInfo(compare.SourceRootDir + entry.FileName);
                            FileInfo toFile = new FileInfo(compare.DestinationRootDir + entry.FileName);
                            copyTasks[taskIX] = CopyTo(fromFile, toFile, 
                                (bytes) => 
                                { 
                                    progress.CopySize += bytes; 
                                }, token);
                            taskIX++;
                            if (taskIX >= copyTasks.Length)
                            {
                                await Task.WhenAll(copyTasks);
                                copyTasks.Initialize();
                                taskIX = 0;
                            }
                        }
                        catch (OperationCanceledException ocex)
                        {
                            results.Errors.Add(new Errors(entry.Flag, entry.FileName, ocex.Message, "File has been left in a partial state."));
                            progress.CopyState = CopyState.Cancelled;
                            break;
                        }
                        catch (Exception ex)
                        {
                            results.Errors.Add(new Errors(entry.Flag, entry.FileName, ex.Message, ex.InnerException?.Message));
                        }
                    }
                    if (token.IsCancellationRequested)
                        break;
                }
            }

            timer.Change(Timeout.Infinite, Timeout.Infinite);
            timer.Dispose();
            if (progress.CopyState < CopyState.Cancelled)
                progress.CopyState = CopyState.Complete;
            progress.CurrentFileName = string.Empty;
            progressReporter.Report(progress);

            results.CopyState = progress.CopyState;
            results.TotalCopyCount = progress.CopyCount;
            results.TotalCopySize = progress.CopySize;

            return results;
        }

        /// <summary>
        /// This performs the copy operation on a single file.
        /// If a cancellation is invoked, the process will throw <see cref="OperationCanceledException"> if there are more than 256 1MB buffers left of the file yet to copy,
        /// else it will finish the file without throwing the exception. 
        /// Therefore, the exception would indicate that the destination file has been left in a partially-copied state.
        /// In a partial state, the destination file will be timestamped with current date/time, as the process copies the source timestamp only when completely copied.
        /// This will show up in a future scan as 'Source is older than Destination'.
        /// If the destination directory does not exist it will be created. This could throw an exception if there's a problem creating the directory.
        /// 
        /// To maximise speed, the copy process will use two 1MB buffers which will be flipped so that as it's reading a second buffer it will write the previous buffer asynchronously.
        /// </summary>
        /// <param name="file">The file to Copy.</param>
        /// <param name="destination">The destination file.</param>
        /// <param name="notifyBytes">The number of bytes copied. Updated after every buffer.</param>
        /// <param name="token">A cancellation token.</param>
        public async static Task CopyTo(FileInfo file, FileInfo destination, Action<int> notifyBytes, CancellationToken token)
        {
            const int bufferSize = 1024 * 1024;  
            byte[] bufferA = new byte[bufferSize], bufferB = new byte[bufferSize];
            bool A = true;
            int readCount = 0;
            Task writer = null;

            using (FileStream srce = file.OpenRead())
            {
                if (!Directory.Exists(Path.GetDirectoryName(destination.FullName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destination.FullName));
                }
                using (var dest = destination.OpenWrite())
                {
                    dest.SetLength(srce.Length);
                    for (long size = 0; size < file.Length; size += readCount)
                    {
                        if (token.IsCancellationRequested)
                        {
                            if ((file.Length - size) > (bufferSize * 256))   // More than 256MB to copy, stop copying immediately, else finish copying this file
                            {
                                dest.Close();
                                srce.Close();
                                token.ThrowIfCancellationRequested();
                            }
                        }
                        readCount = srce.Read(A ? bufferA : bufferB, 0, bufferSize);
                        writer?.Wait(); 
                        writer = dest.WriteAsync(A ? bufferA : bufferB, 0, readCount);
                        notifyBytes(readCount);
                        A = !A;
                    }
                    writer?.Wait(); 
                    dest.Close();
                    srce.Close();
                }
            }
            destination.LastWriteTime = file.LastWriteTime;
        }
    }
}

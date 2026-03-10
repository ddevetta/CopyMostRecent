using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CopyMostRecent
{
    /// <summary>
    /// Represents the results of a compare of two <seealso cref="DirectoryScanResults"/>.
    /// </summary>
    public class DirectoryCompareResults
    {
        public string SourceRootDir { get; set; }
        public int SourceFileCount { get; set; } = 0;
        public int SourceErrorCount { get; set; } = 0;
        public string DestinationRootDir { get; set; }
        public int DestinationFileCount { get; set; } = 0;
        public int DestinationErrorCount { get; set; } = 0;
        /// <summary>
        /// Gets or sets the selections.
        /// This is initialised to a fixed array of <seealso cref="DirectoryCompareSelection"/>
        /// , one for each possible value of <seealso cref="DirectoryCompareResultsFlag"/>.
        /// </summary>
        /// <value>
        /// The possible selections.
        /// </value>
        public DirectoryCompareSelection[] Selections { get; set; }

        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        /// <value>
        /// The list of entries.
        /// </value>
        public List<DirectoryCompareResultsEntry> Entries { get; set; }

        public DirectoryCompareResults() 
        { 
            this.Selections = new DirectoryCompareSelection[]
            {
                new DirectoryCompareSelection(DirectoryCompareResultsFlag.Match),
                new DirectoryCompareSelection(DirectoryCompareResultsFlag.Newer),
                new DirectoryCompareSelection(DirectoryCompareResultsFlag.NewFile),
                new DirectoryCompareSelection(DirectoryCompareResultsFlag.Older),
                new DirectoryCompareSelection(DirectoryCompareResultsFlag.Error)
            };
            this.Entries = new List<DirectoryCompareResultsEntry>();
        }

        public override string ToString()
        {
            return String.Format("SourceRootDir: {0}, SourceFileCount: {1}, DestinationRootDir: {2}, DestinationFileCount: {3}",
                this.SourceRootDir, this.SourceFileCount, this.DestinationRootDir, this.DestinationFileCount);
        }
    }

    /// <summary>
    /// The selection types of the compare results.
    /// </summary>
    public enum DirectoryCompareResultsFlag
    {
        /// <summary>
        /// The UTC Modified dates match, within the allowed time window held in <see cref="UserOptions.TimeWindowMilliseconds"/>
        /// </summary>
        Match,

        /// <summary>
        /// The Source file is Newer than the destination, i.e. it has been modified since the last copy.
        /// Should be selected for inclusion in the copy set.
        /// </summary>
        Newer,

        /// <summary>
        /// The file exists on the source and does not exist in the destinaion location.
        /// Should be selected for inclusion in the copy set.
        /// </summary>
        NewFile,

        /// <summary>
        /// The Source file is Older than the destination file. This may indicate a file has been modified on the destination.
        /// This may also be the result of an aborted Copy operation, which will leave the destination file in a partial state, and with the current date/time stamp.
        /// </summary>
        Older,

        /// <summary>
        /// Either the Source or Destination file was in an Error state. No compare was performed.
        /// </summary>
        Error
    }

    /// <summary>
    /// The entry status of the compare action. If there was a scan error on either the source or destination, this will be 
    /// reflected here and the Compare selection would be set to Error, otherwise the compare is made and the results are captured.
    /// </summary>
    public enum DirectoryCompareResultsEntryStatus
    {
        Success,
        ErrorOnSource,
        ErrorOnDestination
    }

    /// <summary>
    /// A distinct entry of the compare action.
    /// </summary>
    public class DirectoryCompareResultsEntry
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DirectoryCompareResultsFlag Flag { get; set; }
        public DateTime SourceModified { get; set; }
        public DateTime DestinationModified { get; set; }
        public DirectoryCompareResultsEntryStatus Status { get; set; }
        public string ErrorMessage { get; set; }

        public DirectoryCompareResultsEntry() { }
        public DirectoryCompareResultsEntry(string fileName, long fileSize, DirectoryCompareResultsFlag flag, DateTime sourceModified, DateTime destinationModified) 
        { 
            this.FileName = fileName;
            this.FileSize = fileSize;
            this.Flag = flag;
            this.SourceModified = sourceModified;
            this.DestinationModified = destinationModified;
        }

        public override string ToString()
        {
            return String.Format("FileName: {0}, Flag: {1}, SourceModified: {2}, DestinationModified: {3}", this.FileName, this.Flag, this.SourceModified, this.DestinationModified);
        }
    }

    /// <summary>
    /// The possible compare (selections) results.
    /// This is in effect a summary per <see cref="DirectoryCompareResultsFlag"/> value.
    /// </summary>
    public class DirectoryCompareSelection
    {
        public DirectoryCompareResultsFlag Flag { get; set; }
        public int Count { get; set; } = 0;
        public long Size { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether 'copy' was pre-selected or selected by the user.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [copy selected]; otherwise, <c>false</c>.
        /// </value>
        public bool CopySelected { get; set; } = false;

        public DirectoryCompareSelection() { }

        public DirectoryCompareSelection(DirectoryCompareResultsFlag flag)
        {
            Flag = flag;
        }
    }


    /// <summary>
    /// Class to compare two sets of scan results, one representing the 'source' and the other representing the 'destination'.
    /// The compare is one-directional, from source to destination. If a file exists on the destination but not the source this is not reflected.
    /// </summary>
    public class DirectoryCompare
    {
        private readonly UserOptions options = new UserOptions();

        public DirectoryCompare() { }

        /// <summary>
        /// Compares the specified source results.
        /// Because the compare is done in memory with no file IO, it is synchronous and non-cancellable.
        /// The comparison is performed based on the UTC Modified dates. The <see cref="UserOptions.TimeWindowMilliseconds"/> value is used to
        /// provide a 'window of time difference' between the two dates to be deemed equal.
        /// </summary>
        /// <param name="sourceResults">The source results.</param>
        /// <param name="destinationResults">The destination results.</param>
        /// <returns>The results of the compare function in <see cref="DirectoryCompareResults"/> format.</returns>
        public DirectoryCompareResults Compare(DirectoryScanResults sourceResults, DirectoryScanResults destinationResults)
        {
            DirectoryCompareResults results = new DirectoryCompareResults();

            results.SourceFileCount = sourceResults.Results.Count;
            results.SourceRootDir = sourceResults.RootDir;
            results.DestinationFileCount = destinationResults.Results.Count;
            results.DestinationRootDir = destinationResults.RootDir;

            foreach (DirectoryScanResultsEntry src in sourceResults.Results)
            {
                DirectoryCompareResultsEntry compare = new DirectoryCompareResultsEntry(src.FileName, src.Size, DirectoryCompareResultsFlag.NewFile, src.Modified, DateTime.MinValue);

                if (src.Status == DirectoryScanResultsEntryStatus.Error)
                {
                    compare.Flag = DirectoryCompareResultsFlag.Error;
                    compare.Status = DirectoryCompareResultsEntryStatus.ErrorOnSource;
                    compare.ErrorMessage = src.ErrorMessage;
                    results.SourceErrorCount++;
                }
                else
                {
                    DirectoryScanResultsEntry dest = destinationResults.Results.Find((item) => item.FileName == src.FileName);
                    if (dest != null)
                    {
                        if (dest.Status == DirectoryScanResultsEntryStatus.Error)
                        {
                            compare.Flag = DirectoryCompareResultsFlag.Error;
                            compare.Status = DirectoryCompareResultsEntryStatus.ErrorOnDestination;
                            compare.ErrorMessage = dest.ErrorMessage;
                            results.DestinationErrorCount++;
                        }
                        else
                        {
                            compare.DestinationModified = dest.Modified;
                            TimeSpan ts = src.Modified - dest.Modified;

                            if (Math.Abs(ts.TotalMilliseconds) < (double)options.TimeWindowMilliseconds)  // Allow a window
                            {
                                compare.Flag = DirectoryCompareResultsFlag.Match;
                            }
                            else if (dest.Modified < src.Modified)
                            {
                                compare.Flag = DirectoryCompareResultsFlag.Newer;
                            }
                            else
                            {
                                compare.Flag = DirectoryCompareResultsFlag.Older;
                            }
                        }
                    }
                    else
                    {
                        compare.Flag = DirectoryCompareResultsFlag.NewFile;
                    }
                }
                results.Selections[(int)compare.Flag].Count++;
                results.Selections[(int)compare.Flag].Size += src.Size;
                results.Entries.Add(compare);
            }

            return results;
        }
    }
}

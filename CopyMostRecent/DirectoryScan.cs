using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CopyMostRecent
{
    /// <summary>
    /// Class containing the results of a recursive directory scan.
    /// </summary>
    public class DirectoryScanResults
    {
        /// <summary>
        /// The root directory of the scan.
        /// Returned paths of files exclude this common value.
        /// </summary>
        public string RootDir;

        /// <summary>
        /// The List of files found - List of <seealso cref="DirectoryScanResultsEntry"/> 
        /// </summary>
        public List<DirectoryScanResultsEntry> Results;

        public DirectoryScanResults() 
        { 
            this.Results = new List<DirectoryScanResultsEntry>();
        }
    }

    /// <summary>
    /// Status of an Entry in the results list.
    /// </summary>
    public enum DirectoryScanResultsEntryStatus
    {
        Success,
        Error
    }

    /// <summary>
    /// Represents an entry in the list of files found during the scan.
    /// </summary>
    public class DirectoryScanResultsEntry
    {
        /// <summary>
        /// Gets or sets the path and name of the file.
        /// The path excludes the Root directory
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the modified date/time of the file (UTC).
        /// </summary>
        /// <value>
        /// The modified date/time.
        /// </value>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        /// <value>
        /// The size in bytes.
        /// </value>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the file attributes.
        /// </summary>
        /// <value>
        /// The attributes (from <seealso cref="FileInfo"/>.
        /// </value>
        public FileAttributes Attributes { get; set; }

        /// <summary>
        /// Gets or sets the scan status of the entry. See <seealso cref="DirectoryScanResultsEntryStatus"/>.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public DirectoryScanResultsEntryStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the error message, for entries that are flagged as <seealso cref="DirectoryScanResultsEntryStatus.Error"/>.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        public DirectoryScanResultsEntry() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryScanResultsEntry"/> class.
        /// Overload used to construct a Success entry.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="modified">The modified date/time.</param>
        /// <param name="size">The file size.</param>
        /// <param name="attributes">The file attributes.</param>
        public DirectoryScanResultsEntry(string fileName, DateTime modified, long size, FileAttributes attributes)
        {
            this.FileName = fileName;
            this.Modified = modified;
            this.Size = size;
            this.Attributes = attributes;
            this.Status = DirectoryScanResultsEntryStatus.Success;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryScanResultsEntry"/> class.
        /// Overload used to construct an Error entry.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="errorMessage">The error message.</param>
        public DirectoryScanResultsEntry(string fileName, string errorMessage)
        {
            this.FileName = fileName;
            this.Status = DirectoryScanResultsEntryStatus.Error;
            this.ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            return String.Format("FileName: {0}, Size: {1}, Modified: {2}", this.FileName, this.Size, this.Modified);
        }
    }

    /// <summary>
    /// Class to perform a recursive directory scan.
    /// Files (or directories) that are in the <seealso cref="UserOptions.IgnoreList"/> are ignored.
    /// </summary>
    public class DirectoryScan
    {
        private readonly UserOptions options = new UserOptions();
        private string[] ignoreList;

        /// <summary>
        /// Gets or sets the root directory to begin the scan from.
        /// </summary>
        /// <value>
        /// The root dir.
        /// </value>
        public string RootDir { get; set; }

        public DirectoryScan()
        {
            buildIgnoreList();
        }

        public DirectoryScan(string rootDir) 
        { 
            this.RootDir = rootDir;
            this.buildIgnoreList();
        }


        /// <summary>
        /// Begins the scan asynchronously. Uses <seealso cref="Task.Run"/> to execute on a separate thread.
        /// </summary>
        /// <param name="rootDir">The root directory to scan.</param>
        /// <param name="token">A cancellation token, to allow cancel of the search.</param>
        /// <returns>A promise of a <seealso cref="DirectoryScanResults"/> object.</returns>
        public async Task<DirectoryScanResults> ScanAsync(string rootDir, CancellationToken token)
        {
            this.RootDir = rootDir;
            DirectoryScanResults results = await Task.Run(() => this.doScan(token)); // Use Task.Run to run on an actual separate thread
            return results;
        }

        private async Task<DirectoryScanResults> doScan(CancellationToken token)
        {
            if (this.RootDir == null || this.RootDir == string.Empty)
                throw new Exception("Root Directory value is required.");

            DirectoryScanResults results = new DirectoryScanResults();
            results.RootDir = this.RootDir; 

            SearchRecursively(this.RootDir, results, token);

            return results;
        }

        private void SearchRecursively(string path, DirectoryScanResults dsResults, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            foreach (string file in Directory.GetFiles(path))
            {

                if (!onIgnoreList(file))
                {
                    string shortFile = file.Substring(dsResults.RootDir.Length);
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        DirectoryScanResultsEntry dsre = new DirectoryScanResultsEntry(shortFile, fi.LastWriteTimeUtc, fi.Length, fi.Attributes);
                        dsResults.Results.Add(dsre);
                    }
                    catch (Exception ex)
                    {
                        DirectoryScanResultsEntry dsre = new DirectoryScanResultsEntry(shortFile, ex.Message);
                        dsResults.Results.Add(dsre);
                    }
                }
                if (token.IsCancellationRequested)
                    return;
            }

            foreach (string directory in Directory.GetDirectories(path))
            {
                if (!onIgnoreList(directory))
                    SearchRecursively(directory, dsResults, token);
            }
        }

        private void buildIgnoreList()
        {
            ignoreList = options.IgnoreList.Trim().Split('\n');
            for (int i = 0; i < ignoreList.Length; i++)
                ignoreList[i] = ignoreList[i].Trim();
        }

        private bool onIgnoreList(string file)
        {
            foreach (string pattern in ignoreList)
                if (Regex.IsMatch(Path.GetFileName(file), pattern, RegexOptions.IgnoreCase))
                    return true;
            return false;
        }
    }
}

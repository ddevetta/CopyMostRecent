using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace CopyMostRecent
{
    public partial class FormMain : Form
    {
        
        private bool copyMode = false;
        double totSize = 0d;
        private DateTime lastScanDate, lastRunDate = DateTime.MinValue;
        private DirectoryCompareResults compareResults;
        private DirectoryCopyResults copyResults;
        private ProgressLine progLine = new ProgressLine();
        private Plan currentPlan = null;

        private CancellationTokenSource cancelSource = null;

        public FormMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormMain"/> class,
        /// passing a 'source directory' (from Directory context-menu selection).
        /// If there are any plans associated with that source directory, the dialog 
        /// is pre-loaded with the first plan found, as if the user had loaded it.
        /// </summary>
        /// <param name="suppliedDirectory">The supplied directory.</param>
        public FormMain(string suppliedDirectory)
        {
            InitializeComponent();
            this.tbSourceDir.Text = suppliedDirectory;
            try
            {
                PlanManager pm = new PlanManager(Globals.PlanFile);
                List<Plan> initialPlans = pm.FindPlans(suppliedDirectory);
                if (initialPlans.Count > 0)
                {
                    currentPlan = initialPlans[0];
                    tbDestDir.Text = currentPlan.DestinationDirectory;
                    lblPlanName.Text = currentPlan.PlanName;
                }
            }
            catch (Exception ex)
            {
                showMessage(ex.Message + " : " + Globals.PlanFile, true);
            }
        }
        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false; 
            try
            {
                if (copyMode)
                    doCopy();
                else
                    doScan();
            }
            catch (Exception ex)
            {
                showMessage(ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : ""), true);
                tbSourceDir.Focus();
                progressBar.Visible = false;
                return;
            }
        }

        private async void doScan()
        {
            errorLabel.Text = string.Empty;
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Visible = true;

            if (tbSourceDir.Text.Trim() == string.Empty)
            {
                showMessage("Enter or select the source folder to back up from", true);
                tbSourceDir.Focus();
                return;
            }
            if (tbDestDir.Text.Trim() == string.Empty)
            {
                showMessage("Enter or select the destination folder to back up to", true);
                tbDestDir.Focus();
                return;
            }
            if (await directoryNotFoundAsync(tbSourceDir.Text))
            {
                showMessage("The source folder does not exist or is not reachable", true);
                tbSourceDir.Focus();
                return;
            }
            if (await directoryNotFoundAsync(tbDestDir.Text))
            {
                showMessage("The destination location does not exist or is not reachable", true);
                tbDestDir.Focus();
                return;
            }

            flowResults.Controls.Clear();
            btnClose.Text = "Cancel";
            showMessage("Scanning...");

            this.cancelSource = new CancellationTokenSource();
            CancellationToken token = this.cancelSource.Token;

            try
            {
                DirectoryScan dsSource = new DirectoryScan(tbSourceDir.Text);
                DirectoryScan dsDest = new DirectoryScan(tbDestDir.Text);

                // Kick off the scan tasks simultaneously
                Task<DirectoryScanResults> sourceTask = doScanAsync(dsSource, token);
                Task<DirectoryScanResults> destTask = doScanAsync(dsDest, token);

                await Task.WhenAll(sourceTask, destTask);

                DirectoryCompare comparer = new DirectoryCompare();
                compareResults = comparer.Compare(sourceTask.Result, destTask.Result);
            }
            catch (Exception ex) 
            {
                showMessage(ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : ""), true);
                tbSourceDir.Focus();
                return;
            }

            btnClose.Text = "Close";
            progressBar.Visible = false;

            if (token.IsCancellationRequested)
            {
                showMessage("Operation was cancelled!", true);
                cancelSource.Dispose();
                return;
            }

            lastScanDate = DateTime.Now;
            showCompareResults(compareResults);
        }

        private async Task<DirectoryScanResults> doScanAsync(DirectoryScan directorySearch, CancellationToken token)
        {
            Task<DirectoryScanResults> task = directorySearch.ScanAsync(directorySearch.RootDir, token);
            DirectoryScanResults r = await task;
            if (task.Exception != null)
                throw task.Exception;

            return r;
        }


        private async Task<bool> directoryNotFoundAsync(string dir)
        {
            return await Task.Run(() => !Directory.Exists(dir));
        }

        private void hideResults()
        {
            errorLabel.Text = lblPlanName.Text = string.Empty;
            progressBar.Visible = btnShowDetails.Visible = flowResults.Visible = false;
            btnClear.Enabled = false;
            copyMode = false;
            btnGo.Text = "Scan";
            btnGo.Enabled = true;
            lastScanDate = lastRunDate = DateTime.MinValue;
        }

        private void showCompareResults(DirectoryCompareResults results)
        {
            ResultsLine[] resultLines = new ResultsLine[]
            {
                new ResultsLine("Files in Source", results.SourceFileCount),
                new ResultsLine("Files in Destination", results.DestinationFileCount),
                new ResultsLine("Not Changed", results.Selections[(int)DirectoryCompareResultsFlag.Match], false),
                new ResultsLine("New Files", results.Selections[(int)DirectoryCompareResultsFlag.NewFile], true),
                new ResultsLine("Modified Files", results.Selections[(int)DirectoryCompareResultsFlag.Newer], true)
            };
            flowResults.Controls.AddRange(resultLines);
            if (results.Selections[(int)DirectoryCompareResultsFlag.Older].Count > 0)
            {
                ResultsLine ln = new ResultsLine("Older than Backup!", results.Selections[(int)DirectoryCompareResultsFlag.Older], false);
                ln.DescriptionForeColor = Color.Red;
                flowResults.Controls.Add(ln);
            }
            if (results.SourceErrorCount > 0)
            {
                ResultsLine ln = new ResultsLine("Errors scanning Source", results.SourceErrorCount);
                ln.DescriptionForeColor = Color.Red;
                flowResults.Controls.Add(ln);
            }
            if (results.DestinationErrorCount > 0)
            {
                ResultsLine ln = new ResultsLine("Errors scanning Destination", results.DestinationErrorCount);
                ln.DescriptionForeColor = Color.Red;
                flowResults.Controls.Add(ln);
            }

            foreach (ResultsLine ln in flowResults.Controls)
            {
                if (ln.ShowCopy)
                    ln.CopyCheckChanged += _CopyCheckChanged;
            }
            errorLabel.Text = string.Empty;
            btnShowDetails.Visible = flowResults.Visible = true;
            btnClear.Enabled = true;
            copyMode = true;
            btnGo.Enabled = (checkReady(results) > 0); 
            btnGo.Text = "Copy";
        }


        private async void doCopy()
        {
            copyResults = new DirectoryCopyResults();
            flowResults.Controls.Add(progLine);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
            progressBar.Visible = true;
            btnClose.Text = "Cancel";
            showMessage("Copying...");

            this.cancelSource = new CancellationTokenSource();

            try
            {
                 copyResults = await doCopyAsync(cancelSource.Token);

                 errorLabel.Text = string.Empty;
            }
            catch (Exception ex)
            {
                showMessage(ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : ""), true);
                tbSourceDir.Focus();
            }

            if (cancelSource.Token.IsCancellationRequested)
            {
                showMessage("Operation has been cancelled.", true);
                cancelSource.Dispose();
            }

            progLine.ShowResults(copyResults);
            flowResults.ScrollControlIntoView(progLine);
            btnClose.Text = "Close";
            btnGo.Text = "Scan";
            btnGo.Enabled = true;
            copyMode = false;
            cancelSource.Dispose();
            cancelSource = null;

            progressBar.Value = 0;
            progressBar.Visible = false;
            lastRunDate = DateTime.Now;
        }

        private async Task<DirectoryCopyResults> doCopyAsync(CancellationToken token)
        {
            DirectoryCopy copier = new DirectoryCopy(compareResults);
            DirectoryCopyResults r = new DirectoryCopyResults();

            IProgress<DirectoryCopyProgress> progress = new Progress<DirectoryCopyProgress>((i) => 
            {
                progLine.Update(i);
                progressBar.Value = (int)(((double)i.CopySize / this.totSize) * 100);
                flowResults.ScrollControlIntoView(progLine);
            });

            r = await Task.Run(() => copier.CopyAsync(progress, token));

            return r;
        }

        private long checkReady(DirectoryCompareResults results)
        {
            int totFiles = 0;
            this.totSize = 0;
            foreach (DirectoryCompareSelection dcs in results.Selections)
            {
                if (dcs.CopySelected)
                {
                    totFiles += dcs.Count;
                    this.totSize += dcs.Size;
                }
            }
            return totFiles;
        }

        private void showMessage(string msg, bool isError = false)
        {
            if (isError)
            {
                errorLabel.ForeColor = Color.Red;
                progressBar.Visible = false;
            }
            else
                errorLabel.ForeColor = Color.Black;
            
            errorLabel.Text = msg.Replace("\r\n"," - ");
            btnGo.Enabled = true;
            

        }

        private void btnSourceBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserSource.SelectedPath = tbSourceDir.Text;
            if (folderBrowserSource.ShowDialog() == DialogResult.OK)
            {
                tbSourceDir.Text = folderBrowserSource.SelectedPath;
            }
        }

        private void btnDestBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDest.SelectedPath = tbDestDir.Text;
            if (folderBrowserDest.ShowDialog() == DialogResult.OK)
            {
                tbDestDir.Text = folderBrowserDest.SelectedPath;
            }
        }

        private void btnShowDetails_Click(object sender, EventArgs e)
        {
            FormDetail formDetail = new FormDetail(compareResults);
            formDetail.ShowDialog();
        }

        private void _TextChanged(object sender, EventArgs e)
        {
            hideResults();
        }

        private void _CopyCheckChanged(object sender, EventArgs e)
        {
            if (checkReady(compareResults) > 0)
                btnGo.Enabled = true;
            else
                btnGo.Enabled = false;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            hideResults();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (btnClose.Text == "Cancel")
            {
                CancelCancelSource(cancelSource);
                btnClose.Text = "Close";
            }
            else
                this.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void CancelCancelSource(CancellationTokenSource source)
        {
            if (source != null)
            {
                showMessage("Cancelling...", true);
                Application.DoEvents();
                source.Cancel();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void loadPlanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPlanManager planManager = new FormPlanManager();
            planManager.DialogMode = PlanManagerDialogMode.Load;
            planManager.CurrentPlan = this.currentPlan;
            if (planManager.ShowDialog() == DialogResult.OK)
            {
                this.currentPlan = planManager.CurrentPlan;
                tbSourceDir.Text = planManager.CurrentPlan?.SourceDirectory;
                tbDestDir.Text = planManager.CurrentPlan?.DestinationDirectory;
                lblPlanName.Text = planManager.CurrentPlan?.PlanName;
            }
        }

        private void savePlanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tbSourceDir.Text.Trim() == string.Empty)
            {
                tbSourceDir.Focus();
                showMessage("A plan consists of a Source and a Destination. Source directory is missing.", true);
                return;
            }
            if (tbDestDir.Text.Trim() == string.Empty)
            {
                tbDestDir.Focus();
                showMessage("A plan consists of a Source and a Destination (target). Target directory is missing.", true);
                return;
            }
            FormPlanManager planManager = new FormPlanManager();
            planManager.SourceDirectory = tbSourceDir.Text;
            planManager.DestinationDirectory = tbDestDir.Text;
            planManager.DialogMode = PlanManagerDialogMode.Save;
            if (planManager.ShowDialog() == DialogResult.OK)
                lblPlanName.Text = planManager.CurrentPlan?.PlanName;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOptions options = new FormOptions();
            if (options.ShowDialog() == DialogResult.OK) // Changes made and saved
            {
                Console.WriteLine("new options"); // refresh the options
            }
        }
    }
}

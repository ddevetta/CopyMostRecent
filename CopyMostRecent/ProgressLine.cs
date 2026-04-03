using System;
using System.Drawing;
using System.Windows.Forms;

namespace CopyMostRecent
{
    /// <summary>
    /// User Control to show copy progress, and final results.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.UserControl" />
    public partial class ProgressLine : UserControl
    {
        private DirectoryCopyResults copyResults;

        public ProgressLine()
        {
            InitializeComponent();

            this.BackColor = Color.FromArgb(210, 220, 240);
            lblCount.Text = lblSize.Text = lblFileName.Text = lblState.Text = string.Empty;
            lblFileName.BackColor = Color.Transparent;
            btnErrors.Visible = false;
        }

        public override string Text
        { 
            get { return lblFileName.Text; }
            set { lblFileName.Text = value; }
        }

        /// <summary>
        /// Updates the control with progress information.
        /// </summary>
        /// <param name="progress">The progress.</param>
        public void Update(DirectoryCopyProgress progress)
        {
            lblCount.Text = string.Format("{0:n0}", progress.CopyCount);
            lblSize.Text = Classes.FormatSize(progress.CopySize);
            lblFileName.Text = progress.CurrentFileName;
            lblState.Text = progress.CopyState.ToString();
        }

        /// <summary>
        /// Updates the control with final results.
        /// If there were any copy errors, a button is enabled which will allow the user 
        /// to view the errors in a separate dialog (<seealso cref="FormDetail"/>)
        /// </summary>
        /// <param name="results">The results.</param>
        public void ShowResults(DirectoryCopyResults results)
        {
            this.copyResults = results;
            lblCount.Text = string.Format("{0:n0}", results.TotalCopyCount);
            lblSize.Text = Classes.FormatSize(results.TotalCopySize);

            if (results.Errors.Count > 0)
            {
                this.BackColor = Color.FromArgb(240, 220, 210);
                lblFileName.Text = string.Format("Copy completed with {0} error(s) - click to view errors.", results.Errors.Count);
                btnErrors.Visible = true;
            }
            else
            {
                this.BackColor = Color.FromArgb(210, 240, 210);
                lblFileName.Text = "Copy Complete";
                btnErrors.Visible = false;
            }
        }

        private void btnErrors_Click(object sender, EventArgs e)
        {
            FormDetail formDetail = new FormDetail(copyResults);
            formDetail.ShowDialog();
        }
    }
}

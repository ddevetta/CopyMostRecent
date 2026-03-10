using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyMostRecent
{
    /// <summary>
    /// User Control to show each line of the results of the scan/compare operation.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.UserControl" />
    public partial class ResultsLine : UserControl
    {
        private long count = 0;
        private long size = 0;
        private bool showCopy = false;
        private DirectoryCompareSelection selection = null;

        /// <summary>
        /// Occurs when the 'copy checkbox' is changed.
        /// The state is also persisted in the attached <seealso cref="DirectoryCompareSelection"/> object.
        /// </summary>
        public event EventHandler CopyCheckChanged;

        public string Description
        {
            get { return lbDescription.Text; }
            set { lbDescription.Text = value; }
        }
        public long Count
        {
            get { return count; }
            set 
            { 
                count = value;
                lbCount.Text = string.Format("0:n0", count);
            }
        }
        public long FileSize
        {
            get { return size; }
            set 
            { 
                size = value; 
                lbSize.Text = Classes.FormatSize(size);
            }
        }
        public Boolean Copy
        {
            get { return cbCopy.Checked; }
            set { cbCopy.Checked = value; }
        }
        public Boolean ShowCopy
        {
            get { return showCopy; }
            set { showCopy = value; }
        }
        public Color DescriptionForeColor
        {
            get { return lbDescription.ForeColor; }
            set { lbDescription.ForeColor = value; }
        }

        public ResultsLine()
        {
            InitializeComponent();
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsLine"/> class.
        /// This overload creates a line with just the description and count, and
        /// without the size display, or the checkbox to allow selection for copying.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="count">The file count.</param>
        public ResultsLine(string description, long count)
        {
            InitializeComponent();
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            lbDescription.Text = description;
            this.count = count;
            lbCount.Text = string.Format("{0:n0}", count);
            lbSize.Visible = false;
            cbCopy.Visible = showCopy = false; 
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsLine"/> class.
        /// This overload creates a line with the description and count,
        /// including the size display, and the checkbox to allow selection for copying,
        /// with the checkbox pre-initialised to 'checked' or 'not checked'.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="selection">The selection.</param>
        /// <param name="copy">if set to <c>true</c> [copy].</param>
        public ResultsLine(string description, DirectoryCompareSelection selection, bool copy)
        {
            InitializeComponent();
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            this.selection = selection;
            lbDescription.Text = description;
            this.count = selection.Count;
            lbCount.Text = string.Format("{0:n0}", count);
            this.size = selection.Size;
            lbSize.Text = Classes.FormatSize(size);
            cbCopy.Checked = copy;
            cbCopy.Visible = showCopy = true; 
            toolTip.SetToolTip(cbCopy, "Copy these to the Destination");
        }

        private void cbCopy_CheckedChanged(object sender, EventArgs e)
        {
            if (this.selection != null)
            {
                this.selection.CopySelected = ((CheckBox)sender).Checked;
            }
            if (this.CopyCheckChanged != null)
            {
                CopyCheckChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

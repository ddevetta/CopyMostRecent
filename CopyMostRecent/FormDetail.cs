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
    public partial class FormDetail : Form
    {
        private DirectoryCompareResults compareResults;
        private DirectoryCopyResults copyResults;

        private List<DirectoryCompareResultsEntry> filtered;

        public FormDetail()
        {
            InitializeComponent();
        }
        public FormDetail(DirectoryCompareResults results)
        {
            InitializeComponent();
            this.compareResults = results;
            dgResults.Columns["InnerExceptionMessage"].Visible = false;
            this.setFilter("rb2");
        }
        public FormDetail(DirectoryCopyResults results)
        {
            InitializeComponent();
            this.copyResults = results;
            gbFilter.Visible = false;
            dgResults.Columns["SourceModified"].Visible = dgResults.Columns["DestinationModified"].Visible = dgResults.Columns["Status"].Visible = dgResults.Columns["FileSize"].Visible = false;
            dgResults.DataSource = copyResults.Errors;
        }

        private void _CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
                setFilter(rb.Name);
        }

        private void setFilter(string name)
        {
            dgResults.Columns["Status"].Visible = dgResults.Columns["ErrorMessage"].Visible = false;
            switch (name)
            {
                case "rb1":     // No Change
                    filtered = compareResults.Entries.FindAll((e) => e.Status == DirectoryCompareResultsEntryStatus.Success && e.Flag == DirectoryCompareResultsFlag.Match);
                    break;
                case "rb2":     // Modified
                    filtered = compareResults.Entries.FindAll((e) => e.Status == DirectoryCompareResultsEntryStatus.Success && e.Flag == DirectoryCompareResultsFlag.Newer);
                    break;
                case "rb3":     // New
                    filtered = compareResults.Entries.FindAll((e) => e.Status == DirectoryCompareResultsEntryStatus.Success && e.Flag == DirectoryCompareResultsFlag.NewFile);
                    break;
                case "rb4":     // Older
                    filtered = compareResults.Entries.FindAll((e) => e.Status == DirectoryCompareResultsEntryStatus.Success && e.Flag == DirectoryCompareResultsFlag.Older);
                    break;
                case "rb5":     // Error
                    filtered = compareResults.Entries.FindAll((e) => e.Status != DirectoryCompareResultsEntryStatus.Success);
                    dgResults.Columns["Status"].Visible = dgResults.Columns["ErrorMessage"].Visible = true;
                    break;
            }

            dgResults.DataSource = filtered;
            dgResults.Refresh();
        }
        

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

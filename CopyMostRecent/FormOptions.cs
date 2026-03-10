using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CopyMostRecent
{
    public partial class FormOptions : Form
    {
        private UserOptions options = new UserOptions();
        private DialogResult result = DialogResult.None;  // Usage here is : None=no changes, Yes=changes, OK=changes made and saved

        public FormOptions()
        {
            InitializeComponent();

            options.SettingChanging += new SettingChangingEventHandler(options_Changing);

            tbTimeWindow.DataBindings.Add(new Binding("Text", options, "TimeWindowMilliseconds"));
            tbIgnoreList.DataBindings.Add(new Binding("Text", options, "IgnoreList"));

            this.DialogResult = result;
        }

        private void form_Closing(object sender, FormClosingEventArgs e)
        {
            if (result == DialogResult.Yes)
            {
                if (MessageBox.Show("Do you want to lose the changes?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void options_Changing(object sender, EventArgs e)
        {
            result = DialogResult.Yes;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!validate())
                return;

            if (result == DialogResult.Yes)
            {
                options.Save();
                this.DialogResult = result = DialogResult.OK;
            }
            else
                Close();
        }

        private bool validate()
        {
            if (!int.TryParse(tbTimeWindow.Text, out var time))
            {
                tabControl.SelectedTab = tabPage1;
                MessageBox.Show("The value for 'Time Window' is not a valid number.", "Number error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; 
            }

            foreach (string pattern in tbIgnoreList.Text.Trim().Split('\n'))
            {
                try
                {
                    Regex rx = new Regex(pattern, RegexOptions.IgnoreCase);
                }
                catch (Exception ex)
                {
                    tabControl.SelectedTab = tabPage2;
                    MessageBox.Show($"The pattern '{pattern}' contains errors:\n{ex.Message}", "Regex pattern error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
    }
}

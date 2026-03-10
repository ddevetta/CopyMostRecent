using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyMostRecent
{
    public enum PlanManagerDialogMode
    {
        Manage,
        Save,
        Load
    }
    public partial class FormPlanManager : Form
    {
        private PlanManagerDialogMode dialogMode;
        private PlanManager planManager = null;
        private Plan currentPlan;

        public Plan CurrentPlan 
        { 
            get { return currentPlan; }
            set
            {
                currentPlan = value;
                getPlanNodes();
            }
        }
        public string PlanName { get; set; }
        public string Group { get; set; }
        public string SourceDirectory { get; set; }
        public string DestinationDirectory { get; set; }
        public PlanManagerDialogMode DialogMode 
        {
            get { return dialogMode; }
            set 
            { 
                dialogMode = value; 
                setMode(value);
            } 
        }
        public DateTime LastRunOn { get; set; }

        public FormPlanManager()
        {
            InitializeComponent();
            planManager = new PlanManager(Globals.PlanFile);
            setMode(PlanManagerDialogMode.Manage);
            this.DialogResult = DialogResult.Cancel;
        }

        private void setMode(PlanManagerDialogMode mode)
        {
            this.dialogMode = mode;
            switch (mode)
            {
                case PlanManagerDialogMode.Load:
                    this.Text = "Load Plan";
                    this.panelSaveAs.Visible = false;
                    this.panelLoad.Visible = true;
                    getPlanNodes();
                    break;
                case PlanManagerDialogMode.Save:
                    this.Text = "Save Plan";
                    this.Height = 166;
                    this.panelSaveAs.Visible = true;
                    this.panelLoad.Visible = false;
                    CurrentPlan = planManager.FindPlan(SourceDirectory, DestinationDirectory);
                    this.tbSaveAsName.Text = CurrentPlan?.PlanName ?? string.Empty;
                    this.cbGroup.DataSource = getGroups();
                    this.cbGroup.Text = CurrentPlan?.Group ?? string.Empty;
                    break;
                default:
                    this.Text = "Plan Manager";
                    break;
            }
        }

        private void getPlanNodes()
        {
            tvPlans.BeginUpdate();
            tvPlans.Nodes.Clear();
            TreeNode ng = new TreeNode("(Not Grouped)");
            tvPlans.Nodes.Add(ng);
            foreach (Plan p in planManager.Plans)
            {
                if (p.Group == null || p.Group == string.Empty)
                {
                    TreeNode n = tvPlans.Nodes[0].Nodes.Add(p.PlanName);
                    if (p.PlanName == currentPlan?.PlanName)
                        tvPlans.SelectedNode = n;
                }

            }
            foreach (string group in planManager.GetGroups())
            {
                TreeNode g = tvPlans.Nodes.Add(group);
                foreach (Plan p in planManager.Plans)
                {
                    if (p.Group != null && p.Group == group )
                    {
                        TreeNode n = g.Nodes.Add(p.PlanName);
                        if (p.PlanName == currentPlan?.PlanName)
                            tvPlans.SelectedNode = n;
                    }
                }
            }
            tvPlans.ExpandAll();
            tvPlans.EndUpdate();
        }

        private object getGroups()
        {
            return planManager.GetGroups();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.SourceDirectory.Trim() == string.Empty || this.DestinationDirectory.Trim() == string.Empty)
            {
                MessageBox.Show(this, "No plan details to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string planName = tbSaveAsName.Text.Trim();

            if (planName == string.Empty)
            {
                MessageBox.Show(this, "Name is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.tbSaveAsName.Focus();
                return;
            }
            if (CurrentPlan == null || planName != CurrentPlan.PlanName)
            {
                Plan p = planManager.FindPlan(planName);
                if (p != null) 
                {
                    if (MessageBox.Show(this, "A plan already exists with this name, do you want to replace it?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        CurrentPlan = p;
                    }
                    else 
                    {
                        this.tbSaveAsName.Focus();
                        return;
                    }
                }
            }
            this.PlanName = planName;
            this.Group = cbGroup.Text.Trim();
            if (CurrentPlan == null)
            {
                CurrentPlan = new Plan(planName, this.Group, this.SourceDirectory, this.DestinationDirectory);
                planManager.Plans.Add(CurrentPlan);
            }
            else
            {
                CurrentPlan.PlanName = planName;
                CurrentPlan.Group = this.Group;
                CurrentPlan.SourceDirectory = this.SourceDirectory;
                CurrentPlan.DestinationDirectory = this.DestinationDirectory;
            }
            planManager.Save(Globals.PlanFile);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (tvPlans.SelectedNode == null || tvPlans.SelectedNode.Level < 1)
            {
                MessageBox.Show("No plan has been selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            CurrentPlan = planManager.FindPlan(tvPlans.SelectedNode.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void tvPlans_DoubleClick(object sender, EventArgs e)
        {
            btnLoad_Click(sender, e);
        }
    }
}

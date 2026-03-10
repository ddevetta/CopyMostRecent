namespace CopyMostRecent
{
    partial class FormPlanManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPlanManager));
            this.panelSaveAs = new System.Windows.Forms.Panel();
            this.cbGroup = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSaveAsName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.panelLoad = new System.Windows.Forms.Panel();
            this.btnLoad = new System.Windows.Forms.Button();
            this.tvPlans = new System.Windows.Forms.TreeView();
            this.panelSaveAs.SuspendLayout();
            this.panelLoad.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSaveAs
            // 
            this.panelSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSaveAs.Controls.Add(this.cbGroup);
            this.panelSaveAs.Controls.Add(this.label2);
            this.panelSaveAs.Controls.Add(this.tbSaveAsName);
            this.panelSaveAs.Controls.Add(this.label1);
            this.panelSaveAs.Controls.Add(this.btnSave);
            this.panelSaveAs.Location = new System.Drawing.Point(12, 12);
            this.panelSaveAs.Name = "panelSaveAs";
            this.panelSaveAs.Size = new System.Drawing.Size(570, 100);
            this.panelSaveAs.TabIndex = 0;
            // 
            // cbGroup
            // 
            this.cbGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbGroup.FormattingEnabled = true;
            this.cbGroup.Location = new System.Drawing.Point(116, 59);
            this.cbGroup.Name = "cbGroup";
            this.cbGroup.Size = new System.Drawing.Size(303, 24);
            this.cbGroup.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Group :";
            // 
            // tbSaveAsName
            // 
            this.tbSaveAsName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSaveAsName.Location = new System.Drawing.Point(116, 25);
            this.tbSaveAsName.Name = "tbSaveAsName";
            this.tbSaveAsName.Size = new System.Drawing.Size(303, 22);
            this.tbSaveAsName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Save As :";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(447, 20);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 32);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panelLoad
            // 
            this.panelLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLoad.Controls.Add(this.btnLoad);
            this.panelLoad.Controls.Add(this.tvPlans);
            this.panelLoad.Location = new System.Drawing.Point(13, 59);
            this.panelLoad.Name = "panelLoad";
            this.panelLoad.Size = new System.Drawing.Size(569, 437);
            this.panelLoad.TabIndex = 1;
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(446, 397);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(90, 32);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // tvPlans
            // 
            this.tvPlans.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvPlans.Location = new System.Drawing.Point(3, 4);
            this.tvPlans.Name = "tvPlans";
            this.tvPlans.Size = new System.Drawing.Size(563, 387);
            this.tvPlans.TabIndex = 0;
            this.tvPlans.DoubleClick += new System.EventHandler(this.tvPlans_DoubleClick);
            // 
            // FormPlanManager
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 508);
            this.Controls.Add(this.panelLoad);
            this.Controls.Add(this.panelSaveAs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormPlanManager";
            this.Text = "FormPlanManager";
            this.panelSaveAs.ResumeLayout(false);
            this.panelSaveAs.PerformLayout();
            this.panelLoad.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSaveAs;
        private System.Windows.Forms.TextBox tbSaveAsName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cbGroup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelLoad;
        private System.Windows.Forms.TreeView tvPlans;
        private System.Windows.Forms.Button btnLoad;
    }
}
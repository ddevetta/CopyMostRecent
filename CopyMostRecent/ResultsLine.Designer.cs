namespace CopyMostRecent
{
    partial class ResultsLine
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbDescription = new System.Windows.Forms.Label();
            this.lbCount = new System.Windows.Forms.Label();
            this.lbSize = new System.Windows.Forms.Label();
            this.cbCopy = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lbDescription
            // 
            this.lbDescription.AutoSize = true;
            this.lbDescription.Location = new System.Drawing.Point(4, 4);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(73, 16);
            this.lbDescription.TabIndex = 0;
            this.lbDescription.Text = "description";
            this.lbDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbCount
            // 
            this.lbCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCount.Location = new System.Drawing.Point(198, 1);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(100, 23);
            this.lbCount.TabIndex = 1;
            this.lbCount.Text = "count";
            this.lbCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbSize
            // 
            this.lbSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSize.Location = new System.Drawing.Point(304, 1);
            this.lbSize.Name = "lbSize";
            this.lbSize.Size = new System.Drawing.Size(100, 23);
            this.lbSize.TabIndex = 2;
            this.lbSize.Text = "size";
            this.lbSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbCopy
            // 
            this.cbCopy.AutoSize = true;
            this.cbCopy.Location = new System.Drawing.Point(423, 5);
            this.cbCopy.Name = "cbCopy";
            this.cbCopy.Size = new System.Drawing.Size(18, 17);
            this.cbCopy.TabIndex = 3;
            this.cbCopy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbCopy.UseVisualStyleBackColor = true;
            this.cbCopy.CheckedChanged += new System.EventHandler(this.cbCopy_CheckedChanged);
            // 
            // toolTip
            // 
            this.toolTip.IsBalloon = true;
            // 
            // ResultsLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbCopy);
            this.Controls.Add(this.lbSize);
            this.Controls.Add(this.lbCount);
            this.Controls.Add(this.lbDescription);
            this.Name = "ResultsLine";
            this.Size = new System.Drawing.Size(441, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbDescription;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.Label lbSize;
        private System.Windows.Forms.CheckBox cbCopy;
        private System.Windows.Forms.ToolTip toolTip;
    }
}

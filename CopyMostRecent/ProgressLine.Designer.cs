namespace CopyMostRecent
{
    partial class ProgressLine
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
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.btnErrors = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblFileName
            // 
            this.lblFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileName.Location = new System.Drawing.Point(7, 8);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(454, 23);
            this.lblFileName.TabIndex = 0;
            this.lblFileName.Text = "lblFileName";
            this.lblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblState
            // 
            this.lblState.Location = new System.Drawing.Point(7, 31);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(100, 23);
            this.lblState.TabIndex = 1;
            this.lblState.Text = "lblState";
            this.lblState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCount
            // 
            this.lblCount.Location = new System.Drawing.Point(180, 31);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(120, 23);
            this.lblCount.TabIndex = 2;
            this.lblCount.Text = "lblCount";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSize
            // 
            this.lblSize.Location = new System.Drawing.Point(308, 31);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(98, 23);
            this.lblSize.TabIndex = 3;
            this.lblSize.Text = "lblSize";
            this.lblSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnErrors
            // 
            this.btnErrors.Location = new System.Drawing.Point(425, 5);
            this.btnErrors.Name = "btnErrors";
            this.btnErrors.Size = new System.Drawing.Size(36, 28);
            this.btnErrors.TabIndex = 4;
            this.btnErrors.Text = "...";
            this.btnErrors.UseVisualStyleBackColor = true;
            this.btnErrors.Click += new System.EventHandler(this.btnErrors_Click);
            // 
            // ProgressLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnErrors);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.lblFileName);
            this.Name = "ProgressLine";
            this.Size = new System.Drawing.Size(474, 68);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Button btnErrors;
    }
}

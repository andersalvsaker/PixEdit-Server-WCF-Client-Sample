namespace MyPixEditServerClientApp
{
    partial class PSClientApp
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
            this.btnCreateJob = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.btnCollectFinishedJob = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCreateJob
            // 
            this.btnCreateJob.Location = new System.Drawing.Point(12, 12);
            this.btnCreateJob.Name = "btnCreateJob";
            this.btnCreateJob.Size = new System.Drawing.Size(75, 23);
            this.btnCreateJob.TabIndex = 0;
            this.btnCreateJob.Text = "Create job";
            this.btnCreateJob.UseVisualStyleBackColor = true;
            this.btnCreateJob.Click += new System.EventHandler(this.btnCreateJob_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Job status:";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(76, 48);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 13);
            this.labelStatus.TabIndex = 2;
            // 
            // btnCollectFinishedJob
            // 
            this.btnCollectFinishedJob.Location = new System.Drawing.Point(15, 75);
            this.btnCollectFinishedJob.Name = "btnCollectFinishedJob";
            this.btnCollectFinishedJob.Size = new System.Drawing.Size(116, 23);
            this.btnCollectFinishedJob.TabIndex = 3;
            this.btnCollectFinishedJob.Text = "Collect finished job";
            this.btnCollectFinishedJob.UseVisualStyleBackColor = true;
            this.btnCollectFinishedJob.Click += new System.EventHandler(this.btnCollectFinishedJob_Click);
            // 
            // PSClientApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 235);
            this.Controls.Add(this.btnCollectFinishedJob);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCreateJob);
            this.Name = "PSClientApp";
            this.Text = "PixEdit Server Client Application";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PSClientApp_FormClosing);
            this.Load += new System.EventHandler(this.PSClientApp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreateJob;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button btnCollectFinishedJob;
    }
}


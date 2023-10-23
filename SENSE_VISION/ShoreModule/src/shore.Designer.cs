namespace Sense.Vision.ShoreModule
{
    partial class ShoreModule
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFaceDet = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblYarpPort = new System.Windows.Forms.Label();
            this.lblYarpServer = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status :";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(12, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Face Detect : 0";
            // 
            // lblFaceDet
            // 
            this.lblFaceDet.AllowDrop = true;
            this.lblFaceDet.AutoSize = true;
            this.lblFaceDet.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFaceDet.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblFaceDet.Location = new System.Drawing.Point(124, 30);
            this.lblFaceDet.Name = "lblFaceDet";
            this.lblFaceDet.Size = new System.Drawing.Size(16, 17);
            this.lblFaceDet.TabIndex = 2;
            this.lblFaceDet.Text = "0";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblStatus.Location = new System.Drawing.Point(85, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(64, 17);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Stopped";
            // 
            // lblYarpPort
            // 
            this.lblYarpPort.AutoSize = true;
            this.lblYarpPort.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYarpPort.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblYarpPort.Location = new System.Drawing.Point(13, 52);
            this.lblYarpPort.Name = "lblYarpPort";
            this.lblYarpPort.Size = new System.Drawing.Size(88, 17);
            this.lblYarpPort.TabIndex = 4;
            this.lblYarpPort.Text = "YarpPort :";
            // 
            // lblYarpServer
            // 
            this.lblYarpServer.AutoSize = true;
            this.lblYarpServer.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYarpServer.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblYarpServer.Location = new System.Drawing.Point(13, 73);
            this.lblYarpServer.Name = "lblYarpServer";
            this.lblYarpServer.Size = new System.Drawing.Size(168, 17);
            this.lblYarpServer.TabIndex = 5;
            this.lblYarpServer.Text = "YarpServer : Stopped";
            // 
            // ShoreModule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(309, 110);
            this.Controls.Add(this.lblYarpServer);
            this.Controls.Add(this.lblYarpPort);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblFaceDet);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "ShoreModule";
            this.Text = "ShoreModule";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShoreModule_FormClosed);
            this.Load += new System.EventHandler(this.ShoreModule_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFaceDet;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblYarpPort;
        private System.Windows.Forms.Label lblYarpServer;

    }
}


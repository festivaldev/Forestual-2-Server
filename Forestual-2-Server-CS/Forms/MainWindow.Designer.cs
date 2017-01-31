namespace Forestual2ServerCS.Forms
{
    partial class MainWindow
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.tbxInput = new System.Windows.Forms.TextBox();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.Panel5 = new System.Windows.Forms.Panel();
            this.btnSend = new System.Windows.Forms.Button();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblAssembly = new System.Windows.Forms.Label();
            this.lblServerAddress = new System.Windows.Forms.Label();
            this.Panel6 = new System.Windows.Forms.Panel();
            this.pnlAccounts = new System.Windows.Forms.Panel();
            this.tmrLoginTimeout = new System.Windows.Forms.Timer(this.components);
            this.Panel3 = new System.Windows.Forms.Panel();
            this.Panel5.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.Panel6.SuspendLayout();
            this.Panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbxInput
            // 
            this.tbxInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxInput.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tbxInput.Location = new System.Drawing.Point(7, 8);
            this.tbxInput.Name = "tbxInput";
            this.tbxInput.Size = new System.Drawing.Size(398, 25);
            this.tbxInput.TabIndex = 1;
            // 
            // btnStartServer
            // 
            this.btnStartServer.BackColor = System.Drawing.Color.White;
            this.btnStartServer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnStartServer.Location = new System.Drawing.Point(11, 11);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(100, 25);
            this.btnStartServer.TabIndex = 0;
            this.btnStartServer.Text = "Server starten";
            this.btnStartServer.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblStatus.Size = new System.Drawing.Size(111, 29);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Stopped";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Panel5
            // 
            this.Panel5.Controls.Add(this.tbxInput);
            this.Panel5.Controls.Add(this.btnSend);
            this.Panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel5.Location = new System.Drawing.Point(0, 447);
            this.Panel5.Name = "Panel5";
            this.Panel5.Size = new System.Drawing.Size(518, 40);
            this.Panel5.TabIndex = 3;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(411, 7);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(100, 27);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Senden";
            this.btnSend.UseVisualStyleBackColor = true;
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.btnStartServer);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel1.Location = new System.Drawing.Point(0, 0);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(868, 46);
            this.Panel1.TabIndex = 8;
            // 
            // rtbConsole
            // 
            this.rtbConsole.BackColor = System.Drawing.SystemColors.Control;
            this.rtbConsole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbConsole.Font = new System.Drawing.Font("Consolas", 10F);
            this.rtbConsole.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.rtbConsole.Location = new System.Drawing.Point(7, 0);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.ReadOnly = true;
            this.rtbConsole.Size = new System.Drawing.Size(504, 447);
            this.rtbConsole.TabIndex = 0;
            this.rtbConsole.Text = "";
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(53)))), ((int)(((byte)(57)))));
            this.pnlStatus.Controls.Add(this.lblAssembly);
            this.pnlStatus.Controls.Add(this.lblServerAddress);
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatus.Location = new System.Drawing.Point(0, 533);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(868, 29);
            this.pnlStatus.TabIndex = 11;
            // 
            // lblAssembly
            // 
            this.lblAssembly.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAssembly.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblAssembly.ForeColor = System.Drawing.Color.White;
            this.lblAssembly.Location = new System.Drawing.Point(111, 0);
            this.lblAssembly.Name = "lblAssembly";
            this.lblAssembly.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.lblAssembly.Size = new System.Drawing.Size(607, 29);
            this.lblAssembly.TabIndex = 2;
            this.lblAssembly.Text = "F2S Version 2.0.4rc 17w1 (7FC1FBD)";
            this.lblAssembly.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblServerAddress
            // 
            this.lblServerAddress.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblServerAddress.ForeColor = System.Drawing.Color.White;
            this.lblServerAddress.Location = new System.Drawing.Point(718, 0);
            this.lblServerAddress.Name = "lblServerAddress";
            this.lblServerAddress.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.lblServerAddress.Size = new System.Drawing.Size(150, 29);
            this.lblServerAddress.TabIndex = 1;
            this.lblServerAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Panel6
            // 
            this.Panel6.Controls.Add(this.rtbConsole);
            this.Panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel6.Location = new System.Drawing.Point(0, 0);
            this.Panel6.Name = "Panel6";
            this.Panel6.Padding = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.Panel6.Size = new System.Drawing.Size(518, 447);
            this.Panel6.TabIndex = 4;
            // 
            // pnlAccounts
            // 
            this.pnlAccounts.AutoScroll = true;
            this.pnlAccounts.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlAccounts.Location = new System.Drawing.Point(0, 46);
            this.pnlAccounts.Name = "pnlAccounts";
            this.pnlAccounts.Size = new System.Drawing.Size(350, 487);
            this.pnlAccounts.TabIndex = 9;
            // 
            // Panel3
            // 
            this.Panel3.Controls.Add(this.Panel6);
            this.Panel3.Controls.Add(this.Panel5);
            this.Panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel3.Location = new System.Drawing.Point(350, 46);
            this.Panel3.Name = "Panel3";
            this.Panel3.Size = new System.Drawing.Size(518, 487);
            this.Panel3.TabIndex = 10;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(868, 562);
            this.Controls.Add(this.Panel3);
            this.Controls.Add(this.pnlAccounts);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.pnlStatus);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Forestual 2 Server";
            this.Panel5.ResumeLayout(false);
            this.Panel5.PerformLayout();
            this.Panel1.ResumeLayout(false);
            this.pnlStatus.ResumeLayout(false);
            this.Panel6.ResumeLayout(false);
            this.Panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.TextBox tbxInput;
        internal System.Windows.Forms.Button btnStartServer;
        internal System.Windows.Forms.Label lblStatus;
        internal System.Windows.Forms.Panel Panel5;
        internal System.Windows.Forms.Button btnSend;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.RichTextBox rtbConsole;
        internal System.Windows.Forms.Panel pnlStatus;
        internal System.Windows.Forms.Label lblServerAddress;
        internal System.Windows.Forms.Panel Panel6;
        internal System.Windows.Forms.Panel pnlAccounts;
        internal System.Windows.Forms.Timer tmrLoginTimeout;
        internal System.Windows.Forms.Panel Panel3;
        internal System.Windows.Forms.Label lblAssembly;
    }
}


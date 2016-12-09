using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Forestual2CoreCS;
using Forestual2CoreCS.Management;
using Forestual2ServerCS.Internal;
using Forestual2ServerCS.Management;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Forestual2ServerCS
{
    public partial class MainWindow : Form
    {
        private delegate void DAppendText(string content);

        private delegate void DChangeColor(Color color);

        private delegate void DSetServerAddress(string address);

        private delegate void DDisplayForm(Form form);

        private delegate void DRefreshAccounts();

        private Server Server;

        public bool ConsoleLocked { get; set; }
        private bool ServerIsRunning;

        private int YCoordinate;

        private Storage.Localization.Values Lcl;

        public MainWindow() {
            InitializeComponent();
            Load += MainWindow_Load;
            Closing += MainWindow_Closing;
            btnStartServer.Click += BtnStartServer_Click;
            tbxInput.KeyDown += (sender, e) => {
                if (e.KeyCode == Keys.Enter) {
                    btnSend_Click(btnSend, new EventArgs());
                }
            };
            btnSend.Click += btnSend_Click;
        }

        private void btnSend_Click(object sender, EventArgs e) {
            // Extension Management
            ListenerManager.InvokeEvent(Event.ConsoleInputReceived, tbxInput.Text);
            //End

            tbxInput.Clear();
        }

        private void MainWindow_Load(object sender, EventArgs e) {
            // Start TimeoutTimer
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            StopServer();
            Application.Exit();
        }

        private void BtnStartServer_Click(object sender, EventArgs e) {
            if (!ServerIsRunning) {
                if (StartServer()) {
                    btnStartServer.Text = "Server stoppen";
                    SetTaskbarBackground(TaskbarProgressBarState.Normal);
                    DisplayAccounts();
                }
            } else {
                StopServer();
                btnStartServer.Text = "Server starten";
            }
        }

        private bool StartServer() {
            Server = new Server();
            ServerManagement.RegisterServer(Server);
            Server.Connected += Connected;
            Server.ConsoleColorChanged += ConsoleColorChanged;
            Server.ConsoleMessageReceived += ConsoleMessageReceived;
            Server.DisplayFormEvent += DisplayFormEvent;
            Server.RefreshAccounts += RefreshAccounts;
            ServerIsRunning = Server.Start();
            return ServerIsRunning;
        }

        public void StopServer() {
            try {
                Server.Stop();
            } catch { }
            ServerIsRunning = false;
            SetTaskbarBackground(TaskbarProgressBarState.Error);
        }

        private void ConsoleMessageReceived(string content, bool newLine = true) {
            if (newLine)
                content += Environment.NewLine;
            Invoke(new DAppendText(AppendText), content);
        }

        private void AppendText(string content) {
            rtbConsole.AppendText(content);
        }

        private void ConsoleColorChanged(Color color) {
            Invoke(new DChangeColor(ChangeColor), color);
        }

        private void ChangeColor(Color color) {
            rtbConsole.SelectionColor = color;
        }

        private void Connected(string address) {
            Invoke(new DSetServerAddress(SetServerAddress), address);
        }

        private void SetServerAddress(string address) {
            lblServerAddress.Text = address;
        }

        private void DisplayFormEvent(Form form) {
            Invoke(new DDisplayForm(DisplayForm), form);
        }

        private void DisplayForm(Form form) {
            form.Show();
        }

        public void SetTaskbarBackground(TaskbarProgressBarState color) {
            if (TaskbarManager.IsPlatformSupported) {
                TaskbarManager tbm = TaskbarManager.Instance;
                tbm.SetProgressValue(100, 100);
                tbm.SetProgressState(color);
            }
            switch (color) {
            case TaskbarProgressBarState.Normal:
                pnlStatus.BackColor = ColorTranslator.FromHtml("#07D159");
                lblStatus.Text = "Running";
                break;
            case TaskbarProgressBarState.Paused:
                pnlStatus.BackColor = ColorTranslator.FromHtml("#FF6600");
                lblStatus.Text = "Warning";
                break;
            case TaskbarProgressBarState.Error:
                pnlStatus.BackColor = ColorTranslator.FromHtml("#FC3539");
                lblStatus.Text = "Stopped";
                break;
            }
            Focus();
        }

        private void RefreshAccounts() {
            Invoke(new DRefreshAccounts(DisplayAccounts));
        }

        private void DisplayAccounts() {
            pnlAccounts.Controls.Clear();
            YCoordinate = 0;
            var Database = Server.Database;
            foreach (var Rank in Database.Ranks) {
                var Header = GetHeaderPanel(Rank.Name, ColorTranslator.FromHtml(Rank.Color));
                Header.Location = new Point(0, YCoordinate);
                YCoordinate += 20;
                pnlAccounts.Controls.Add(Header);
                var Accounts = Database.Accounts.FindAll(a => a.RankId == Rank.Id);
                foreach (var Account in Accounts) {
                    var Item = GetItemPanel(Account.Name, ColorTranslator.FromHtml(Rank.Color), Account.Online);
                    Item.Location = new Point(0, YCoordinate);
                    YCoordinate += 51;
                    pnlAccounts.Controls.Add(Item);
                }
                YCoordinate -= 1;
            }
        }

        private Panel GetHeaderPanel(string header, Color background) {
            var Panel = new Panel();
            Panel.Width = pnlAccounts.Width - 17;
            Panel.Height = 20;
            Panel.BackColor = background;
            var Label = new Label();
            Label.Text = header;
            Label.ForeColor = Color.White;
            Label.AutoSize = false;
            Label.TextAlign = ContentAlignment.MiddleCenter;
            Label.Dock = DockStyle.Fill;
            Panel.Controls.Add(Label);
            return Panel;
        }

        private Panel GetItemPanel(string accountName, Color background, bool online) {
            var Panel = new Panel();
            Panel.Width = pnlAccounts.Width - 17;
            Panel.Height = 50;
            Panel.BackColor = Color.Gainsboro;
            var Label = new Label();
            Label.ForeColor = Color.White;
            Label.Padding = new Padding(3);
            Label.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            Label.Text = accountName;
            Label.AutoSize = true;
            Label.BackColor = background;
            Label.Location = new Point(15, 15);
            Panel.Controls.Add(Label);
            var Status = new DoubleBufferedPanel();
            Status.Size = new Size(20, 20);
            Status.Location = new Point(Panel.Width - 35, 15);
            Status.State = online;
            Status.Paint += StatusElementPaint;
            Panel.Controls.Add(Status);
            return Panel;
        }

        private void StatusElementPaint(object sender, PaintEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            SolidBrush Brush;
            if (((DoubleBufferedPanel) sender).State) {
                Brush = new SolidBrush(ColorTranslator.FromHtml("#1ED760"));
            } else {
                Brush = new SolidBrush(Color.DimGray);
            }
            e.Graphics.FillEllipse(Brush, new RectangleF(0, 0, 19F, 19F));
        }
    }
}
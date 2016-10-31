using System;
using System.Drawing;
using System.Windows.Forms;
using Forestual2ServerCS.Internal;

namespace Forestual2ServerCS
{
    public partial class MainWindow : Form
    {
        private delegate void DAppendText(string content);
        private delegate void DChangeColor(Color color);
        private delegate void DSetServerAddress(string address);

        private Forestual2ServerCS.Internal.Server Server;

        public bool ConsoleLocked { get; set; }
        private bool ServerIsRunning;

        private Storage.Localization.Values Lcl;

        public MainWindow() {
            InitializeComponent();
            this.Load += MainWindow_Load;
            this.Closing += MainWindow_Closing;
            btnStartServer.Click += BtnStartServer_Click;
        }

        private void MainWindow_Load(object sender, System.EventArgs e) {
            // Start TimeoutTimer
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            StopServer();
            Application.Exit();
        }

        private void BtnStartServer_Click(object sender, EventArgs e) {
            if (!ServerIsRunning) {
                btnStartServer.Text = StartServer() ? "Server stoppen" : "Server starten";
            } else {
                StopServer();
                btnStartServer.Text = "Server starten";
            }
        }

        private bool StartServer() {
            Server = new Server();
            Server.Connected += Connected;
            Server.ConsoleColorChanged += ConsoleColorChanged;
            Server.ConsoleMessageReceived += ConsoleMessageReceived;
            ServerIsRunning = Server.Start();
            return ServerIsRunning;
        }

        public void StopServer() {
            try {
                Server.Stop();
            } catch { }
            ServerIsRunning = false;
        }

        private void ConsoleMessageReceived(string content, bool newLine = true) {
            if (newLine)
                content += Environment.NewLine;
            this.Invoke(new DAppendText(AppendText), content);
        }

        private void AppendText(string content) {
            rtbConsole.AppendText(content);
        }

        private void ConsoleColorChanged(Color color) {
            this.Invoke(new DChangeColor(ChangeColor), color);
        }

        private void ChangeColor(Color color) {
            rtbConsole.SelectionColor = color;
        }

        private void Connected(string address) {
            this.Invoke(new DSetServerAddress(SetServerAddress), address);
        }

        private void SetServerAddress(string address) {
            lblServerAddress.Text = address;
        }
    }
}

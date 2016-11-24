using System;
using System.Drawing;
using System.Windows.Forms;
using Forestual2CoreCS;
using Forestual2CoreCS.Management;
using Forestual2ServerCS.Internal;
using Forestual2ServerCS.Management;

namespace Forestual2ServerCS
{
    public partial class MainWindow : Form
    {
        private delegate void DAppendText(string content);
        private delegate void DChangeColor(Color color);
        private delegate void DSetServerAddress(string address);

        private Server Server;

        public bool ConsoleLocked { get; set; }
        private bool ServerIsRunning;

        private Storage.Localization.Values Lcl;

        public MainWindow() {
            InitializeComponent();
            Load += MainWindow_Load;
            Closing += MainWindow_Closing;
            btnStartServer.Click += BtnStartServer_Click;
            tbxInput .KeyDown += (sender, e) => {
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
                btnStartServer.Text = StartServer() ? "Server stoppen" : "Server starten";
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
    }
}

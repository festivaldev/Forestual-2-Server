using System.Windows.Forms;
using Forestual2Core;
using F2CE = Forestual2Core.Enumerations;

namespace Forestual2ServerCS
{
    public partial class LoginWindow : Form
    {
        private Storage.Localization.Values Lcl;

        public bool IsTimeoutLogin;

        public LoginWindow() {
            InitializeComponent();
            this.Load += LoginWindow_Load;
            this.Shown += LoginWindow_Shown;
            this.Closing += LoginWindow_Closing;
            btnLogin.Click += BtnLogin_Click;
        }

        private void BtnLogin_Click(object sender, System.EventArgs e) {
            var Database = Storage.Database.Helper.GetDatabase();
            if (Storage.Database.Helper.AccountExists(Database, tbxAccountId.Text)) {
                var Account = Storage.Database.Helper.GetAccount(Database, tbxAccountId.Text);
                if (Account.Password == Cryptography.ComputeHash(tbxPassword.Text)) {
                    if (Storage.Database.Helper.AccountHasFlags(Database, Account, F2CE.Flag.CanControlServer)) {
                        if (IsTimeoutLogin) {
                            ((MainWindow) Owner).ConsoleLocked = false;
                            ((MainWindow) Owner).tmrLoginTimeout.Start();
                            IsTimeoutLogin = false;
                            this.Close();
                        } else {
                            MainWindow MWindow = new MainWindow();
                            MWindow.Show();
                            this.Hide();
                        }
                    } else {
                        MessageBox.Show("Not Allowed", "Forestual 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } else {
                    MessageBox.Show("Password Incorrect", "Forestual 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                MessageBox.Show("Account Unknown", "Forestual 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (IsTimeoutLogin) {
                e.Cancel = true;
                if (MessageBox.Show("Do you really want to close?", "Forestual 2", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    ((MainWindow) Owner).StopServer();
                    Application.Exit();
                }
            }
        }

        private void LoginWindow_Shown(object sender, System.EventArgs e) {
            if (!Storage.Configuration.Helper.GetConfig().ConsoleRequiresAuthentification) {
                MainWindow MWindow = new MainWindow();
                MWindow.Show();
                this.Hide();
            }
        }

        private void LoginWindow_Load(object sender, System.EventArgs e) {
            if (!Storage.Configuration.Helper.Exists())
                Storage.Configuration.Helper.CreateDefault();
            if (!Storage.Database.Helper.Exists())
                Storage.Database.Helper.CreateDefault();
        }

        public void ShowTimeoutDialog(Form owner) {
            IsTimeoutLogin = true;
            this.ShowDialog(owner);
        }
    }
}

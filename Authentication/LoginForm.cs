using Owmeta.Authentication;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Owmeta
{
    public partial class LoginForm : Form
    {
        private readonly KeycloakAuth keycloakAuth;

        public LoginForm()
        {
            InitializeComponent();

            string keycloakUrl = "https://id.owmeta.io";
            string realm = "owmeta";

            keycloakAuth = new KeycloakAuth(keycloakUrl, realm);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            try
            {
                var tokenResponse = await keycloakAuth.Authenticate(username, password);

                File.WriteAllText("owmeta_oauth_token.json", System.Text.Json.JsonSerializer.Serialize(tokenResponse));

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}\n\nPlease make sure you're using the correct credentials for your OWMETA account.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://owmeta.io/signup") { UseShellExecute = true });
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '•';
        }
    }
}
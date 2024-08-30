using Owcounter.Authentication;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Owcounter
{
    public partial class LoginForm : Form
    {
        private readonly KeycloakAuth keycloakAuth;

        public LoginForm()
        {
            InitializeComponent();

            string keycloakUrl = "https://id.owcounter.com";
            string realm = "owcounter";

            keycloakAuth = new KeycloakAuth(keycloakUrl, realm);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            try
            {
                var tokenResponse = await keycloakAuth.Authenticate(username, password);

                File.WriteAllText("owcounter_oauth_token.json", System.Text.Json.JsonSerializer.Serialize(tokenResponse));

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}\n\nPlease make sure you're using the correct credentials for your OWCOUNTER account.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://owcounter.com/signup") { UseShellExecute = true });
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '•';
        }
    }
}
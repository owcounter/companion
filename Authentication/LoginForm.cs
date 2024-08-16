using System;
using System.IO;
using System.Windows.Forms;
using Owcounter.Authentication;

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
                MessageBox.Show($"Login failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
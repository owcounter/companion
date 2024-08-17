using Owcounter.Authentication;
using Owcounter.Services;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Owcounter
{
    public class TrayApplicationContext : ApplicationContext
    {
        private readonly TrayIcon trayIcon;
        private readonly ApiService apiService;
        private readonly ScreenshotMonitoringService monitoringService;
        private readonly KeycloakAuth keycloakAuth;

        private const string ApiBaseUrl = "https://api.owcounter.com";
        private const string KeycloakUrl = "https://id.owcounter.com";
        private const string Realm = "owcounter";
        private const string TokenFileName = "owcounter_oauth_token.json";

        public TrayApplicationContext()
        {
            keycloakAuth = new KeycloakAuth(KeycloakUrl, Realm);
            apiService = new ApiService(ApiBaseUrl, keycloakAuth, TokenFileName);
            monitoringService = new ScreenshotMonitoringService(apiService);
            trayIcon = new TrayIcon();

            InitializeApplication();
        }

        private async void InitializeApplication()
        {
            trayIcon.OnLogout += (s, e) => Logout();
            trayIcon.OnExit += (s, e) => Exit();

            if (await apiService.LoadAndValidateTokens())
            {
                StartMonitoring();
            }
            else
            {
                ShowLoginForm();
            }
        }

        private void StartMonitoring()
        {
            monitoringService.StartMonitoring();
            Logger.Log("Started monitoring Overwatch screenshots folder.");
        }

        private void ShowLoginForm()
        {
            var loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                Task.Run(async () =>
                {
                    if (await apiService.LoadAndValidateTokens())
                    {
                        StartMonitoring();
                    }
                    else
                    {
                        MessageBox.Show("Login failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ShowLoginForm();
                    }
                });
            }
            else
            {
                Application.Exit();
            }
        }

        private async void Logout()
        {
            await apiService.Logout();
            monitoringService.Dispose();
            ShowLoginForm();
        }

        private void Exit()
        {
            Application.Exit();
        }
    }
}
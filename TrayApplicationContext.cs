using Owcounter.Authentication;
using Owcounter.Services;
using Owcounter.UI;
using System;
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
        private readonly LogWindow logWindow;

        private const string ApiBaseUrl = "https://api.owcounter.com";
        private const string KeycloakUrl = "https://id.owcounter.com";
        private const string Realm = "owcounter";
        private const string TokenFileName = "owcounter_oauth_token.json";

        public TrayApplicationContext()
        {
            logWindow = new LogWindow();
            keycloakAuth = new KeycloakAuth(KeycloakUrl, Realm);
            apiService = new ApiService(ApiBaseUrl, keycloakAuth, TokenFileName, Log);
            monitoringService = new ScreenshotMonitoringService(apiService);
            trayIcon = new TrayIcon();

            InitializeApplication();
        }

        private async void InitializeApplication()
        {
            trayIcon.OnOpenLog += (s, e) => OpenLog();
            trayIcon.OnLogout += (s, e) => Logout();
            trayIcon.OnExit += (s, e) => Exit();

            monitoringService.OnLog += Log;

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
            Log("Started monitoring Overwatch screenshots folder.");
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
                        Log("Login failed. Please try again.");
                        ShowLoginForm();
                    }
                });
            }
            else
            {
                Application.Exit();
            }
        }

        private void OpenLog()
        {
            if (!logWindow.Visible)
            {
                logWindow.Show();
            }
            else
            {
                logWindow.BringToFront();
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

        private void Log(string message)
        {
            if (logWindow.InvokeRequired)
            {
                logWindow.Invoke(new Action<string>(Log), message);
            }
            else
            {
                logWindow.AddLog(message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                trayIcon.Dispose();
                monitoringService.Dispose();
                logWindow.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
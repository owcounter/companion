using Owmeta.Authentication;
using Owmeta.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Owmeta
{
    public class TrayApplicationContext : ApplicationContext
    {
        private readonly TrayIcon trayIcon;
        private readonly ApiService apiService;
        private readonly ScreenshotMonitoringService monitoringService;
        private readonly KeycloakAuth keycloakAuth;

        private const string ApiBaseUrl = "https://api.owmeta.io";
        private const string KeycloakUrl = "https://id.owmeta.io";
        private const string Realm = "owmeta";
        private const string TokenFileName = "owmeta_oauth_token.json";
        private const string DashboardUrl = "https://owmeta.io/dashboard";

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
            trayIcon.OnOpenDashboard += OpenDashboard;
            trayIcon.OnOpenLog += OpenLog;
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
            trayIcon.SetToolTip("OWMETA Companion - Monitoring");
            trayIcon.ShowNotification("OWMETA Companion", "Monitoring started. Open your dashboard for real-time insights!");
        }

        private void ShowLoginForm()
        {
            var loginForm = new LoginForm();
            DialogResult result = loginForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                Task.Run(async () =>
                {
                    if (await apiService.LoadAndValidateTokens())
                    {
                        ShowWelcomeMessage();
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
                // User closed the form without logging in
                Exit();
            }
        }

        private void ShowWelcomeMessage()
        {
            MessageBox.Show(
                "Welcome to OWMETA Companion!\n\n" +
                "The app is now running in the background and will automatically upload screenshots.\n" +
                "Open your OWMETA dashboard to view real-time insights during gameplay.\n\n" +
                "Right-click the tray icon for more options.",
                "OWMETA Companion",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private async void Logout()
        {
            await apiService.Logout();
            monitoringService.Dispose();
            trayIcon.SetToolTip("OWMETA Companion - Not logged in");
            trayIcon.ShowNotification("OWMETA Companion", "Logged out successfully.");
            ShowLoginForm();
        }

        private void Exit()
        {
            trayIcon.Dispose();
            Environment.Exit(0);
        }

        private void OpenLog(object sender, EventArgs e)
        {
            string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OwmetaCompanion.log");
            if (System.IO.File.Exists(logPath))
            {
                Process.Start(new ProcessStartInfo(logPath) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Log file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenDashboard(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo(DashboardUrl) { UseShellExecute = true });
        }
    }
}
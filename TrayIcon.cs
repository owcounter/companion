using System;
using System.Drawing;
using System.Windows.Forms;

namespace Owcounter
{
    public class TrayIcon : IDisposable
    {
        private readonly NotifyIcon trayIcon;
        public event EventHandler? OnOpenLog;
        public event EventHandler? OnOpenDashboard;
        public event EventHandler? OnLogout;
        public event EventHandler? OnExit;

        public TrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                Icon = new Icon(System.IO.Path.Combine(Application.StartupPath, "OWCounterCompanion.ico")),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };

            InitializeContextMenu();
        }

        private void InitializeContextMenu()
        {
            if (trayIcon.ContextMenuStrip != null)
            {
                trayIcon.ContextMenuStrip.Items.Add("Open Dashboard", null, (sender, e) => OnOpenDashboard?.Invoke(this, EventArgs.Empty));
                trayIcon.ContextMenuStrip.Items.Add("Open Log", null, (sender, e) => OnOpenLog?.Invoke(this, EventArgs.Empty));
                trayIcon.ContextMenuStrip.Items.Add("Logout", null, (sender, e) => OnLogout?.Invoke(this, EventArgs.Empty));
                trayIcon.ContextMenuStrip.Items.Add("Exit", null, (sender, e) => OnExit?.Invoke(this, EventArgs.Empty));
            }
        }

        public void ShowNotification(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            trayIcon.ShowBalloonTip(3000, title, message, icon);
        }

        public void SetToolTip(string message)
        {
            trayIcon.Text = message;
        }

        public void Dispose()
        {
            trayIcon.Dispose();
        }
    }
}
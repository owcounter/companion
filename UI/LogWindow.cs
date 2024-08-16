using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Owcounter
{
    public partial class LogWindow : Form
    {
        private readonly string logFilePath;
        private System.Windows.Forms.Timer updateTimer;

        public LogWindow(string logFilePath)
        {
            InitializeComponent();
            DisplayVersion();
            this.logFilePath = logFilePath;

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 1000; // Update every second
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }

        private void DisplayVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string versionString = $"v{version.Major}.{version.Minor}.{version.Build}";
            this.Text = $"OwcounterCompanion Log - {versionString}";
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (File.Exists(logFilePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(logFilePath);
                    logTextBox.Lines = lines;
                    logTextBox.SelectionStart = logTextBox.Text.Length;
                    logTextBox.ScrollToCaret();
                }
                catch (IOException)
                {
                    // File might be locked, we'll try again on the next tick
                }
            }
        }

        private void LogWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}

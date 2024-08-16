using System;
using System.Reflection;
using System.Windows.Forms;

namespace Owcounter
{
    public partial class LogWindow : Form
    {
        public LogWindow()
        {
            InitializeComponent();
            DisplayVersion();
        }

        private void DisplayVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string versionString = $"v{version.Major}.{version.Minor}.{version.Build}";
            this.Text = $"OwcounterCompanion Log - {versionString}";
        }

        public void AddLog(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AddLog), message);
                return;
            }

            logTextBox.AppendText($"[{DateTime.Now}] {message}{Environment.NewLine}");
            logTextBox.ScrollToCaret();
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

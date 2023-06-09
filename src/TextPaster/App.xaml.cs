using System;
using System.Drawing;
using System.Windows;

namespace TextPaster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly System.Windows.Forms.NotifyIcon notifyIcon;

        public App()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new Window();

            notifyIcon.Icon = new System.Drawing.Icon(@".\icon.ico");
            notifyIcon.Text = "Text Paster";
            notifyIcon.Visible = true;
            notifyIcon.MouseDown += NotifyIcon_MouseDown; ;

            notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Dashboard", Image.FromFile(@".\icon.ico"), OnStart);
            notifyIcon.ContextMenuStrip.Items.Add("Exit", Image.FromFile(@".\icon.ico"), OnExit);
        }

        private void NotifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MainWindow.WindowState = WindowState.Normal;
                MainWindow.Activate();
                MainWindow.Show();
            }
        }

        private void OnStart(object sender, EventArgs e)
        {
            MainWindow.Activate();
            MainWindow.Show();
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

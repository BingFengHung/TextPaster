using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TextPaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IntPtr windowHandle;
        GlobalKeyboardHook hook;

        public MainWindow()
        {
            Sql sql = new Sql();
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            hook = new GlobalKeyboardHook();

            string[] text = new string[]
            {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10",
                "11",
                "12",
            };
            //AutoTyper auto = new AutoTyper(text);
            //this.ShowMinimizeAndMaximizeButtons();
            // this.HideMinimizeAndMaximizeButtons();
            this.SourceInitialized += MainWindow_SourceInitialized;
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            windowHandle = new WindowInteropHelper(this).Handle;

            HideMinimizeAndMaximizeButtons();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;

            this.Hide();
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;

        private const int WS_MAXIMIZEBOX = 0x10000; //maximize button
        private const int WS_MINIMIZEBOX = 0x20000; //minimize button


        private const int WS_SYSMENU = 0x80000;
        protected void HideAllButtons()
        {
            if (windowHandle == null)
                throw new InvalidOperationException("The window has not yet been completely initialized");

            SetWindowLong(windowHandle, GWL_STYLE, GetWindowLong(windowHandle, GWL_STYLE) & ~WS_SYSMENU);
        }

        protected void HideMinimizeAndMaximizeButtons()
        {
            if (windowHandle == null)
                throw new InvalidOperationException("The window has not yet been completely initialized");

            SetWindowLong(windowHandle, GWL_STYLE, GetWindowLong(windowHandle, GWL_STYLE) & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX);
        }

        protected void EnableMinimizeButton()
        {
            if (windowHandle == IntPtr.Zero)
                throw new InvalidOperationException("The window has not yet been completely initialized");

            SetWindowLong(windowHandle, GWL_STYLE, GetWindowLong(windowHandle, GWL_STYLE) | WS_MINIMIZEBOX);
        }

        protected void EnableMaximizeButton()
        {
            if (windowHandle == null)
                throw new InvalidOperationException("The window has not yet been completely initialized");

            SetWindowLong(windowHandle, GWL_STYLE, GetWindowLong(windowHandle, GWL_STYLE) | WS_MAXIMIZEBOX);
        }

        protected void ShowMinimizeAndMaximizeButtons()
        {
            if (windowHandle == null)
                throw new InvalidOperationException("The window has not yet been completely initialized");

            SetWindowLong(windowHandle, GWL_STYLE, GetWindowLong(windowHandle, GWL_STYLE) | WS_MAXIMIZEBOX | WS_MINIMIZEBOX);
        }

        protected void ShowAllButtons()
        {
            if (windowHandle == null)
                throw new InvalidOperationException("The window has not yet been completely initialized");

            SetWindowLong(windowHandle, GWL_STYLE, GetWindowLong(windowHandle, GWL_STYLE) | WS_SYSMENU);
        }

    }
}

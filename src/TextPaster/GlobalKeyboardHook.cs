using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextPaster
{
    internal class GlobalKeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;
        private LowLevelKeyboardProc proc;
        private IntPtr hookId = IntPtr.Zero;
        private bool _tmpIntercept = true;

        private bool _ctrlPressed = false;
        private bool _altPressed = false;
        private bool _shiftPressed = false;

        public GlobalKeyboardHook()
        {
            proc = new LowLevelKeyboardProc(HookCallback);
            hookId = SetHook(proc);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(hookId);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private bool _replaceText = false;
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (_tmpIntercept && nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                // Debug($"wParam = {wParam}, lParam = {lParam}, vkCode = {vkCode}");
                //if (wParam == (IntPtr)WM_SYSKEYUP)
                //{
                //    switch (vkCode)
                //    {
                //        case 164:
                //            _altPressed = false;
                //            break;
                //    }
                //}

                //if (wParam == (IntPtr)WM_SYSKEYDOWN)
                //{
                //    switch (vkCode)
                //    {
                //        case 164:
                //            _altPressed = true;
                //            break;
                //    }
                //}

                if (wParam == (IntPtr)WM_KEYUP)
                {
                    switch ((Keys)vkCode)
                    {
                        case Keys.Alt:
                            _altPressed = false;
                            break;

                        case Keys.LShiftKey:
                        case Keys.RShiftKey:
                            _shiftPressed = false;
                            break;

                        case Keys.LControlKey:
                        case Keys.RControlKey:
                            _ctrlPressed = false;
                            break;

                        case Keys.LWin:
                        case Keys.RWin:
                            //_windowsPressed = false;
                            break;
                    }
                }
                else if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    switch ((Keys)vkCode)
                    {
                        case Keys.LShiftKey:
                        case Keys.RShiftKey:
                            _shiftPressed = true;
                            break;

                        case Keys.LControlKey:
                        case Keys.RControlKey:
                            _ctrlPressed = true;
                            break;

                        case Keys.LWin:
                        case Keys.RWin:
                            //_windowsPressed = true;
                            break;

                        case Keys.Escape:
                            break;

                        case Keys.F1:
                        case Keys.F2:
                        case Keys.F3:
                        case Keys.F4:
                        case Keys.F5:
                        case Keys.F6:
                        case Keys.F7:
                        case Keys.F8:
                        case Keys.F9:
                        case Keys.F10:
                        case Keys.F11:
                        case Keys.F12:
                        case Keys.D1:
                        case Keys.D2:
                        case Keys.D3:
                        case Keys.D4:
                        case Keys.D5:
                        case Keys.D6:
                        case Keys.D7:
                        case Keys.D8:
                        case Keys.D9:
                            // if (_ctrlPressed && _shiftPressed)
                            if (_ctrlPressed)
                            {
                                _tmpIntercept = true;
                                _replaceText = true;
                                TypeWork(vkCode - 0x30);
                            }
                            break;

                        default:

                            break;
                    }
                }


            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        Sql sql = new Sql();
        private void TypeWork(int value)
        {
            object lockObj = new object();
            sql.SelectSingle(value.ToString(), out KeyboardMap model);

            string text = model.Text;

            if (string.IsNullOrEmpty(text)) return;

            Task.Factory.StartNew(() =>
            {
                if (_replaceText)
                {
                    string firstKey = FormatText(text[0]);
                    _tmpIntercept = false;
                    System.Threading.SpinWait.SpinUntil(() => false, 200);
                    System.Windows.Forms.SendKeys.SendWait(firstKey);


                    _tmpIntercept = true;
                    for (int i = 1; i < text.Length; i++)
                    {
                        string keys = FormatText(text[i]);

                        System.Threading.SpinWait.SpinUntil(() => false, 50);
                        _tmpIntercept = false;
                        System.Windows.Forms.SendKeys.SendWait(keys);
                        //SendKeys.Send(System.Windows.Input.Key.V);
                        _tmpIntercept = true;
                    }

                    _replaceText = false;
                }
            });
        }

        private string FormatText(char code)
        {
            string keys;
            switch (code)
            {
                case '{':
                case '}':
                case '(':
                case ')':
                case '%':
                case '+':
                case '^':
                case '~':
                    keys = string.Format("{{{0}}}", code);
                    break;

                default:
                    keys = code.ToString();//AutoTypedText[_textIndex][_replIndex].ToString();
                    break;
            }

            return keys;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hk, int ncode, IntPtr wparam, IntPtr lparam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string l);
    }
}

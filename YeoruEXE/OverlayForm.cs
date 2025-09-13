using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace YeoruEXE
{
    public partial class OverlayForm : Form
    {
        private NamedPipeServerStream pipeServer = null!;
        private Timer positionCheckTimer;

        public OverlayForm()
        {
            InitializeComponent();

            Task.Run(() => StartPipeServer());

            // this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(300, 300);
            this.Size = new Size(800, 600);
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.Opacity = 0.6;
            this.FormClosing += new FormClosingEventHandler(OverlayForm_FormClosing);

            positionCheckTimer = new Timer();
            positionCheckTimer.Interval = 100; // 100ms 마다 체크
            // positionCheckTimer.Tick += CheckPosition;
            positionCheckTimer.Start();
        }

        private void StartPipeServer()
        {
            using (pipeServer = new NamedPipeServerStream("UnityPipe", PipeDirection.InOut))
            {
                pipeServer.WaitForConnection();

                using (StreamReader sr = new StreamReader(pipeServer))
                {
                    while (pipeServer.IsConnected)
                    {
                        try
                        {
                            string message = sr.ReadLine();
                            if (message == null) break;

                            this.Invoke((MethodInvoker)delegate {
                                // 메시지 처리 로직 (예: MessageBox.Show, Label.Text 변경 등)
                                MessageBox.Show($"Unity에서 받은 메시지: {message}");
                            });
                        }
                        catch (IOException)
                        {
                            break;
                        }
                    }
                }
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private void CheckPosition(object? sender, EventArgs e)
        {
            IntPtr hwnd = FindWindow(null, "Project_Yeoru - TestScene - Windows, Mac, Linux - Unity 2022.3.59f1 <DX11>"); // 에디터면 "Unity Editor"
            if (hwnd == IntPtr.Zero)
                return;

            if (GetWindowRect(hwnd, out RECT rect))
            {
                Rectangle unityWindow = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                Rectangle overlayRect = new Rectangle(this.Location, this.Size);

                if (unityWindow.IntersectsWith(overlayRect))
                {
                    File.WriteAllText("event_signal.txt", "trigger");
                }
                else
                {
                    File.WriteAllText("event_signal.txt", "none");
                }
            }
        }

        private void OverlayForm_Load(object sender, EventArgs e)
        {

        }

        // 프로그램 끌 때 none 설정함
        private void OverlayForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            File.WriteAllText("event_signal.txt", "none");
        }
    }
}

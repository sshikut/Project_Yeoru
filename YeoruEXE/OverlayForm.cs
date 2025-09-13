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

            this.Load += (s, e) => Task.Run(() => StartPipeServerAsync());

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

        private async Task StartPipeServerAsync()
        {
            using (pipeServer = new NamedPipeServerStream("UnityPipe", PipeDirection.InOut))
            {
                pipeServer.WaitForConnection();

                using (StreamReader sr = new StreamReader(pipeServer))
                {
                    while (pipeServer.IsConnected)
                    {
                        var jsonMessage = await sr.ReadLineAsync();
                        if (jsonMessage == null) break;

                        try
                        {
                            PipeMessage ?receivedMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<PipeMessage>(jsonMessage);

                            this.Invoke((MethodInvoker)delegate {
                                MessageBox.Show($"Unity에서 받은 메시지: {jsonMessage}");

                                switch (receivedMessage.command) 
                                {
                                    case "test_Hole":
                                        MessageBox.Show($"Unity에서 받은 메시지: {jsonMessage}");
                                        break;

                                    default:
                                        MessageBox.Show($"이건 뭐임? : {jsonMessage}");
                                        break;
                                }

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

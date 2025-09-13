using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YeoruEXE
{
    public partial class OverlayForm : Form
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Timer positionCheckTimer;

        public OverlayForm()
        {
            InitializeComponent();

            this.Load += (s, e) => Task.Run(() => StartPipeServerAsync(cts.Token));

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

        private async Task StartPipeServerAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using (var pipeServer = new NamedPipeServerStream("UnityPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                {
                    try
                    {
                        await pipeServer.WaitForConnectionAsync(token);

                        using (var sr = new StreamReader(pipeServer))
                        {
                            while (pipeServer.IsConnected && !token.IsCancellationRequested)
                            {
                                var jsonMessage = await sr.ReadLineAsync();
                                if (string.IsNullOrWhiteSpace(jsonMessage)) break;

                                try
                                {
                                    PipeMessage? receivedMessage = JsonConvert.DeserializeObject<PipeMessage>(jsonMessage);

                                    if (receivedMessage != null)
                                    {
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            switch (receivedMessage.command)
                                            {
                                                case "Command":
                                                    MessageBox.Show($"Unity에서 받은 메시지: {jsonMessage}");
                                                    break;

                                                case "test_Hole":
                                                    LoadImageFromResource();
                                                    MessageBox.Show($"테스트: {jsonMessage}");
                                                    break;

                                                default:
                                                    MessageBox.Show($"이건 뭐임? : {jsonMessage}");
                                                    break;
                                            }
                                        });
                                    }

                                }
                                catch (JsonException ex)
                                {
                                    Console.WriteLine($"JSON Error: {ex.Message}");
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // 프로그램 종료 시 정상적으로 발생
                        break;
                    }
                    catch (Exception ex)
                    {
                        // 예기치 않은 오류 발생 시 로그 기록 (서버는 계속 실행)
                        Console.WriteLine($"Pipe server error: {ex.Message}");
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
            cts.Cancel();
            cts.Dispose();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void LoadImageFromFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Image = Image.FromFile(@"C:\MyImages\picture.png");
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("이미지 파일이 지정된 경로에 존재하지 않습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("이미지를 불러오는 중 오류가 발생했습니다: " + ex.Message);
            }
        }

        private void LoadImageFromResource()
        {
            pictureBox1.Image = Properties.Resources.ItemTestImage;
        }
    }
}

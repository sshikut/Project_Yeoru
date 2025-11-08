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
        private const int WM_NCHITTEST = 0x84;
        private const int HTCAPTION = 0x2;

        private CancellationTokenSource cts = new CancellationTokenSource();
        private Timer positionCheckTimer;

        private StreamReader? sr;
        private StreamWriter? writer;

        public OverlayForm()
        {
            InitializeComponent();

            this.Load += (s, e) => Task.Run(() => StartPipeServerAsync(cts.Token));

            this.Activated += new EventHandler(OverlayForm_Activated);
            this.Deactivate += new EventHandler(OverlayForm_Deactivate);

            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(300, 300);
            this.Size = new Size(800, 600);
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.Opacity = 0.6;
            this.FormClosing += new FormClosingEventHandler(OverlayForm_FormClosing);

            positionCheckTimer = new Timer();
            positionCheckTimer.Interval = 100; // 100ms 마다 체크
            // positionCheckTimer.Tick += CheckPosition;
            positionCheckTimer.Start();

            this.LocationChanged += new EventHandler(OnLocationChanged);
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

                        sr = new StreamReader(pipeServer);
                        writer = new StreamWriter(pipeServer);
                        writer.AutoFlush = true;

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

                                            case "test_XYCorrect":
                                                LoadImageFromResourceChangsub();
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
                    finally
                    {
                        sr?.Dispose();
                        writer?.Dispose();
                        sr = null;
                        writer = null;
                    }
                }
            }

        }

        public void SendMessageToUnity(string command, string value)
        {
            // writer가 null이면 (연결 전이거나 이미 끊긴 후) 아무것도 하지 않음
            if (writer == null)
            {
                return;
            }

            try
            {
                // 1. PipeMessage 객체 생성 및 JSON 변환
                var message = new PipeMessage
                {
                    command = command,
                    value = value
                };
                string jsonMessage = JsonConvert.SerializeObject(message);

                // 2. 메시지 전송 (AutoFlush=true이므로 Flush() 불필요)
                writer.WriteLine(jsonMessage);
            }
            catch (ObjectDisposedException)
            {
                // 클라이언트 연결이 끊어진 상태에서 보내려고 할 때 발생 (무시)
            }
            catch (IOException)
            {
                // 메시지 전송 중 연결이 끊겼을 때 발생 (무시)
            }
            catch (Exception ex)
            {
                // 기타 예외
                Console.WriteLine($"SendMessageToUnity Error: {ex.Message}");
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

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

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

        private void LoadImageFromResource()
        {
            pictureBox1.Image = Properties.Resources.ItemTestImage;
        }

        private void OverlayForm_Activated(object? sender, EventArgs e)
        {
            this.TopMost = true;
            this.TransparencyKey = Color.Gray;
            this.Opacity = 0.4;
        }

        private void OverlayForm_Deactivate(object? sender, EventArgs e)
        {
            this.TopMost = true;
            this.TransparencyKey = Color.Black;
            this.Opacity = 0.2;
        }

        protected override void WndProc(ref Message m)
        {
            // 마우스 관련 메시지(WM_NCHITTEST)인지 확인
            if (m.Msg == WM_NCHITTEST)
            {
                // 메시지 결과를 HTCAPTION으로 설정하여
                // 창의 클라이언트 영역(내용 부분)이 제목 표시줄처럼 동작하게 만듭니다.
                m.Result = (IntPtr)HTCAPTION;
                return; // 메시지 처리를 여기서 끝냅니다.
            }

            // 다른 메시지들은 기본 처리 방식에 맡깁니다.
            base.WndProc(ref m);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void OnLocationChanged(object sender, EventArgs e)
        {
            SendKeyPosition();
        }

        private void SendKeyPosition()
        {
            IntPtr hwnd = FindWindow(null, "Project_Yeoru - TestScene - Windows, Mac, Linux - Unity 2022.3.59f1 <DX11>");
            if (hwnd == IntPtr.Zero)
            {
                // Unity 창을 못 찾았으면 전송 중지
                return;
            }

            GetClientRect(hwnd, out RECT unityRect);
            int unityClientHeight = unityRect.Bottom - unityRect.Top;

            // 2. Unity 창의 '클라이언트 영역' (테두리 제외)의 시작점을 찾습니다.
            Point unityClientOrigin = new Point(0, 0);
            ClientToScreen(hwnd, ref unityClientOrigin);
            // 'unityClientOrigin'은 이제 Unity 창의 렌더링 영역의 
            // '절대 화면 좌표' (예: (800, 200))를 가집니다.

            // 3. '열쇠 문양'(pictureBoxKey)의 '절대 화면 좌표'를 계산합니다.
            Point keyImageScreenPos = this.PointToScreen(pictureBox2.Location);

            // 4. (가장 중요) '절대 좌표'를 'Unity 창 기준 상대 좌표'로 변환합니다.
            int relativeX = keyImageScreenPos.X - unityClientOrigin.X;
            int relativeY_from_top = keyImageScreenPos.Y - unityClientOrigin.Y;
            int relativeY_from_bottom = unityClientHeight - relativeY_from_top;

            // 5. PipeMessage 객체 생성 및 JSON 변환
            var message = new PipeMessage
            {
                command = "update_key_pos",
                value = $"{relativeX},{relativeY_from_bottom}" // '상대 좌표'를 보냅니다.
            };
            string jsonMessage = JsonConvert.SerializeObject(message);

            // 6. Unity 클라이언트로 메시지 전송
            SendMessageToUnity(message.command, message.value);
        }

        private void pictureBox3_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {

        }

        private void LoadImageFromResourceChangsub()
        {
            pictureBox3.Image = Properties.Resources.image_removebg_preview;
        }
    }
}

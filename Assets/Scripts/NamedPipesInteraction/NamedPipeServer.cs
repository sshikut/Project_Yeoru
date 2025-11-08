using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.IO.Pipes;
using System.IO;
using System;
using Newtonsoft.Json;

public class NamedPipeServer : MonoBehaviour
{
    public static NamedPipeServer Instance;

    private NamedPipeClientStream pipeClient;
    private StreamWriter writer;
    private StreamReader reader;

    [Header("퍼즐 설정")]
    [Tooltip("정답으로 판정할 문양이 있는 게임 오브젝트")]
    public GameObject targetObject;

    [Tooltip("정답으로 인정할 오차 범위 (픽셀 단위)")]
    public float tolerance = 10.0f;

    private Vector2 targetScreenPosition; // '정답 문양'의 화면 좌표
    [SerializeField] private bool isSolved = false;

    private float _pointTime = 1.0f; //1초마다 실행
    private float _nextTime = 0.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        targetScreenPosition = Camera.main.WorldToScreenPoint(targetObject.transform.position);
        ConnectToPipeAsync();
    }

    private void Update()
    {
        targetScreenPosition = Camera.main.WorldToScreenPoint(targetObject.transform.position);
        
    }

    void FixedUpdate()
    {
        if (Time.time > _nextTime)
        {
            _nextTime = Time.time + _pointTime; //다음번 실행할 시간
            Debug.Log(targetScreenPosition);
        }
    }

    private async void ConnectToPipeAsync()
    {
        try
        {
            pipeClient = new NamedPipeClientStream(".", "UnityPipe", PipeDirection.InOut, PipeOptions.Asynchronous);

            Debug.Log("파이프 서버에 연결을 시도합니다...");

            await pipeClient.ConnectAsync(5000);

            Debug.Log("파이프 서버에 연결되었습니다!");

            writer = new StreamWriter(pipeClient);
            reader = new StreamReader(pipeClient);

            await SendMessageAsync("Command", "Unity가 접속함");

            _ = Task.Run(ReadPipeMessagesAsync);
        }
        catch (TimeoutException)
        {
            Debug.LogError("파이프 연결 시간 초과.");
        }
        catch (Exception e)
        {
            Debug.LogError($"파이프 연결 실패: {e.Message}");
        }
    }

    private async Task ReadPipeMessagesAsync()
    {
        // 파이프가 연결되어 있는 동안 계속 메시지 수신
        while (pipeClient.IsConnected)
        {
            try
            {
                var jsonMessage = await reader.ReadLineAsync();
                Debug.Log(jsonMessage);
                if (string.IsNullOrWhiteSpace(jsonMessage)) break;

                PipeMessage receivedMessage = JsonConvert.DeserializeObject<PipeMessage>(jsonMessage);

                if (receivedMessage != null)
                {
                    UnityMainThreadDispatcher.Enqueue(() => {
                        HandlePipeMessage(receivedMessage);
                    });
                }
            }
            catch { break; } // 연결 끊김 등 예외 발생 시 루프 종료
        }
    }

    // 메시지를 보내는 별도 함수
    public async Task SendMessageAsync(string command, string value)
    {
        if (writer != null && pipeClient.IsConnected)
        {
            PipeMessage messageObject = new PipeMessage
            {
                command = command,
                value = value
            };

            string jsonMessage = JsonUtility.ToJson(messageObject);

            Debug.Log($"[PipeClient] Sending JSON: {jsonMessage}");
            await writer.WriteLineAsync(jsonMessage);
            await writer.FlushAsync();
        }
    }

    // 게임 종료 시 자원 해제
    void OnApplicationQuit()
    {
        writer?.Dispose();
        reader?.Dispose();
        pipeClient?.Dispose();
    }

    private void HandlePipeMessage(PipeMessage message)
    {
        if (isSolved || message.command != "update_key_pos")
        {
            return; // 이미 해결했거나, 좌표 업데이트 명령이 아니면 무시
        }

        try
        {
            // 1. WinForms가 보낸 "x,y" 형식의 '상대 좌표' 문자열을 파싱
            string[] pos = message.value.Split(',');
            float keyX = float.Parse(pos[0]);
            float keyY = float.Parse(pos[1]);

            // (WinForms가 Y축 보정을 이미 했다고 가정)
            Vector2 keyPosition = new Vector2(keyX, keyY);

            // 2. '정답 좌표'와 '열쇠 좌표' 사이의 거리를 계산
            float distance = Vector2.Distance(targetScreenPosition, keyPosition);

            // 3. 거리가 오차 범위(tolerance) 이내로 들어오면 정답으로 판정
            if (distance < tolerance)
            {
                isSolved = true;
                Debug.Log($"정답! 문양이 일치했습니다! (거리: {distance})");

                // 여기에 원하는 로직을 실행합니다.
                ActivatePuzzleSuccess();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Puzzle] 좌표 파싱 오류: {ex.Message} (원본 값: {message.value})");
        }
    }

    private async void ActivatePuzzleSuccess()
    {
        // 예: 문 열기, 아이템 획득, 다음 스테이지로 이동 등
        Debug.Log("퍼즐 성공 로직 실행!");

        await SendMessageAsync("test_XYCorrect", "");
    }
}

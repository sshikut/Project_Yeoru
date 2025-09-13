using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.IO.Pipes;
using System.IO;
using System;

public class NamedPipeServer : MonoBehaviour
{
    public static NamedPipeServer Instance;

    private NamedPipeClientStream pipeClient;
    private StreamWriter writer;
    private StreamReader reader;

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
        ConnectToPipeAsync();
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
                var message = await reader.ReadLineAsync();
                if (message != null)
                {
                    Debug.Log($"서버로부터 받은 메시지: {message}");
                    // TODO: 받은 메시지 처리 (Unity API 접근 시 주의)
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
}

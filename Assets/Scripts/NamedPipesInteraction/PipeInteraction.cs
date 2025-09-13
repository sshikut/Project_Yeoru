using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeInteraction : MonoBehaviour
{
    [Header("Pipe Message Data")]
    public string command;
    public string value;

    // 플레이어가 이 오브젝트와 상호작용할 때 이 함수를 호출한다고 가정
    public async void OnInteract()
    {
        PipeMessage message = new PipeMessage
        {
            command = this.command,
            value = this.value
        };

        string jsonMessage = JsonUtility.ToJson(message);
        Debug.Log($"Sending: {jsonMessage}");

        try
        {
            await NamedPipeServer.Instance.SendMessageAsync(jsonMessage);
            Debug.Log("메시지 전송 완료!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"메시지 전송 실패: {ex.Message}");
        }
    }
}

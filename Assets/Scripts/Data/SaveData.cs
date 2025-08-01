using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string currentScene;         // 현재 위치한 씬 이름
    public float playerX;               // 플레이어 위치 X
    public float playerY;               // 플레이어 위치 Y
    // public List<string> inventory;      // 인벤토리 아이템 이름들
    // public Dictionary<string, bool> flags; // 퀘스트 플래그, 상호작용 여부 등
}

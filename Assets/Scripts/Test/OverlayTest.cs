using System.Collections;
using System.IO;
using UnityEngine;

public class OverlayTest : MonoBehaviour
{
    string path;

    void Start()
    {
        // Debug.Log(Application.dataPath);
        path = Application.dataPath + "/../YeoruEXE/bin/Debug/net8.0-windows/event_signal.txt";

        StartCoroutine(CheckSignal());
    }

    IEnumerator CheckSignal()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(fs))
                {
                    string signal = reader.ReadToEnd();
                    Debug.Log(signal);

                    if (signal == "trigger")
                    {
                        Debug.Log("보조 EXE가 Unity 위에 있음!");
                        // TODO: 원하는 이벤트 실행
                    }
                }
                //string signal = File.ReadAllText(path);

            }
        }
    }
}

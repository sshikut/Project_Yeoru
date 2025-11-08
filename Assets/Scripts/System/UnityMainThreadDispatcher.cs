using UnityEngine;
using System;
using System.Collections.Concurrent; // ?? 스레드 안전 큐를 위해 필수

public class UnityMainThreadDispatcher : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static UnityMainThreadDispatcher _instance;

    // 메인 스레드에서 실행할 작업들을 담아두는 큐 (스레드 안전)
    private static readonly ConcurrentQueue<Action> _jobs = new ConcurrentQueue<Action>();

    /// <summary>
    /// 싱글톤 인스턴스에 접근합니다.
    /// </summary>
    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에서 인스턴스를 찾거나, 없으면 새로 생성
                _instance = FindObjectOfType<UnityMainThreadDispatcher>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("UnityMainThreadDispatcher");
                    _instance = go.AddComponent<UnityMainThreadDispatcher>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // 싱글톤 패턴: 씬에 이미 인스턴스가 있는지 확인
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            // 씬이 바뀌어도 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        // Update는 메인 스레드에서 매 프레임 실행됨
        // 큐에 작업이 남아있으면, 하나씩 꺼내서 실행
        while (_jobs.TryDequeue(out Action job))
        {
            try
            {
                job.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"메인 스레드 작업 실행 중 오류 발생: {ex}");
            }
        }
    }

    /// <summary>
    /// 백그라운드 스레드에서 메인 스레드로 작업을 보냅니다.
    /// </summary>
    /// <param name="job">메인 스레드에서 실행될 Action (함수)</param>
    public static void Enqueue(Action job)
    {
        // 큐에 작업을 추가 (스레드 안전)
        _jobs.Enqueue(job);
    }
}
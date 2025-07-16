using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraView : MonoBehaviour
{
    public Transform target;
    public float pixelsPerUnit = 32f;

    private Vector3 lastTargetPos;

    void Start()
    {
        lastTargetPos = target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 플레이어가 움직였을 때만 따라감
        if (target.position != lastTargetPos)
        {
            Vector3 pos = target.position;
            pos.z = transform.position.z;

            pos.x = Mathf.Round(pos.x * pixelsPerUnit) / pixelsPerUnit;
            pos.y = Mathf.Round(pos.y * pixelsPerUnit) / pixelsPerUnit;

            transform.position = pos;
            lastTargetPos = target.position;
        }
    }
}

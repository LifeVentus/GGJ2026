using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public Vector2 offset = new Vector2(0.2f, 0.1f);
    public float boundaryThreshold = 50f;
    public float backgroundWidth = 200f;
    public float backgroundHeight = 200f;
    private PlayerController player;
    private Vector3 lastCameraPos;
    private Vector3 startPos;
    void Start()
    {
        player = PlayerController.Instance;
        startPos = transform.position;
        lastCameraPos = Camera.main.transform.position;
    }


    void LateUpdate()
    {
        // 计算摄像机移动的差值
        Vector3 cameraMoveDelta = Camera.main.transform.position - lastCameraPos;
        // 背景移动距离 = 摄像机移动差值 × 视差系数（Z轴不动）
        transform.position += new Vector3(cameraMoveDelta.x * offset.x, cameraMoveDelta.y * offset.y, 0);
        // 更新上一帧摄像机位置
        lastCameraPos = Camera.main.transform.position;

        MoveToCenter();
    }

    private void MoveToCenter()
    {
        if (player == null) return;

        // 计算玩家相对于背景的位置（背景中心为原点）
        float playerXRelative = player.transform.position.x - transform.position.x;
        float playerYRelative = player.transform.position.y - transform.position.y;

        // X轴（左右）滚动：玩家接近左/右边界时，背景向对应方向平移整宽
        if (playerXRelative > backgroundWidth / 2 - boundaryThreshold)
        {
            transform.position = player.transform.position;
        }
        else if (playerXRelative < - (backgroundWidth / 2 - boundaryThreshold))
        {
            transform.position = player.transform.position;
        }

        // Y轴（上下）滚动：玩家接近上/下边界时，背景向对应方向平移整高
        if (playerYRelative > backgroundHeight / 2 - boundaryThreshold)
        {
            transform.position = player.transform.position;
        }
        else if (playerYRelative < - (backgroundHeight / 2 - boundaryThreshold))
        {
            transform.position = player.transform.position;
        }
    }
    public void ResetBackground()
    {
        transform.position = startPos;
    }
}

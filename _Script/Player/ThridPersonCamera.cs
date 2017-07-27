using UnityEngine;
using TNet;

public class ThridPersonCamera : TNBehaviour {
    /// <summary>
    /// 角色
    /// </summary>
    Transform player;
    
    /// <summary>
    /// 相机距离角色距离
    /// </summary>
    public float distance = 20f;
    /// <summary>
    /// 相机距离角色最小距离
    /// </summary>
    public float minDistance = 3f;
    /// <summary>
    /// 相机距离角色最大距离
    /// </summary>
    public float maxDistance = 20f;

    /// <summary>
    /// 相机距离角色缩放速度
    /// </summary>
    public float zoomSpeed = 0.2f;
    /// <summary>
    /// 相机环绕角色选择速度
    /// </summary>
    public float rotationSpeed = 2f;

    /// <summary>
    /// 相机俯仰角最小角度
    /// </summary>
    public float yMinAngle = -40f;

    /// <summary>
    /// 相机俯仰角最大角度
    /// </summary>
    public float yMaxAngle = 80f;
    
    /// <summary>
    /// 角色距离相机偏移值
    /// </summary>
    public Vector3 offset = Vector3.zero;

    
    /// <summary>
    /// 主相机
    /// </summary>
    Transform trans;
    /// <summary>
    ///主相机旋转
    /// </summary>
    Vector3 rot;
    void Awake()
    {
        trans = Camera.main.transform;
        rot = trans.eulerAngles;
        player = transform;
    }
	
	void LateUpdate () {
        //如果角色未指定，返回
	    if(!player)return;
        if(!trans)return;

        //得到实际角色位置
        Vector3 targetPos = player.position + offset;

	    if (tno.isMine)
	    {
            //鼠标右键控制旋转
            if (Input.GetMouseButton(1))
            {
                rot.y += Input.GetAxis("Mouse X") * rotationSpeed;
                rot.x -= Input.GetAxis("Mouse Y") * rotationSpeed;
                rot.x = Mathf.Clamp(rot.x, yMinAngle, yMaxAngle);
                trans.rotation = Quaternion.Euler(rot.x, rot.y, 0f);
            }

            //鼠标中键缩放
            float scroll = GetAxisRawScrollUniversal();
            float step = scroll * zoomSpeed * Mathf.Abs(distance);
            distance = Mathf.Clamp(distance - step, minDistance, maxDistance);

            //相机跟随角色
            trans.position = targetPos - (trans.rotation * Vector3.forward * distance);
        }
    }

    /// <summary>
    /// 鼠标滚轮数值
    /// </summary>
    /// <returns></returns>
    float GetAxisRawScrollUniversal()
    {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scroll < 0) return -1f;
        if (scroll > 0) return 1f;
        return 0f;
    }
}

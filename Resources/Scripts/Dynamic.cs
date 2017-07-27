using UnityEngine;

namespace VrNet.CommonLogic
{
    /// <summary>
    /// 动态物体分析标记=》Runtime状态必须放在Resource文件夹！
    /// </summary>
    public class Dynamic : MonoBehaviour
    {
        public string Tag = null;
       
        void Start()
        {
            Tag = this.gameObject.name;
        }
    }
}

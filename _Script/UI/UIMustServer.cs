using UnityEngine;
using System.Collections;
using TNet;
namespace VrNet.UICommon
{
    public class UIMustServer : MonoBehaviour
    {

        public GameObject target;
        // Use this for initialization
        void Start()
        {
			if (target != null && !TNManager.isHosting)
            {
                target.SetActive(false);
            }
        }
    }

}

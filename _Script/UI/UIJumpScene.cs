using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace VrNet.LoginLogic
{
    [RequireComponent(typeof(UIButton))]
    public class UIJumpScene : MonoBehaviour
    {

        public string Level;

        void OnClick()
        {
            SceneManager.LoadSceneAsync(Level);
        }
    }
}

using UnityEngine;
using System.Collections;

public class PlayAnimate : MonoBehaviour {

    public Animation m_Animation;
    public AnimationClip[ ] m_AnimationClips;

    void Start () {
        StartCoroutine(LoopPlayAllAnimation( ));
    }

    IEnumerator LoopPlayAllAnimation( ) {
        int i = 0;
        while (true) {
            m_Animation.CrossFade(m_AnimationClips[i].name);
            yield return new WaitForSeconds(m_AnimationClips[i].length);
            i = (i + 1) % m_AnimationClips.Length;
        }
    }

}

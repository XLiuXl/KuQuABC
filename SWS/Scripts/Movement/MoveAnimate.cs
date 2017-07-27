using UnityEngine;
using System.Collections;
using SWS;
using DG.Tweening;

namespace Gzc.Animate {

    public class MoveAnimate : MonoBehaviour {

        public Animation m_animation;

        public AnimationClip m_clipIdle;
        public AnimationClip m_clipWalk;        

        //movement script references
        private splineMove sMove;
        private UnityEngine.AI.NavMeshAgent nAgent;        

        void Start( ) {
            sMove = GetComponent<splineMove>( );
            if (!sMove) {
                nAgent = GetComponent<UnityEngine.AI.NavMeshAgent>( );
            }    
        }
      
        void Update( ) {
            UpdateAnimationClipState( );
        }

        private bool isMove( ) {
            //init variables
            float speed = 0f;          

            if (sMove) {
                speed = sMove.tween == null || !sMove.tween.IsPlaying( ) ? 0f : sMove.speed;               
            } else {
                speed = nAgent.velocity.magnitude;               
            }
            //Debug.Log(string.Format("speed={0}", speed));
            return speed > 0F;
        }

        private void playAnimationClip(string clipName) {
            //Debug.Log(string.Format("clipName={0}", clipName));
            m_animation.CrossFade(clipName, 0.2F);
        }

        private void UpdateAnimationClipState( ) {
            if (isMove( )) {
                playAnimationClip(m_clipWalk.name);
            } 
//			else {
//                playAnimationClip(m_clipIdle.name);
//            }
        }

    }
}


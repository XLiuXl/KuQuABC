namespace VRTK.Examples
{
    using UnityEngine;
    using DG.Tweening;
    public class VRTK_ControllerPointerEvents_ListenerExample : MonoBehaviour
    {
        public bool showHoverState = false;
        public bool showPointer = true;
        #region LWL 射线距离缩放
        private bool haveThings = false;
        private Transform AttachPoint = null;
        private float maximumLength = 2;
        Tweener tweener = null;
        #endregion

        private void Start()
        {
            if (GetComponent<VRTK_DestinationMarker>() == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerPointerEvents_ListenerExample", "VRTK_DestinationMarker", "the Controller Alias"));
                return;
            }

            //Setup controller event listeners
            GetComponent<VRTK_DestinationMarker>().DestinationMarkerEnter += new DestinationMarkerEventHandler(DoPointerIn);
            if (showHoverState)
            {
                GetComponent<VRTK_DestinationMarker>().DestinationMarkerHover += new DestinationMarkerEventHandler(DoPointerHover);
            }
            GetComponent<VRTK_DestinationMarker>().DestinationMarkerExit += new DestinationMarkerEventHandler(DoPointerOut);
            GetComponent<VRTK_DestinationMarker>().DestinationMarkerSet += new DestinationMarkerEventHandler(DoPointerDestinationSet);

            GetComponent<VRTK_ControllerEvents>().GripPressed += VRTK_ControllerPointerEvents_ListenerExample_GripPressed;
            GetComponent<VRTK_ControllerEvents>().GripReleased += VRTK_ControllerPointerEvents_ListenerExample_GripReleased;


            GetComponent<VRTK_ControllerEvents>().TouchpadPressed += VRTK_ControllerPointerEvents_ListenerExample_TouchedPressed;
            GetComponent<VRTK_ControllerEvents>().TouchpadReleased += VRTK_ControllerPointerEvents_ListenerExample_TouchedReleased;
            maximumLength = this.GetComponent<VRTK_StraightPointerRenderer>().maximumLength;
            InitControllerAttach();


        }

        void PointerState(bool state)
        {
            if (state)
            {
                GetComponent<VRTK_StraightPointerRenderer>().tracerVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOn;
                GetComponent<VRTK_StraightPointerRenderer>().cursorVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOn;
            }
            else {
                GetComponent<VRTK_StraightPointerRenderer>().tracerVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
                GetComponent<VRTK_StraightPointerRenderer>().cursorVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
            }
        }

        //private void VRTK_ControllerPointerEvents_ListenerExample_TouchpadReleased(object sender, ControllerInteractionEventArgs e)
        //{
        //    //InitControllerAttach();
        //    showPointer = !showPointer;
        //    PointerState(showPointer);
        //}
        void Update()
        {
            //使用交互物体时 持续改变最大值  禁止VRTK将其放到最大值处
            if (haveThings)
            {
                this.GetComponent<VRTK_StraightPointerRenderer>().maximumLength = AttachPoint.transform.localPosition.z;
            }

        }
           
        private void VRTK_ControllerPointerEvents_ListenerExample_TouchedPressed(object sender, ControllerInteractionEventArgs e)
        {

            AttachPoint = this.GetComponent<VRTK_InteractGrab>().controllerAttachPoint.transform;


            #region 求出圆盘上方向  还是下方向被按下
            float angle = e.touchpadAngle;
            float time = 0;

            Debug.Log(time);

            if (angle >= 0 && angle <= 90 || angle > 270)
            {
                time = 3.0f * (Mathf.Abs(maximumLength - AttachPoint.localPosition.z) / maximumLength);
                tweener = AttachPoint.DOLocalMoveZ(maximumLength, time);

            }
            else
            {
                time = 3.0f * (Mathf.Abs(AttachPoint.localPosition.z - 0.6f) / maximumLength);
                tweener = AttachPoint.DOLocalMoveZ(0.6f, time);
            }
            #endregion

        }

        private void VRTK_ControllerPointerEvents_ListenerExample_TouchedReleased(object sender, ControllerInteractionEventArgs e)
        {
            if (tweener != null)
            {
                tweener.Kill();
                tweener = null;
            }
         //   this.GetComponent<VRTK_StraightPointerRenderer>().maximumLength = maximumLength;
        }

        //IEnumerator MoveToPosition(Vector3 pos)
        //{

        //    pos += new Vector3(0, 0, -0.8f);
        //    Transform m_take = AttachPoint;
        //    while (m_take.transform.localPosition != pos)
        //    {
        //        m_take.transform.position = Vector3.MoveTowards(m_take.position, pos, 0.5f * Time.deltaTime);
        //        yield return 0;
        //    }
        //}

        #region 射线开关
        private void VRTK_ControllerPointerEvents_ListenerExample_GripReleased(object sender, ControllerInteractionEventArgs e)
        {
            //InitControllerAttach();
            showPointer = !showPointer;
            PointerState(showPointer);
        }

        private void VRTK_ControllerPointerEvents_ListenerExample_GripPressed(object sender, ControllerInteractionEventArgs e)
        {
            //InitControllerAttach();
        }
        #endregion


        private void DebugLogger(uint index, string action, Transform target, RaycastHit raycastHit, float distance, Vector3 tipPosition)
        {
            string targetName = (target ? target.name : "<NO VALID TARGET>");
            string colliderName = (raycastHit.collider ? raycastHit.collider.name : "<NO VALID COLLIDER>");
            VRTK_Logger.Info("Controller on index '" + index + "' is " + action + " at a distance of " + distance + " on object named [" + targetName + "] on the collider named [" + colliderName + "] - the pointer tip position is/was: " + tipPosition);
        }

        private void DoPointerIn(object sender, DestinationMarkerEventArgs e)
        {
            //DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "POINTER IN", e.target, e.raycastHit, e.distance, e.destinationPosition);
            InitControllerAttach();
        }

        private void Grab_ControllerGrabInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            //InitControllerAttach();
        }

        void InitControllerAttach()
        {
			var actualCursor = this.GetComponent<VRTK_StraightPointerRenderer>().actualCursor;
            //GetComponent<VRTK_InteractGrab>().controllerAttachPoint = actualCursor.gameObject.GetComponent<Rigidbody>();
            GetComponent<VRTK_InteractGrab>().controllerAttachPoint.transform.parent = actualCursor.transform.parent;
            GetComponent<VRTK_InteractGrab>().controllerAttachPoint.transform.localPosition = actualCursor.transform.localPosition;
            VRTK_InteractGrab controller = this.GetComponent<VRTK_InteractGrab>();
            controller.ControllerGrabInteractableObject += Controller_ControllerGrabInteractableObject;
            controller.ControllerUngrabInteractableObject += Controller_ControllerUngrabInteractableObject;
            Debug.Log(actualCursor.transform.localPosition+"-----------"+ actualCursor.transform.position);
        }

        private void Controller_ControllerUngrabInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            haveThings = false;
            var actualCursor = this.GetComponent<VRTK_StraightPointerRenderer>().actualCursor;
            //GetComponent<VRTK_InteractGrab>().controllerAttachPoint = actualCursor.gameObject.GetComponent<Rigidbody>();
            GetComponent<VRTK_InteractGrab>().controllerAttachPoint.transform.parent = actualCursor.transform.parent;
            GetComponent<VRTK_InteractGrab>().controllerAttachPoint.transform.localPosition = actualCursor.transform.localPosition;
        }

        private void Controller_ControllerGrabInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            haveThings = true;

            var actualCursor = this.GetComponent<VRTK_StraightPointerRenderer>().actualCursor;
            //GetComponent<VRTK_InteractGrab>().controllerAttachPoint = actualCursor.gameObject.GetComponent<Rigidbody>();
            GetComponent<VRTK_InteractGrab>().controllerAttachPoint.transform.parent = actualCursor.transform.parent;
            GetComponent<VRTK_InteractGrab>().controllerAttachPoint.transform.localPosition = actualCursor.transform.localPosition;
        }

        private void DoPointerOut(object sender, DestinationMarkerEventArgs e)
        {
            //DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "POINTER OUT", e.target, e.raycastHit, e.distance, e.destinationPosition);
            //InitControllerAttach();
        }

        private void DoPointerHover(object sender, DestinationMarkerEventArgs e)
        {
            //DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "POINTER HOVER", e.target, e.raycastHit, e.distance, e.destinationPosition);
            //InitControllerAttach();
        }

        private void DoPointerDestinationSet(object sender, DestinationMarkerEventArgs e)
        {
            //DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "POINTER DESTINATION", e.target, e.raycastHit, e.distance, e.destinationPosition);
        }
    }
}
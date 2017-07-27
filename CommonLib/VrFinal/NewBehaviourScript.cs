using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    

    public OVRPose centerEyePose = OVRPose.identity;
    public OVRPose leftEyePose = OVRPose.identity;
    public OVRPose rightEyePose = OVRPose.identity;
    public OVRPose leftHandPose = OVRPose.identity;
    public OVRPose rightHandPose = OVRPose.identity;
    public OVRPose trackerPose = OVRPose.identity;
    void State() { }
    void Update() { }
    void Awake()
    {
        OVRCameraRig rig = GameObject.FindObjectOfType<OVRCameraRig>();
        if (rig != null)
            rig.UpdatedAnchors += OnUpdatedAnchors;
    }
    void OnUpdatedAnchors(OVRCameraRig rig)
    {
        if (!enabled)
            return;
        //This doesn't work because VR camera poses are read-only.  
        //rig.centerEyeAnchor.FromOVRPose(OVRPose.identity);  
        //Instead, invert out the current pose and multiply in the desired pose.  
        OVRPose pose = rig.centerEyeAnchor.ToOVRPose(true).Inverse();
        pose = centerEyePose * pose; rig.
        trackingSpace.FromOVRPose(pose, true);
        //OVRPose referenceFrame = pose.Inverse();  
        //The rest of the nodes are updated by OVRCameraRig, not Unity, so they're easy.  
        rig.leftEyeAnchor.FromOVRPose(leftEyePose);
        rig.rightEyeAnchor.FromOVRPose(rightEyePose);
        rig.leftHandAnchor.FromOVRPose(leftHandPose);
        rig.rightHandAnchor.FromOVRPose(rightHandPose);
        rig.trackerAnchor.FromOVRPose(trackerPose);
    }
}

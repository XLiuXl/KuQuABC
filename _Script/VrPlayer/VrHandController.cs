using UnityEngine;
using System.Collections;
using TNet;

public  enum Hands
{
    Left,
    Right
}

public class VrHandController : TNBehaviour {
    
    public  Hands side;
  

    private GameObject trackedController;

    private SteamVR_Controller.Device steamDevice;

    void Start()
    {
        if (tno.isMine)
        {
			trackedController = GameObject.Find(string.Format("Controller ({0})", side.ToString("G").ToLowerInvariant()));

			Debug.Log("vrController=>" + string.Format("Controller ({0})", side.ToString("G").ToLowerInvariant()));

			Helper.AttachAtGrip(trackedController.transform, transform);

			steamDevice = SteamVR_Controller.Input((int)trackedController.GetComponent<SteamVR_TrackedObject>().index);
        }      
    }

}

using UnityEngine;
using TNet;
using VrNet.UILogic;
using VRTK;
using System.Collections;

public class VrPlayerController :TNBehaviour {
    /// <summary>
    /// VrCam-NoVrController
    /// </summary>
    public GameObject vrCameraRigBasic;
		/// <summary>
	/// The vr camera rig
	/// </summary>
	public GameObject vrCameraRigVr;

	private GameObject vrCameraRig;

    public GameObject CamRT;

    /// <summary>
    ///left hand controller
    /// </summary>
    public string leftHandPrefab;
    /// <summary>
    /// right hand controller 
    /// </summary>
    public string rightHandPrefab;
    /// <summary>
    /// Vr camera
    /// </summary>
    private GameObject vrCameraRigInstance;   
	private GameObject MainPlayer;
	private UnityEngine.AI.NavMeshAgent agent;
	private GameObject currentTarget;

    /// <summary>
    /// Vr character head
    /// </summary>
    private Transform headOfVrPlayer;
    [Header("ConfigVrCharacter")]
    /// <summary>
    /// Vr character body
    /// </summary>
    public Transform bodyOfVrPlayer;
	public bool showVrBody = false;

	private bool useHmdControlller = false;

    public static bool LoadOnCompleted = false;
    public bool isTeacherNeedFollow = false;
    void Awake()
    {
        Messenger.AddListener("OnLoadOnCompleted", OnLoadOnCompleted);
    }

    void Start () {
        Debug.Log("------------------------------------------------------------------------------------------------------------VrPlayerControllerInit");
		if (PlayerProfile.VUseHmd == "True")
			useHmdControlller = true;
		else
			useHmdControlller = false;

		if (PlayerProfile.VAdjustHeight == "True") {
			if (useHmdControlller) {
				vrCameraRig = vrCameraRigVr;
			} else {
				vrCameraRig = vrCameraRigBasic;
			}
		} else {
			if (useHmdControlller) {
				vrCameraRig = vrCameraRigVr;
			} else {
				vrCameraRig = vrCameraRigBasic;
			}
		}



		MainPlayer = GameObject.FindGameObjectWithTag ("Player");
        //default close vr body
        bodyOfVrPlayer.gameObject.SetActive(showVrBody);

        if (!tno.isMine) return;

        //del scene main camera
       
        DestroyImmediate(Camera.main.gameObject);
       
        //open or close vr body
        if(bodyOfVrPlayer!=null)bodyOfVrPlayer.gameObject.SetActive(showVrBody);

        //create vr camera and link to VrCameraRigInstance
        vrCameraRigInstance = (GameObject)Instantiate(
            vrCameraRig,
            transform.position,
            transform.rotation);
		//add teleport script
		//if (useHmdControlller) {
		//	if(vrCameraRig.GetComponent<VRTK_BasicTeleport>()==null)
		//		vrCameraRig.AddComponent<VRTK_BasicTeleport> ();
		//}


        if (showVrBody) {
            bodyOfVrPlayer = transform.Find("VRPlayerBody");           
        }

        headOfVrPlayer = transform.Find("VrPlayerHead");
		//Show or hide...
        headOfVrPlayer.gameObject.SetLayerRecursively(LayerMask.NameToLayer("VrPlayer_Head"));

		StartCoroutine (Init (3f));
		       
        //TryDetectControllers();

        Invoke("TrySetVrPlayerName", 2f);

		if(tno.isMine)ConfigVrPlayerId ();
        

    }

	IEnumerator Init(float time){
		yield return new WaitForSeconds (time);

		Debug.Log("VrDevices:" + UnityEngine.VR.VRSettings.loadedDeviceName);

		//Check Vr Input Device
		GameObject head = null;
        

        if (vrCameraRigInstance.GetComponentInChildren<VRTK_TrackedHeadset>()!=null)
			head = vrCameraRigInstance.GetComponentInChildren<VRTK_TrackedHeadset>().gameObject;

		//Debug.Log (head.name);
               
		transform.parent = head.transform;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;

		headOfVrPlayer.transform.position = Vector3.zero;
        headOfVrPlayer.transform.rotation = Quaternion.identity;
		headOfVrPlayer.localPosition = Vector3.zero;
		headOfVrPlayer.localRotation = Quaternion.identity;

        if (showVrBody)bodyOfVrPlayer.transform.parent = transform.parent.parent;
        
        Debug.Log("Hosting:"+TNManager.isHosting + " Client:"+TNManager.client.playerName);

		//opt
		if (!tno.isMine)
		{
			head.GetComponent<Camera>().enabled = false;
			Debug.Log("Opt.......=>Disable Camera Render...................................");
		}

		if (tno.isMine)
		{
			CamRT.gameObject.SetActive(false);
			Debug.Log("Opt.......=>Disable ....................CamRT...........");
		}

	}

    void OnLoadOnCompleted()
    {
        Debug.Log("setLoadOnCompleter==true");
        LoadOnCompleted = true;
    }


    void UpdateVrBodyPos()
    {
        if (showVrBody)
        {
            bodyOfVrPlayer.localPosition = new Vector3(Camera.main.transform.localPosition.x,
                                                headOfVrPlayer.transform.localPosition.y + Camera.main.transform.localPosition.y - 0.03f,
                                                Camera.main.transform.localPosition.z);
            bodyOfVrPlayer.localRotation = Quaternion.Euler(new Vector3(-90f, Camera.main.transform.eulerAngles.y,
                                                                        0f));
        }
    }

	/// <summary>
	/// Configs the vr player identifier.
	/// this not good way
	/// </summary>
	void ConfigVrPlayerId(){
		int id = TNManager.players.Count;
		Debug.Log ("PlayerCount=>"+id);
        //Server + 1 Client

        if (useHmdControlller)
        {

            currentTarget = GameObject.Find("Player" + id.ToString());
        }
        else
        {
            currentTarget = GameObject.Find("Player1");
        }
        Debug.Log (currentTarget.name);
        
        //init
        vrCameraRigInstance.transform.position = currentTarget.transform.position;
	}

    /// <summary>
    /// TryDetectControllers
    /// </summary>
    void TryDetectControllers()
    {
        var controllers = vrCameraRigInstance.GetComponentsInChildren<SteamVR_TrackedObject>();

        //add coustom script
		/*if(useHmdControlller){
			for (int i = 0; i < controllers.Length; i++)
			{
				if(controllers[i].transform.gameObject.GetComponent<VRTK_SimplePointer>()==null)
					controllers[i].transform.gameObject.AddComponent<VRTK_SimplePointer>();
				if (controllers[i].transform.gameObject.GetComponent<VRTK_ControllerEvents>() == null)
					controllers[i].transform.gameObject.AddComponent<VRTK_ControllerEvents>();
				if (controllers[i].transform.gameObject.GetComponent<VRTK_ControllerActions>() == null)
					controllers[i].transform.gameObject.AddComponent<VRTK_ControllerActions>();
				if (controllers[i].transform.gameObject.GetComponent<VRTK_InteractGrab>() == null)
					controllers[i].transform.gameObject.AddComponent<VRTK_InteractGrab>();
			}
		}*/


		// (!useHmdControlller) {
			//for (int i = 0; i < controllers.Length; i++)
			//{
				//if (controllers [i].transform.GetComponentInChildren<SteamVR_RenderModel> () != null) {
					//controllers [i].transform.GetComponentInChildren<SteamVR_RenderModel> ().gameObject.transform.localScale = new Vector3 (0.1f,0.1f,0.1f);
					//controllers[i].transform.GetComponentInChildren<SteamVR_RenderModel>().gameObject.SetActive(false);
				//}
			//}
		//}


		//close hmd models(scale to 0.1f)
        //for (int i = 0; i < controllers.Length; i++)
        //{
			//if (controllers [i].transform.GetComponentInChildren<SteamVR_RenderModel> () != null) {
				//controllers [i].transform.GetComponentInChildren<SteamVR_RenderModel> ().gameObject.transform.localScale = new Vector3 (0.2f,0.2f,0.2f);
				//controllers[i].transform.GetComponentInChildren<SteamVR_RenderModel>().gameObject.SetActive(true);
			//}
        //}

        //if hmd controller not null call rcc
		if (controllers != null && controllers.Length == 2 && controllers[0] != null && controllers[1] != null)
        {
			if (useHmdControlller) {
				Debug.Log("ServerInfo=>" + "SpawnHands......");
				TNManager.Instantiate(TNManager.lastChannelID, "SpawnLHand", leftHandPrefab, false, transform.position, transform.rotation);
				TNManager.Instantiate(TNManager.lastChannelID, "SpawnRHand", rightHandPrefab, false, transform.position, transform.rotation);
			}
        }
        else
        {
			Invoke("TryDetectControllers", 2f);
        }
    }

    [RCC]
    static GameObject SpawnLHand(GameObject prefab, Vector3 pos, Quaternion rot)
    {
            
        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        VrHandController leftVRHand = go.GetComponent<VrHandController>();
        leftVRHand.side = Hands.Left;
        leftVRHand.tno.channelID = TNManager.lastChannelID;

        Debug.Log("VrLeft=>"+go.name);

        // Set the position and rotation based on the passed values
        Transform t = go.transform;
        t.position = pos;
        t.rotation = rot;
        return go;
    }

    [RCC]
    static GameObject SpawnRHand(GameObject prefab, Vector3 pos, Quaternion rot)
    {

        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        VrHandController rightVRHand = go.GetComponent<VrHandController>();
        rightVRHand.side = Hands.Right;
        rightVRHand.tno.channelID = TNManager.lastChannelID;

        Debug.Log("VrRight=>" + go.name);

        // Set the position and rotation based on the passed values
        Transform t = go.transform;
        t.position = pos;
        t.rotation = rot;
        return go;
    }
    
    void Update () {
        UpdateVrBodyPos();
		if(!useHmdControlller)
			UpdateVrPlayerPos ();
        if (isTeacherNeedFollow)
            TeacherNeedVrPlayerPos();
        
    }

	void UpdateVrPlayerPos(){
       // Debug.Log("Dont‘t  UseHand");
		if (MainPlayer!=null) {	
			var mAgent = MainPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>();
            //Fixed Teacher Rotate
          //  Debug.Log(mAgent.remainingDistance+"  "+LoadOnCompleted) ;
            if (mAgent.remainingDistance <= 0.05f&&mAgent.remainingDistance>0&&LoadOnCompleted) {
                
                if (tno.isMine)
                {
                    Debug.Log("无手柄学生自动跟随");
                    vrCameraRigInstance.transform.position = currentTarget.transform.position;
                    //Debug.Log(vrCameraRigInstance.name);
                    //Debug.Log(vrCameraRigInstance.transform.parent.name);
                    //Debug.Log(vrCameraRigInstance.transform.parent.transform.name);
                }
			}
		}
	}
    void TeacherNeedVrPlayerPos()
    {
        if (MainPlayer != null)
        {
            var mAgent = MainPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>();
            //Fixed Teacher Rotate
            if (mAgent!=null&&mAgent.remainingDistance <= 0.05f && mAgent.remainingDistance > 0)
            {
                Debug.Log("老师到达地点学生跟随");
                if (tno.isMine) {

                    var target = vrCameraRigInstance.GetComponentInChildren<VRTK_SDKSetup>().gameObject;
                    Transform[] transforms = target.GetComponentsInChildren<Transform>();

                    for (int i = 0; i < transforms.Length; i++)
                    {
                        if (transforms[i].gameObject.name.Contains("CameraRig"))
                        {
                            transforms[i].position = currentTarget.transform.position;
                        }
                    }

                    //vrCameraRigInstance.transform.position = currentTarget.transform.position;
                    //Debug.Log(vrCameraRigInstance.name);
                }
            }
        }
    }


    void TrySetVrPlayerName()
    {
        vrCameraRigInstance.name = headOfVrPlayer.parent.name;
    }
}

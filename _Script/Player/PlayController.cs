using UnityEngine;
using System.Collections;
using TNet;
using VrNet.UILogic;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class PlayController : TNBehaviour
{
	public LayerMask layersToIgnore = Physics.IgnoreRaycastLayer;

    public GameObject CamRT;

    float maxDistance = 150f;
    
    private UnityEngine.AI.NavMeshAgent agent;

    private Animation animaton;

    [Header("Indicator")]
    [SerializeField]
    GameObject indicatorPrefab;
    GameObject indicator;
    
	[Header("VrPlayerArea")]
	public GameObject areaObject;

	private bool useHmdController = false;
	private bool isSmall = false;

    void Awake()
    {
        Init();



        VMaxPlayer = int.Parse(PlayerProfile.VMaxPlayer) - 1;
        Debug.Log("=======================================" + VMaxPlayer);
        CalculationPoints(VMaxPlayer);


        animaton = transform.GetComponent<Animation>();
        agent = transform.GetComponent<UnityEngine.AI.NavMeshAgent>();
        Debug.Log("添加点击事件====》 双击到达指定位置");
        Messenger.AddListener<Vector3>("MoveToPos", MoveToPos);

        //opt
        if (!tno.isMine)
        {
            CamRT.gameObject.SetActive(false);
        }
    }
    void Init()
    {
        if (PlayerPrefs.GetString("Vr_Mulit_HmdHand") == "True")
            useHmdController = true;
        else
            useHmdController = false;

        if (PlayerProfile.VSmall == "True")
            isSmall = true;
        else
            isSmall = false;

        if (useHmdController)
        {
            if (isSmall)
            {
                areaObject.SetActive(true);
                areaObject.transform.localScale = new Vector3(0.53f, 0.53f, 1f);
                radius = 2.0f;
            }
            else
            {
                areaObject.SetActive(true);
                areaObject.transform.localScale = new Vector3(1f, 1f, 1f);
                radius = 4.0f;
            }
        }
        else
            areaObject.SetActive(useHmdController);
    }
	void Start(){
       
		
       
    }
    private float radius = 2.0f;
    private int Maxcount = 15;
    private int VMaxPlayer = 31;
    private float angle = 0;
    int LeftPlayerCount = 0;
    int index = 0;
    /// <summary>
    /// Configs the vr player identifier.
    /// this not good way
    /// </summary>
    void CalculationPoints(int count)

    {
        if (count<=0)
            return;

        int finalCount = count;
        float radiu = radius;
        LeftPlayerCount = count - Maxcount;
        if (count > Maxcount)
        {
            finalCount = Maxcount;
      
            
       
            radius -= 1;

        }

        angle = 360 / finalCount;
        Vector3 v = transform.position + transform.forward  * radiu;

        Quaternion r = transform.rotation;

        
        //+1为了围成圈
        for (int i = 1; i < finalCount+1; i++)

        {
            index += 1;
            Quaternion q = Quaternion.Euler(r.eulerAngles.x , r.eulerAngles.y - (angle * i), r.eulerAngles.z);

            v = transform.position + (q * Vector3.forward) * radiu;
                GameObject obj = new GameObject();
           // GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            string name = "Player" + index.ToString();
             obj .name=name;

            var player = GameObject.FindGameObjectWithTag("Player");

            obj.transform.SetParent(player.GetChild("Main_Range").transform);
            obj.transform.localPosition = v;

        }
        CalculationPoints(LeftPlayerCount);


    }
    void Update () {

	    if (tno.isMine)
	    {
			//left mouse clicked move character
            //Hide NGUI Event
            if (Input.GetMouseButtonDown(0)&&!UICamera.isOverUI)
            {
                //Raycast
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

				if (Physics.Raycast(ray, out hit, maxDistance,~layersToIgnore))
                {
                    Vector3 pos = hit.point;

                    //look at 
                    transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
                    //set agent des position
                    SetIndicatorViaPosition(pos, hit.normal);
                    agent.SetDestination(pos);

					if (agent.destination == pos) {
						agent.updateRotation = false;
						agent.updatePosition = false;
					}
					//call rfc function
                    tno.SendQuickly("OnSetDestination", Target.Others,pos);
                }
            }

            UpdateAnimation();
        }
    }
    void MoveToPos(Vector3 pos)
    {
        //look at 
        transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
        agent.SetDestination(pos);

        if (agent.destination == pos)
        {
            agent.updateRotation = false;
            agent.updatePosition = false;
        }
        Debug.Log("到达位置  :  " + pos);
        //call rfc function
        tno.SendQuickly("OnSetDestination", Target.Others, pos);
    }

    [RFC]
    void OnSetDestination(Vector3 p)
    {
        agent.SetDestination(p);
    }

    [RFC]
    void OnUpdateAnimation(string name)
    {
        animaton.Play(name);
    }

    void UpdateAnimation()
    {
        string name = string.Empty;
        if (!agent.isOnNavMesh)
        {
            return;
        }
        //Check agent distance
        if (agent.remainingDistance <= 0.1f)
        {
            name = "Idle";
            animaton.Play(name);
        }
        else
        {
            name = "Fly";
            animaton.Play(name);
        }

        tno.SendQuickly("OnUpdateAnimation",Target.Others,name);
    }
   
    /// <summary>
    /// Set Indicator pos
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="normal"></param>
    void SetIndicatorViaPosition(Vector3 pos, Vector3 normal)
    {
        if (!indicator) indicator = Instantiate(indicatorPrefab);
        indicator.transform.parent = null;
        indicator.transform.position = pos + Vector3.up * 0.01f;
        indicator.transform.up = normal; // adjust to terrain normal
        indicator.transform.eulerAngles = new Vector3(0f,0f,0f);
    }
}

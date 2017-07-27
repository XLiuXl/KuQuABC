using System;
using UnityEngine;
using System.Collections;
using TNet;

public class CreatePlayer : MonoBehaviour {

    /// <summary>
    /// 0代表使用最后一个通道
    /// </summary>

    public int channelID = 0;

    /// <summary>
    /// Vr预制体路径
    /// </summary>

     string VrPrefabPath = "VrPlayer";
    /// <summary>
    /// 常规角色路径
    /// </summary>
     string NoVrPrefabPath = "Player";
   
    public bool persistent = false;

    [Header("ConfigControllerType")]
    public bool enableVR;
    Vector3 playerPos;
  
    List<Vector3> mListPos = new List<Vector3>(); 

    //Random Client Player Pos
    void Init()
    {
        Transform[] trans = GameObject.FindGameObjectWithTag("Player").GetChild("Main_Range").GetComponentsInChildren<Transform>();
        Debug.Log(trans.Length);
		for (int i = 0; i < trans.Length; i++)
        {
            
			if (trans [i].name.Contains ("Player")) {
                
				mListPos.Add(trans[i].localPosition);
                Debug.Log(mListPos.Count);
                Debug.Log(i.ToString()+"   "+mListPos[mListPos.Count-1]);
                   Debug.Log( "===>"+trans [i].name);

			}
        }
    }

    IEnumerator Start()
    {       
		if (enableVR)
		{
			Debug.Log("Enaled Vr ......");
		}

		while (TNManager.isJoiningChannel) yield return null;
        if (channelID < 1) channelID = TNManager.lastChannelID;

        //NoVR
        //GetDbPlayerPos
        List<float> tempList = new List<float>();
        for (int i = 0; i < 3; i++)
        {
            tempList.Insert(i, Convert.ToSingle(PlayerPrefs.GetString("Vr_Player_Pos" + i)));
        }

        playerPos = new Vector3(tempList[0], tempList[1], tempList[2]);
        transform.position = playerPos;
        //if (!UserSession.isAdmin) {
        if (!TNManager.isHosting) {
            //VR
            yield return new WaitForSeconds(2);
            Init( );
            //GetDbPlayerPos
            //List<float> tempList = new List<float>( );
            //for (int i = 0; i < 3; i++) {
            //    tempList.Insert(i, Convert.ToSingle(PlayerPrefs.GetString("Vr_Player_Pos" + i)));
            //}
        
            //playerPos = new Vector3(tempList[0], tempList[1], tempList[2]);
            //transform.position = playerPos;
            var ff = mListPos.Count <= 1 ? 0 : mListPos.Count - 1;
            var arrIndex = Mathf.FloorToInt(UnityEngine.Random.value * ff);
            Debug.Log("Index=>" + arrIndex + "=>" + mListPos[arrIndex]);
            TNManager.Instantiate(channelID, "CreateAtPosition", VrPrefabPath, persistent, mListPos[arrIndex], transform.rotation);
        }
        else
        {

            //NoVR
            //GetDbPlayerPos
            //List<float> tempList = new List<float>();
            //for (int i = 0; i < 3; i++)
            //{
            //    tempList.Insert(i, Convert.ToSingle(PlayerPrefs.GetString("Vr_Player_Pos" + i)));
            //}

            //playerPos = new Vector3(tempList[0], tempList[1], tempList[2]);
            //transform.position = playerPos;
            Debug.Log ("Player=Pos=>"+playerPos);

            TNManager.Instantiate(channelID, "CreateAtPosition", NoVrPrefabPath, persistent,playerPos, transform.rotation);
        }

        GameObject.Find("outside").SendMessage("InitOutSide",playerPos);

        Destroy(gameObject);
    }

    [RCC]
    static GameObject CreateAtPosition(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        //Instantiate the prefab
        GameObject go = prefab.Instantiate();

        //Set the position and rotation based on the passed values
        Transform t = go.transform;

        if (t.GetComponent<UnityEngine.AI.NavMeshAgent>()!=null)
        {
            t.GetComponent<UnityEngine.AI.NavMeshAgent>().nextPosition = pos;
            t.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = pos;
           
        }
        else
        {
            t.position = pos;
            t.rotation = rot;
        }
        
        Debug.Log("Rcc.Pos=>" + t.position);

        return go;
    }
}

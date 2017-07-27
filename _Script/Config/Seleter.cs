using ABSystem;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;
using TNet;
using UnityEngine;
using UnityEngine.SceneManagement;
using VrNet.CommonLogic;
using VRTK;

namespace VrNet.Logic
{

    /// <summary>
    /// Search db
    /// </summary>
    public class Seleter : MonoBehaviour
    {
       
        private const string TAG = "Dynamic";
        private const string TAG2 = "Static";
        private const string TAG3 = "AllHand";
        int Elementslength = 0;
        private AssetBundleManager mManager;

        void Awake()
        {
            SceneManager.sceneLoaded += OnSelecter;

            if(mManager==null
                )mManager = gameObject.AddComponent<AssetBundleManager>();

            if (mManager != null)
                mManager.onProgress += OnProgress;

        }

        private void OnProgress(AssetBundleLoadProgress progress)
        {
            //Debug.Log("LoadingState============complete===========>" + progress.complete);
            //Debug.Log("LoadingState============total============>" + progress.total);
            //Debug.Log("LoadingState============bundleName===========>" + progress.loader.bundleName);

            if (progress.complete == progress.total)
            {
                if (TNManager.isHosting)
                Messenger.Broadcast("LoadOnCompleted");
                VrPlayerController.LoadOnCompleted = true;
             //   Messenger.Broadcast("OnLoadOnCompleted");
                Debug.Log("Broadcast=>LoadOnCompleted");
            }
        }


        void OnSelecter(Scene s, LoadSceneMode l)
        {
            Debug.LogFormat("Element：{0}", PlayerProfile.VElements);

            Debug.LogFormat("Pos：{0}", PlayerProfile.VPos);
            

            if (TNManager.isHosting)
            {
                Debug.Log("host loads......");

                JoObjectMethod(PlayerProfile.VPos, PlayerProfile.VElements);
            }
            else
            { // clients

                Debug.Log("clients loads......");

                JoObjectMethod(PlayerProfile.VPos, PlayerProfile.VElements);
            }
        }
        
        void JoObjectMethod(string myPos, string myElement)
        {
            //Pase Json Pos
            var joPos = JObject.Parse(myPos);
            int pLength = joPos["pos"].Count();
            for (int i = 0; i < pLength; i++)
            {
                //Debug.Log("JsonPosElements=>" + i + "=>" + joPos["pos"][i]["p"]);
                PlayerPrefs.SetString("Vr_Player_Pos" + i, joPos["pos"][i]["p"].ToString());
            }
            //Parse Json Data
            var jo = JObject.Parse(myElement);
            Elementslength = jo["elements"].Count();

            for (int i = 0; i < Elementslength; i++)
            {
                //Debug.Log("JsonElements=>" + i + "=>" + jo["elements"][i]["v"]);

                mManager.Init(() => { LoadObjects(jo["elements"][i]["v"].ToString()); });
            }

        }


        int index = 0;

        void LoadObjects(string objName)
        {
            //Debug.Log("objName=>" + objName);
            
            mManager.Load(objName, (a) =>
            {
                
                a.Instantiate();
                //Search "Dynamic" String
                //Analysis This Main Gameobject
                //Debug.Log("Loaded Name =>"+a.mainObject.name);
                #region Dynamic
                if (a.mainObject.name.Contains(TAG))
                {
                    GameObject go = GameObject.Find(a.mainObject.name);

                    Dynamic[] t = go.GetComponentsInChildren<Dynamic>();

                    //Debug.Log("Go=>" + go.name+"=>Length=>"+t.Length);

                    List<GameObject> tArray = new List<GameObject>();

                    for (int i = 0; i < t.Length; i++)
                    {
                        //Set - Send List
                        tArray.Add(t[i].gameObject);
                    }

                    Messenger.Broadcast<List<GameObject>, string>("RecvDynamicObjects", tArray, a.mainObject.name);
                    return;
                    //Debug.Log("Broadcast=>RecvDynamicObjects");
                }
                #endregion


                //Search "PickUp" component gameobject(host create)
                #region AllHandOBJ
                if (a.mainObject.name.Contains(TAG3))
                {
                    //lwl   找到可拿物体的父对象
                    GameObject go = GameObject.Find(a.mainObject.name);
                    //获取子对象
                    Transform[] trans = go.GetComponentsInChildren<Transform>();
                    Debug.Log("trans count-------------------------------"+trans.Length);
                    for (int i = 0; i < trans.Length; i++)
                    {
                        //重复操作
                        if (trans[i].GetComponent<PickUp>() != null)
                        {
                            if (trans[i].GetComponent<TNObject>() != null && trans[i].GetComponent<CSyncObject>() == null)
                            {

                                var csnc = trans[i].gameObject.AddComponent<CSyncObject>();
                                csnc.enabled = true;
                                csnc.isGrabbable = true;
                               //layer
                                trans[i].gameObject.layer = LayerMask.NameToLayer("CanTake");
                                trans[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                                trans[i].SetParent(null);
                                //broadcast message
                                if (trans[i].GetComponent<CSyncObject>() != null)
                                {

                                    Messenger.Broadcast<string, int>("OnRccCreatHandObj", trans[i].name, index);
                                    index += 1;
                                }
                            }
                        }
                    }
                }
                #endregion


            });

        }

//          if (!a.mainObject.name.Contains(TAG)&& !a.mainObject.name.Contains(TAG2))
//                {
//                    //lwl   找到可难物体的父对象
//                    GameObject go = GameObject.Find(a.mainObject.name);
//        //获取子对象
//        Transform[] trans = go.GetComponentsInChildren<Transform>();
//                    for (int i = 0; i<trans.Length; i++)
//                    {
//                        //重复操作
//                        if (trans[i].GetComponent<PickUp>() != null)
//                        {
//                            if (trans[i].GetComponent<TNObject>() != null && trans[i].GetComponent<CSyncObject>() == null)
//                            {

//                                var csnc = trans[i].gameObject.AddComponent<CSyncObject>();
//        csnc.enabled = true;
//                                csnc.isGrabbable = true;


//                                //layer
//                                trans[i].gameObject.layer = LayerMask.NameToLayer("CanTake");
//                                trans[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
//                                trans[i].SetParent(null);
//                                //broadcast message
//                                if (trans[i].GetComponent<CSyncObject>() != null)
//                                {
                                   
//                                    Messenger.Broadcast<string, int>("OnRccCreatHandObj", trans[i].name, index);
//                                    index += 1;
//                                }
//}
//                        }
                   

//                        //Debug.Log("Pick Go=>" + go.name);
//                    }
//                }

            /// <summary>
            /// 旧版
            /// </summary>
            /// <returns></returns>
        
//                if (!a.mainObject.name.Contains(TAG)&& !a.mainObject.name.Contains(TAG2))
//                {
//                    GameObject go = GameObject.Find(a.mainObject.name);

//                    if (go.GetComponent<PickUp>() != null)
//                    {
//                        if (go.GetComponent<TNObject>() != null && go.GetComponent<CSyncObject>() == null)
//                        {
//                            //var n = go.GetComponent<TNObject>();
//                            ////Set uid
//                            //System.Random ran = new System.Random(GetRandomSeed());
//                            //n.uid = (uint)ran.Next(10000000, 16777215);
//                            ////SetChannelID
//                            //go.GetComponent<TNObject>().channelID = TNManager.lastChannelID;
                            

//                            var csnc = go.AddComponent<CSyncObject>();
//        csnc.enabled = true;
//                            csnc.isGrabbable = true;
                            

//                            //layer
//                            go.layer = LayerMask.NameToLayer("CanTake");
                            
//                            //broadcast message
//                            if (go.GetComponent<CSyncObject>() != null)
//                            {
//                                index += 1;
//                                Messenger.Broadcast<string, int>("OnRccCreatHandObj", go.name, index);
//                            }
//}

//                        //Debug.Log("Pick Go=>" + go.name);
//                    }
//                }
        int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);

        }
        
    }
}

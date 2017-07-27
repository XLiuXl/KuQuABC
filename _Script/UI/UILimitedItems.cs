using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[System.Serializable]public class Items
{
    [SerializeField] public string itemName = string.Empty;
    [SerializeField] public Texture itemImage = null;
    [SerializeField] public bool itemChecked = false;
}

public class UILimitedItems : MonoBehaviour
{

    public GameObject itemPrefab;

    public Items[] myItems;

    private  Dictionary<int,GameObject> cached = new Dictionary<int, GameObject>();

    private bool created = true;

    void OnEnable()
    {
        if (myItems.Length>0&&created)
        {
            created = false;
            for (int i = 0; i < myItems.Length; i++)
            {
                GameObject go = Instantiate(itemPrefab);
                
                go.transform.GetComponentInChildren<UILabel>().text = myItems[i].itemName;
                go.transform.GetComponentInChildren<UITexture>().mainTexture = myItems[i].itemImage;
                go.transform.GetComponentInChildren<UIToggle>().value = myItems[i].itemChecked;

                go.transform.GetComponentInChildren<UIToggle>().gameObject.AddComponent<UISavedOption>();
                go.transform.GetComponentInChildren<UIToggle>().gameObject.GetComponent<UISavedOption>().keyName =
                    "Vr_Mulit_" + myItems[i].itemName;


                EventDelegate.Add(go.transform.GetComponentInChildren<UIToggle>().onChange, OnSelection);

                go.transform.parent = transform;
                go.transform.localScale =Vector3.one;
                go.transform.position = Vector3.zero;

                cached.Add(i,go);

                Debug.Log("Index=>"+i+"-Element=>"+ go.transform.GetComponentInChildren<UILabel>().text);
            }
            
        }
    }

    void OnSelection()
    {
        for (int i = 0; i < cached.Count; i++)
        {
            Debug.Log(myItems[i].itemName+"=>"+"index:"+i+"-"+cached.ElementAt(i).Value.transform.GetComponentInChildren<UIToggle>().value);
        }
    }

    void Start () {
	    
	}
	

}

using UnityEngine;
using System.Collections;
using VRTK.Highlighters;
using System.Collections.Generic;
public class SetObjOutLineH : MonoBehaviour {
    private static GameObject _Instance = null;
    private static SetObjOutLineH thisObj = null;
    private Dictionary<string, object> highlighterOptions;
    private VRTK_BaseHighlighter currentOutLine = null;
    private float outlineTime = 0;
    public static SetObjOutLineH GetInstance()
    {
        if (_Instance==null)
        {
            _Instance = new GameObject("SetObjOutLineH");
          thisObj=_Instance.AddComponent < SetObjOutLineH > ();
        }
        return thisObj;
    }
    // Use this for initialization
    public  void SetHighlitghter(GameObject obj)
    {

        if (currentOutLine != null)
        {
            currentOutLine.Unhighlight();
        }
        currentOutLine = obj.GetComponent<VRTK_BaseHighlighter>();
        //if (currentOutLine==null)
        //{
        //    currentOutLine=obj.AddComponent<VRTK_BaseHighlighter>();

        //}
        currentOutLine.Initialise(null, highlighterOptions);
        currentOutLine.Highlight(Color.red);
    }
	

}

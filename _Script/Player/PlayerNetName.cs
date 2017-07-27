using UnityEngine;
using System.Collections;
using TNet;

public class PlayerNetName : TNBehaviour
{
    /// <summary>
    /// 需要动态更改名称的对象
    /// </summary>
    private GameObject target;
    public TextMesh mTextMesh;

	void Start ()
	{
	    target = this.gameObject;
	    mTextMesh = mTextMesh.GetComponent<TextMesh>();
        SetPlayerName();
	}

    void SetPlayerName()
    {
        if (tno != null)
        {
            //tno.owner.name = PlayerProfile.playerName;
            target.name = tno.owner.name;
            mTextMesh.text = target.name;
            mTextMesh.color = Color.green;
        }
    }
    
}

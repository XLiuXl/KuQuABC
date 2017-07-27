using UnityEngine;
using TNet;
/// <summary>
/// Script controlling player name entries that show up on the left-hand side in the game.
/// </summary>

public class UIPlayerName : MonoBehaviour
{
	public TNet.Player player;
	public UISprite background;
	public UISprite icon;
	public UILabel label;
    public GameObject MarkFollow = null;
	bool mIsVisible = false;

    //public GameObject mMoveable;
    //public GameObject mFollow;
    void Awake()
    {
        
    }
    public void UpdateInfo (bool isVisible)
	{
		if (player != null)
		{
            icon.spriteName = "UI_Login_User";
			label.text = player.name;
            Debug.Log("UIPlayerName =>"+label.text);
		}
		else
		{
			label.text = "[AI]";
			icon.spriteName = "Circle - AI";
		}

		Color c = Color.white;
		icon.color = c;

		c.a = label.alpha;
		label.color = c;

		if (mIsVisible != isVisible)
		{
			mIsVisible = isVisible;
			//TweenAlpha.Begin(icon.gameObject, 0.25f, isVisible ? 0f : 1f).method = UITweener.Method.EaseInOut;
		}
	}
}

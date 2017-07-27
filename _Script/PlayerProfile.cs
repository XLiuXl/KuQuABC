using System;
using System.IO;
using UnityEngine;

public static class PlayerProfile
{
    static string mName;
	static int mFull = -1;
	static int mHints = -1;
	static int mWifi = -1;
	static int mExp = -1;
	static int mPowerSaving = -1;

    public const string mCommonLoaderScene = "Common";

    public const string mDbName = "chocvr.db";
    public const string mDbPassword = "chocvr5201314";

    public const string mSubjet = "Vr_Mulit_Subject";
    public const string mLevel = "Vr_Mulit_Subject_Level";   
    public const string mTitle = "Vr_Mulit_Subject_Title";
    public const string mContext = "Vr_Mulit_Subject_Context";
    public const string mAdjustHeight = "Vr_Mulit_AdjustHeight";
    public const string mSmall = "Vr_Mulit_Small";
    public const string mPos = "Vr_Mulit_Pos";
    public const string mElements = "Vr_Mulit_Elements";
    public const string mScene = "Vr_Mulit_Scene";
    public const string mUseHmd = "Vr_Mulit_HmdHand";
    public const string mUserPw = "Vr_Mulit_RoomPw";
    public const string mMaxPlayer = "Vr_Mulit_Players";

    private static string vSubject = string.Empty;
    private static string vLevel = string.Empty;
    private static string vTitle = string.Empty;
    private static string vContext = string.Empty;
    private static string vAdjustHeight = string.Empty;
    private static string vSmall = string.Empty;
    private static string vPos = string.Empty;
    private static string vElements = string.Empty;
    private static string vScene = string.Empty;
    private static string vUseHmd = string.Empty;
    private static string vUserPw = string.Empty;
    private static string vMaxPlayer = string.Empty;

    public static string VUserPw
    {
        get
        {
            return PlayerPrefs.GetString(mUserPw);
        }

        set
        {
            vUserPw = value;
        }
    }

    public static string VElements
    {
        get
        {
            return PlayerPrefs.GetString(mElements);
        }

        set
        {
            vElements = value;
        }
    }
    public static string VMaxPlayer
    {
        get
        {
            return PlayerPrefs.GetString(mMaxPlayer);
        }

        set
        {
            vMaxPlayer = value;
        }
    }

    public static string VUseHmd
    {
        get
        {
            return PlayerPrefs.GetString(mUseHmd);
        }

        set
        {
            vUseHmd = value;
        }
    }

    public static string VScene
    {
        get
        {
            return PlayerPrefs.GetString(mScene);
        }

        set
        {
            vScene = value;
        }
    }

    public static string VPos
    {
        get
        {
            return PlayerPrefs.GetString(mPos);
        }

        set
        {
            vPos = value;
        }
    }

    public static string VSmall
    {
        get
        {
            return PlayerPrefs.GetString(mSmall);
        }

        set
        {
            vSmall = value;
        }
    }

    public static string VAdjustHeight
    {
        get
        {
            return PlayerPrefs.GetString(mAdjustHeight);
        }

        set
        {
            vAdjustHeight = value;
        }
    }

    public static string VContext
    {
        get
        {
            return PlayerPrefs.GetString(mContext);
        }

        set
        {
            vContext = value;
        }
    }

    public static string VTitle
    {
        get
        {
            return PlayerPrefs.GetString(mTitle);
        }

        set
        {
            vTitle = value;
        }
    }

    public static string VLevel
    {
        get
        {
            return PlayerPrefs.GetString(mLevel);
        }

        set
        {
            vLevel = value;
        }
    }

    public static string VSubjet
    {
        get
        {
            return PlayerPrefs.GetString(mSubjet);
        }

        set
        {
            vSubject = value;
        }
    }


    public enum ClientType
    {
        None,
        Normal,
        VR,
    }
    static public ClientType clientType = ClientType.None;

    /// <summary>
    /// Whether the player should have full access to all of the game's features.
    /// </summary>

    static public bool fullAccess
	{
		get
		{
			if (mFull == -1) mFull = PlayerPrefs.GetInt("Full", 0);
			return (mFull == 1);
		}
		set
		{
			int val = value ? 1 : 0;

			if (mFull != val)
			{
				mFull = val;
				PlayerPrefs.SetInt("Full", val);
				NGUITools.Broadcast("OnAccessLevelChanged");
			}
		}
	}

	/// <summary>
	/// Player's chosen name.
	/// </summary>

	static public string playerName
	{
		get
		{
            //mName = UserSession.username;
            mName = PlayerPrefs.GetString("vr_username");
            return mName;
		}
		set
		{
			if (string.IsNullOrEmpty(value.Trim()))
                value = PlayerPrefs.GetString("vr_username"); ;


            if (mName != value)
			{
				mName = value;
				PlayerPrefs.SetString("vr_username", value);
			}
		}
	}

    
    /// <summary>
	/// Player's experience.
	/// </summary>

	static public int experience
	{
		get
		{
			if (mExp == -1) mExp = PlayerPrefs.GetInt("Experience", 0);
			return mExp;
		}
		set
		{
			if (mExp != value)
			{
				mExp = value;
				PlayerPrefs.SetInt("Experience", value);
			}
		}
	}

	/// <summary>
	/// Whether the player wants hints in single player mode.
	/// </summary>

	static public bool hints
	{
		get
		{
			if (mHints == -1) mHints = PlayerPrefs.GetInt("Hints", 1);
			return mHints == 1;
		}
		set
		{
			int val = value ? 1 : 0;

			if (val != mHints)
			{
				mHints = val;
				PlayerPrefs.SetInt("Hints", mHints);
			}
		}
	}

	/// <summary>
	/// Whether the data will be restricted to a wifi-only connection. If off, 3G/4G/LTE traffic will be allowed.
	/// </summary>

	static public bool allow3G
	{
		get
		{
			if (mWifi == -1) mWifi = PlayerPrefs.GetInt("Wifi", 0);
			return mWifi == 1;
		}
		set
		{
			int i = value ? 1 : 0;

			if (i != mWifi)
			{
				mWifi = i;
				PlayerPrefs.SetInt("Wifi", mWifi);
			}
		}
	}

	/// <summary>
	/// Whether the application is allowed to access the internet.
	/// </summary>

	static public bool allowedToAccessInternet
	{
		get
		{
#if UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
			return allow3G;
#else
			return (allow3G || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
#endif
		}
	}

	/// <summary>
	/// Power-saving mode limits framerate to 30 instead of the usual 60.
	/// </summary>

	static public bool powerSavingMode
	{
		get
		{
			if (mPowerSaving == -1)
			{
#if UNITY_ANDROID || UNITY_IPHONE
				Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 30);
#else
				Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 120);
#endif
				mPowerSaving = (Application.targetFrameRate == 120) ? 1 : 0;
			}
			return (mPowerSaving == 1);
		}
		set
		{
			int val = value ? 1 : 0;
			
			if (mPowerSaving != val)
			{
				mPowerSaving = val;
				Application.targetFrameRate = value ? 120 : 120;
				PlayerPrefs.SetInt("FPS", Application.targetFrameRate);
			}
		}
	}

	/// <summary>
	/// Experience points it takes to obtain each level, mainly here for example purposes.
	/// You can change it to be an experience curve, or some kind of a formula if you wish.
	/// </summary>

	public const int expPerLevel = 50000;

	/// <summary>
	/// Total maximum number of obtainable experience points.
	/// </summary>

	static public int maxExp { get { return expPerLevel * 69; } }

	/// <summary>
	/// Player's ability point cap.
	/// </summary>

	static public int abilityPoints { get { return 12; } }

	/// <summary>
	/// Player's current level.
	/// </summary>

	static public int level { get { return experience / expPerLevel; } }

	/// <summary>
	/// Player's progress toward the next level.
	/// </summary>

	static public float progressToNextLevel { get { return GetProgressToNextLevel(experience); } }

    

    /// <summary>
    /// Calculate the progress toward next level, given the experience.
    /// </summary>

    static public float GetProgressToNextLevel (int exp)
	{
		int lvl = GetLevelByExp(exp) - 1;
		return (float)(exp - lvl * expPerLevel) / expPerLevel;
	}

	/// <summary>
	/// Retrieve a title associated with the specified level.
	/// </summary>

	static public string GetTitle (int lvl) { return Localization.Get("Title " + (lvl / 5)); }

	/// <summary>
	/// Given the experience amount, return the level.
	/// </summary>

	static public int GetLevelByExp (int exp)
	{
		if (exp > maxExp) exp = maxExp;
		return 1 + exp / expPerLevel;
	}

	/// <summary>
	/// Retrieve the title associated with the specified amount of experience.
	/// </summary>

	static public string GetTitleByExp (int exp) { return GetTitle(GetLevelByExp(exp) - 1); }

    public static bool CheckFolder(string p)
    {
        string path = GetLocalPath(p);
        if (Directory.Exists(path))
            return true;
        else
            return false;
    }
    static string GetLocalPath(string relativePath)
    {
        //Dont Include Assetsbundle to Unity Assets Folder
        //Read Out Folder
        string targetPlatform = string.Empty;
        if (RuntimePlatform.WindowsPlayer == Application.platform ||
            RuntimePlatform.WindowsEditor == Application.platform)
        {
            targetPlatform = "StandaloneWindows";
        }
        else if (Application.platform == RuntimePlatform.OSXDashboardPlayer ||
            Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.OSXEditor)
        {
            targetPlatform = "StandaloneOSXIntel";
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            targetPlatform = "Android";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            targetPlatform = "iOS";
        }

        if (string.IsNullOrEmpty(targetPlatform))
        {
            Debug.LogError("TargetPlatform is Null!");
        }

        string outPath = Path.Combine(Environment.CurrentDirectory + "\\Data\\" + targetPlatform, relativePath);

        return outPath;
    }
}

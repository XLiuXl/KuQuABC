using System.Collections;
using System.Collections.Generic;
using SqlCipher4Unity3D;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif


using SQLite.Attribute;
using UnityEngine.Scripting;

public class DbService
{
    public SQLiteConnection _connection;

    public DbService(string DatabaseName,string Password)
    {

        //#if UNITY_STANDALONE
        //        var dbPath = Application.dataPath + "/StreamingAssets/" + DatabaseName;
        //        Debug.LogFormat("StandalonePath==>{0}",dbPath);
        //#endif

        //#if UNITY_EDITOR
        //         dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
        //#else
        //        // check if file exists in Application.persistentDataPath
        //        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        //        if (!File.Exists(filepath))
        //        {
        //            Debug.Log("Database not in Persistent path");
        //            // if it doesn't ->
        //            // open StreamingAssets directory and load the db ->

        //#if UNITY_ANDROID
        //            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
        //            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
        //            // then save to Application.persistentDataPath
        //            File.WriteAllBytes(filepath, loadDb.bytes);
        //#elif UNITY_IOS
        //                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
        //                // then save to Application.persistentDataPath
        //                File.Copy(loadDb, filepath);
        //#elif UNITY_WP8
        //                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
        //                // then save to Application.persistentDataPath
        //                File.Copy(loadDb, filepath);

        //#elif UNITY_WINRT
        //		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
        //		// then save to Application.persistentDataPath
        //		File.Copy(loadDb, filepath);
        //#else
        //	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
        //	// then save to Application.persistentDataPath
        //	File.Copy(loadDb, filepath);

        //#endif

        //            Debug.Log("Database written");
        //        }

        //         dbPath = filepath;
        //#endif

        var dbPath = string.Empty;
        
#if UNITY_EDITOR
        dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else

         dbPath = Application.dataPath + "/StreamingAssets/" + DatabaseName;
       
#endif
        _connection = new SQLiteConnection(dbPath, Password);
        //Debug.Log("Final PATH: " + dbPath);
    }
    
}

[Preserve]
public class LevelType
{
    [PrimaryKey, AutoIncrement]
    public string Type { get; set; }
    public int Seq { get; set; }
}

[Preserve]
public class SubjectType
{
    [PrimaryKey, AutoIncrement]
    public string Type { get; set; }
    public int Seq { get; set; }
}

[Preserve]
public class VrClass
{
    [PrimaryKey,AutoIncrement]
    public int Id { get; set; }
    public string Subject { get; set; }
    public string Level { get; set; }
    public string Title { get; set; }
    public string SceneName { get; set; }
    public string Context { get; set; }
    public string Element { get; set; }
    public string Position { get; set; }
    public string DynamicPath { get; set; }
    public string IsSmall { get; set; }
    public string AdjustHeight { get; set; }
    public string UseHmd { get; set; }
}

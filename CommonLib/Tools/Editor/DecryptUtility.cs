using TNet;
using UnityEditor;
/// <summary>
/// 辅助下载解密
/// 每次使用需要更改input路径和key
/// </summary>
public class DecryptUtility  {
    [MenuItem("Tools/DecryptFile")]
    static void DecryptFile()
    {
        var input = @"F:\Downloads\891a548c-a675-4423-81f1-aa0b6d170a0d";
        var key = "caa61899d2e5dde00894df551dbb4ce07e5091384f0c35372468f650dd5c1a32a6b7e664bc05384a156b21a0fbf41eb9";

        var u = typeof(Editor).Assembly;
        var utils = u.GetType("UnityEditor.AssetStoreUtils");
        utils.Invoke("DecryptFile", input, input + ".unitypackage", key);
    }
}

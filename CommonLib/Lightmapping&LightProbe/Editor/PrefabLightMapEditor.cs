using UnityEngine;
using System.Collections;
using UnityEditor;

public class PrefabLightMapEditor : EditorWindow {

   
    bool checkToggle;

 

    [MenuItem("Window/光影图处理工具")]
    public static void FirstTest()
    {

         EditorWindow.GetWindow(typeof(PrefabLightMapEditor));
    }


    public  string pathToFolder = "Assets/Resources/Lightmaps/";

    int selected = 0;


    TextureImporterFormat[] options = new TextureImporterFormat[]
{
    TextureImporterFormat.Alpha8,
      TextureImporterFormat.ARGB16,
          TextureImporterFormat.ARGB32,
      TextureImporterFormat.DXT1,    TextureImporterFormat.DXT5,
      TextureImporterFormat.ETC2_RGB4,    TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA,
      TextureImporterFormat.ETC2_RGBA8,    TextureImporterFormat.ETC_RGB4,
      TextureImporterFormat.PVRTC_RGB2,    TextureImporterFormat.RGB16,
      TextureImporterFormat.RGBA32,

            TextureImporterFormat.RGBA16,    TextureImporterFormat.RGB24,
      TextureImporterFormat.PVRTC_RGBA4,    TextureImporterFormat.PVRTC_RGBA2,
      TextureImporterFormat.PVRTC_RGB4,

};

   

    string[] options_string = new string[]
{
    "TextureImporterFormat.Alpha8",
      "TextureImporterFormat.ARGB16",
          "TextureImporterFormat.ARGB32",    
      "TextureImporterFormat.DXT1",    "TextureImporterFormat.DXT5",
      "TextureImporterFormat.ETC2_RGB4 bits",    "TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA",
      "TextureImporterFormat.ETC2_RGBA 8 bits",    "TextureImporterFormat.ETC_RGB 4 Bit",
      "TextureImporterFormat.PVRTC_RGB2",    "TextureImporterFormat.RGB16",
      "TextureImporterFormat.RGBA32",

            "TextureImporterFormat.RGBA16",    "TextureImporterFormat.RGB24",
      "TextureImporterFormat.PVRTC_RGBA4",    "TextureImporterFormat.PVRTC_RGBA2",
      "TextureImporterFormat.PVRTC_RGB4"

};

    int textureSizeIndex = 0;
    int[] textureSize = new int[]
    {

        512,1024,2048,4096,8192
    };
    string[] textureSting = new string[]
 {

        "512","1024","2048","4096","8192"
 };

    public string lightMapFix = "LOD";
    void OnGUI()
    {
        GUILayout.Label("预制体烘焙面板（请务必先设置好烘焙参数）", EditorStyles.boldLabel);
        GUILayout.Space(20);
        GUILayout.Label("光影图输出路径:");
        GUILayout.Label(pathToFolder);
        GUILayout.Space(5);

        selected = EditorGUILayout.Popup("选择烘焙贴图格式", selected, options_string);
        GUILayout.Space(5);

        textureSizeIndex = EditorGUILayout.Popup("选择烘焙贴图大小", textureSizeIndex, textureSting);
        GUILayout.Space(5);
        if (GUILayout.Button("第一步 : 烘焙预制体光影图"))
        {
            
            if (!AssetDatabase.IsValidFolder(pathToFolder.Remove(pathToFolder.Length-1)))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Lightmaps");
            }
            EditorApplication.SaveScene();
            PrefabLightmapData.isBakedCompleted = false;
            EditorApplication.update += IsComplete;
            PrefabLightmapData.GenerateLightmapInfo(pathToFolder, options[selected], textureSize[textureSizeIndex]);

        }

        GUILayout.Space(15);

        if (GUILayout.Button("(备选项)第二步: 保存环境反射球"))
        {
            PrefabLightmapData.SaveReflectionProbes();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("(备选项)第三步: 保存光照反射探针"))
        {
            PrefabLightmapData.SaveLightProbeGroup();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("(备选项)第四步: 更新场景中预制体光影图"))
        {


            if (AssetDatabase.IsValidFolder(pathToFolder.Remove(pathToFolder.Length - 1)))
            {

                PrefabLightmapData.UpdateLightmaps();
            }
            else
            {
                Debug.LogError("光影图输出路径错误，请仔细检查光影图路径！");

            }

        }

        GUILayout.Space(20);

        checkToggle = EditorGUILayout.BeginToggleGroup("开启LOD功能", checkToggle);

        GUILayout.Label("添加光影图前缀", EditorStyles.boldLabel);

        lightMapFix = GUILayout.TextField(lightMapFix, 150);

        if (GUILayout.Button("第五步 : 指定光影图到LOD物体"))
        {


            if (AssetDatabase.IsValidFolder(pathToFolder.Remove(pathToFolder.Length - 1)))
            {

                PrefabLightmapData.UseMeBeforeUpdatint(lightMapFix);

            }
            else {
                Debug.Log("光影图输出路径错误，请仔细检查光影图路径！");
            }

        }
        EditorGUILayout.EndToggleGroup();


    }
    void IsComplete()
    {
        if (PrefabLightmapData.isBakedCompleted)
        {
            Debug.Log("bakeCompleted");
            EditorApplication.update -= IsComplete;
            PrefabLightmapData.SaveReflectionProbes();
            PrefabLightmapData.SaveLightProbeGroup();
            PrefabLightmapData.UpdateLightmaps();
            PrefabLightmapData.UseMeBeforeUpdatint(lightMapFix);
            EditorApplication.SaveScene();
           
           
           
        }
    }
}

using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ABSystem
{
    public class AssetBundleBuildPanel : EditorWindow
    {
        [MenuItem("ABSystem/Builder Panel")]
        static void Open()
        {
            GetWindow<AssetBundleBuildPanel>("ABSystem", true);
        }

       
        static void BuildAssetBundles()
        {
            AssetBundleBuildConfig config = LoadAssetAtPath<AssetBundleBuildConfig>(savePath);

            if (config == null)
                return;

#if UNITY_5
            ABBuilder builder = new AssetBundleBuilder5x(new AssetBundlePathResolver());
#else
			ABBuilder builder = new AssetBundleBuilder4x(new AssetBundlePathResolver());
#endif
            builder.SetDataWriter(config.depInfoFileFormat == AssetBundleBuildConfig.Format.Text ? new AssetBundleDataWriter() : new AssetBundleDataBinaryWriter());

            builder.Begin();

            for (int i = 0; i < config.filters.Count; i++)
            {
                AssetBundleFilter f = config.filters[i];
                if (f.valid)
                    builder.AddRootTargets(new DirectoryInfo(f.path), new string[] { f.filter });
            }

            builder.Export();
            builder.End();
        }

        static T LoadAssetAtPath<T>(string path) where T: UnityEngine.Object
        {
#if UNITY_5
			return AssetDatabase.LoadAssetAtPath<T>(savePath);
#else
			return (T)AssetDatabase.LoadAssetAtPath(savePath, typeof(T));
#endif
		}

        const string savePath = "Assets/ABSystem/config.asset";

        private AssetBundleBuildConfig _config;
        private ReorderableList _list;
        private Vector2 _scrollPosition = Vector2.zero;

        AssetBundleBuildPanel()
        {

        }

        void OnListElementGUI(Rect rect, int index, bool isactive, bool isfocused)
        {
            const float GAP = 5;

            AssetBundleFilter filter = _config.filters[index];
            rect.y++;

            Rect r = rect;
            r.width = 16;
            r.height = 18;
            filter.valid = GUI.Toggle(r, filter.valid, GUIContent.none);

            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax - 300;
            GUI.enabled = false;
            filter.path = GUI.TextField(r, filter.path);
            GUI.enabled = true;

            r.xMin = r.xMax + GAP;
            r.width = 50;
            if (GUI.Button(r, "Select"))
            {
                var path = SelectFolder();
                if (path != null)
                    filter.path = path;
            }

            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax;
            filter.filter = GUI.TextField(r, filter.filter);
        }

        string SelectFolder()
        {
            string dataPath = Application.dataPath;
            string selectedPath = EditorUtility.OpenFolderPanel("Path", dataPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.StartsWith(dataPath))
                {
                    return "Assets/" + selectedPath.Substring(dataPath.Length + 1);
                }
                else
                {
                    ShowNotification(new GUIContent("不能在Assets目录之外!"));
                }
            }
            return null;
        }

        void OnListHeaderGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, "Asset Filter");
        }

        void InitConfig()
        {
            _config = LoadAssetAtPath<AssetBundleBuildConfig>(savePath);
            if (_config == null)
            {
                _config = new AssetBundleBuildConfig();
            }
        }

        void InitFilterListDrawer()
        {
            _list = new ReorderableList(_config.filters, typeof(AssetBundleFilter));
            _list.drawElementCallback = OnListElementGUI;
            _list.drawHeaderCallback = OnListHeaderGUI;
            _list.draggable = true;
            _list.elementHeight = 22;
            _list.onAddCallback = (list) => Add();
        }

        void Add()
        {
            string path = SelectFolder();
            if (!string.IsNullOrEmpty(path))
            {
                var filter = new AssetBundleFilter();
                filter.path = path;
                _config.filters.Add(filter);
            }
        }

        void OnGUI()
        {
            if (_config == null)
            {
                InitConfig();
            }

            if (_list == null)
            {
                InitFilterListDrawer();
            }

            bool execBuild = false;
            //tool bar
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                //if (GUILayout.Button("Add", EditorStyles.toolbarButton))
                //{
                //    Add();
                //}
                //if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                //{
                //    Save();
                //}
                

                GUILayout.FlexibleSpace();
                //if (GUILayout.Button("Build", EditorStyles.toolbarButton))
                //{
                //    execBuild = true;
                //}
            }
            GUILayout.EndHorizontal();

            //context
            GUILayout.BeginVertical();
            {
                //format
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("依赖文件数据格式");
                    _config.depInfoFileFormat = (AssetBundleBuildConfig.Format)EditorGUILayout.EnumPopup(_config.depInfoFileFormat);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                //Filter item list
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
                {
                    _list.DoLayoutList();
                }

                GUILayout.Space(10);

                ABConfig.level = (LEVEL)EditorGUILayout.EnumPopup("选择课程等级", ABConfig.level, GUILayout.MaxHeight(25));
                EditorGUILayout.Separator();

                string sceneName = string.Empty;
                string titleName = string.Empty;

                if (GUILayout.Button("开始查询数据库！"))
                {
                    ABConfig.cSceneNames.Clear(); ABConfig.cTitleNames.Clear();
                    InitDbForLevel((int)ABConfig.level);
                    

                    for (int k = 0; k < ABConfig.cSceneNames.Count; k++)
                    {
                        if (ABConfig.sceneIndex == k) sceneName = ABConfig.cSceneNames[k];
                    }

                    for (int m = 0; m < ABConfig.cTitleNames.Count; m++)
                    {
                        if (ABConfig.titleIndex == m) titleName = ABConfig.cTitleNames[m];
                    }
                    
                }
                

                EditorGUILayout.Separator();

                ABConfig.sceneIndex = EditorGUILayout.Popup(ABConfig.sceneIndex, ABConfig.cSceneNames.ToArray(), GUILayout.MaxHeight(25));

                ABConfig.titleIndex = EditorGUILayout.Popup(ABConfig.titleIndex, ABConfig.cTitleNames.ToArray(), GUILayout.MaxHeight(25));

                if (GUILayout.Button("生成编译路径"))
                {

                    for (int k = 0; k < ABConfig.cSceneNames.Count; k++)
                    {
                        if (ABConfig.sceneIndex == k) sceneName = ABConfig.cSceneNames[k];
                    }

                    for (int m = 0; m < ABConfig.cTitleNames.Count; m++)
                    {
                        if (ABConfig.titleIndex == m) titleName = ABConfig.cTitleNames[m];
                    }

                    string outPath = ABConfig.level.ToString() + "/" + sceneName + "/" + titleName;

                    AssetBundlePathResolver.instance = new AssetBundlePathResolver();
                    PlayerPrefs.SetString("BundleDir", outPath);

                    Debug.LogFormat("BundleSaveDirName：{0}", AssetBundlePathResolver.instance.BundleSavePath);

                    if (!Directory.Exists(AssetBundlePathResolver.instance.BundleSavePath) && ABConfig.level != LEVEL.None)
                        Directory.CreateDirectory(AssetBundlePathResolver.instance.BundleSavePath);
                    
                }

                EditorGUILayout.Separator();

                if (GUILayout.Button("导出数据包"))
                {
                    execBuild = true;
                }

                EditorGUILayout.Space();

                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();

           

          

            //set dirty
            if (GUI.changed)
                EditorUtility.SetDirty(_config);

            if (execBuild)
                Build();
        }

        private void Build()
        {
            Save();
            BuildAssetBundles();
        }

        void Save()
        {
            AssetBundlePathResolver.instance = new AssetBundlePathResolver();

            if (LoadAssetAtPath<AssetBundleBuildConfig>(savePath) == null)
            {
                AssetDatabase.CreateAsset(_config, savePath);
            }
            else
            {
                EditorUtility.SetDirty(_config);
            }
        }

        static void InitDbForLevel(int index)
        {
            var ds = new DbService(DbConfig.mDbName, DbConfig.mDbPassword);
            var t = ds._connection.Table<VrClass>().GetEnumerator();

            while (t.MoveNext())
            {

                string lv = t.Current.Level.Substring(5);//Level
                

                if (!string.IsNullOrEmpty(t.Current.SceneName) && index == Convert.ToInt16(lv))
                {
                    if (!ABConfig.cSceneNames.Contains(t.Current.SceneName)) ABConfig.cSceneNames.Add(t.Current.SceneName);
                    if (!ABConfig.cTitleNames.Contains(t.Current.Title)) ABConfig.cTitleNames.Add(t.Current.Title);
                }
            }

            t.Dispose();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABSystem
{
    public static class ABConfig
    {

        public static LEVEL level = LEVEL.None;

        public static int sceneIndex = 0;
        public static int titleIndex = 0;

        [SerializeField]
        public static List<string> cSceneNames = new List<string>();
        [SerializeField]
        public static List<string> cTitleNames = new List<string>();
    }

    public enum LEVEL
    {
        None,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7,
        Level8,
        Level9
    }
}

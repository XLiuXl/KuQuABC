using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VrNet.Logic
{

    public enum Subject
    {
        /// <summary>
        /// 语文
        /// </summary>
        Chinese,
        /// <summary>
        /// 英文
        /// </summary>
        English,
        /// <summary>
        /// 数学
        /// </summary>
        Mathematics,
        /// <summary>
        /// 音乐
        /// </summary>
        Music,
        /// <summary>
        /// 艺术
        /// </summary>
        Art,
        /// <summary>
        /// 体育
        /// </summary>
        PE,
        /// <summary>
        /// 品德教育
        /// </summary>
        MoralEducation,
        None
    }
    public enum Level : int
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5,
        Level6 = 6,
        Level7 = 7,
        Level8 = 8,
        Level9 = 9,
        None
    }
    public class VrClass
    {
        /// <summary>
        /// 学科
        /// </summary>
        public Subject MySubject;
        /// <summary>
        /// 等级
        /// </summary>
        public Level MyLevel;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title;
        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName;
        /// <summary>
        /// 课程提纲
        /// </summary>
        public string Context;

        /// <summary>
        /// 课程元素
        /// </summary>
        public string Element;
    }
}

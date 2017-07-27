using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Choc.Native.Win32
{
    public static class Win32
    {
        /// <summary>
        /// 查询指定的窗口句柄(判断!=IntPtr.Zero&&IsWnd)
        /// </summary>
        /// <param name="lpClassName">类名</param>
        /// <param name="lpWindowName">窗口名称</param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        /// <summary>
        /// 查询子窗口句柄
        /// </summary>
        /// <param name="hwndParent"></param>
        /// <param name="hwndChildAfter"></param>
        /// <param name="lpszClass"></param>
        /// <param name="lpszWindow"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        /// <summary>
        /// 发送窗口消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        /// <summary>
        /// 窗口关闭消息
        /// </summary>
        public const int WM_CLOSE = 0x0010;
        /// <summary>
        /// 检查是否为窗口
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        public static extern bool IsWindow(IntPtr hWnd);

        /// <summary>
        /// 打开外部程序
        /// </summary>
        /// <param name="filename">完整文件路径,包括名称和后缀</param>
        /// <param name="arg">参数</param>
        public static void Run(string filename,string arg)
        {
            if (File.Exists(filename))
            {
                try
                {
                    Process.Start(filename, arg);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.Log("error:" + ex);
                }
            }
            else {
                UnityEngine.Debug.LogError("error:dont find target program!");
                return;
            }

          
        }
    }
}

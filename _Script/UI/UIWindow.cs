using UnityEngine;

namespace VrNet.UICommon
{
    /// <summary>
    /// This class facilitates an easy way of switching between different windows.
    /// Use UIWindow.Show(panel) to show a window and UIWindow.GoBack() to go back to the previous one.
    /// </summary>

    public class UIWindow : MonoBehaviour
    {
        /// <summary>
        /// Sound to play when showing any window.
        /// </summary>

        static public AudioClip showSound;

        /// <summary>
        /// Sound to play when hiding any window.
        /// </summary>

        static public AudioClip hideSound;

        static UIWindow mInst;
        static bool mIgnoreEnable = false;
        static BetterList<UIPanel> mHistory = new BetterList<UIPanel>();
        static BetterList<UIPanel> mFading = new BetterList<UIPanel>();
        static UIPanel mActive;

        /// <summary>
        /// Currently visible window.
        /// </summary>

        static public UIPanel current { get { return mActive; } }

        /// <summary>
        /// Ensure we have an instance to work with.
        /// </summary>

        static void CreateInstance()
        {
            if (mInst == null)
            {
                mIgnoreEnable = true;
                GameObject go = new GameObject("_UIWindow");
                mInst = go.AddComponent<UIWindow>();
                DontDestroyOnLoad(go);
                mIgnoreEnable = false;
            }
        }

        /// <summary>
        /// Ensure that the specified window has been added to the list.
        /// </summary>

        static public void Add(UIPanel window)
        {
            if (mActive == window) return;

            CreateInstance();

            if (mActive == null)
                mActive = window;
        }

        /// <summary>
        /// Show the specified window.
        /// </summary>

        static public void Show(UIPanel window)
        {
            if (mActive == window) return;

            CreateInstance();

            if (mActive != null)
            {
                if (showSound != null) NGUITools.PlaySound(hideSound);
                mFading.Add(mActive);
                mHistory.Add(mActive);
            }

            if (mHistory.Remove(window))
            {
                mFading.Remove(window);
            }
            else if (window != null)
            {
                window.alpha = 0f;
            }

            mActive = window;

            if (mActive != null)
            {
                mActive.alpha = 0f;
                mActive.transform.localScale = Vector3.one * 0.9f;
                mActive.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Hide the specified window, but only if the window is currently visible. If it's not, do nothing.
        /// </summary>

        static public void Hide(UIPanel window) { if (mActive == window) GoBack(); }

        /// <summary>
        /// Return to the previous window.
        /// </summary>

        static public void GoBack()
        {
            CreateInstance();

            if (mHistory.size > 0)
            {
                if (mActive != null)
                {
                    if (showSound != null) NGUITools.PlaySound(hideSound);
                    mFading.Add(mActive);
                    mActive = null;
                }

                while (mActive == null)
                {
                    mActive = mHistory.Pop();

                    if (mActive != null)
                    {
                        mActive.alpha = 0f;
                        mActive.gameObject.SetActive(true);
                        mFading.Remove(mActive);
                        break;
                    }
                }
            }
            else Close();
        }

        /// <summary>
        /// Hide the current window and clear the history.
        /// </summary>

        static public void Close()
        {
            if (mActive != null)
            {
                CreateInstance();
                if (showSound != null) NGUITools.PlaySound(hideSound);
                mFading.Add(mActive);
                mActive = null;
            }
            mHistory.Clear();
        }

        /// <summary>
        /// Hide the current window and clear the history.
        /// </summary>

        static public void Close(GameObject go)
        {
            if (go != null)
            {
                UIPanel p = NGUITools.FindInParents<UIPanel>(go);
                if (p != null) Close(p);
                else Debug.LogWarning("Unable to find a panel on " + go.name);
            }
        }

        /// <summary>
        /// Hide the current window and clear the history.
        /// </summary>

        static public void Close(UIPanel panel)
        {
            if (NGUITools.GetActive(panel))
            {
                if (mActive == null) mActive = panel;
                if (mActive == panel) Close();
            }
        }

        /// <summary>
        /// Add this window to the list.
        /// </summary>

        void OnEnable()
        {
            if (!mIgnoreEnable && mInst != this)
            {
                UIPanel panel = GetComponent<UIPanel>();
                if (panel != null) UIWindow.Add(panel);
            }
        }

        /// <summary>
        /// Do the actual fading of panels.
        /// </summary>

        void Update()
        {
            if (mInst != this) return;

            // Fade out the previous window
            for (int i = mFading.size; i > 0;)
            {
                UIPanel p = mFading[--i];

                if (p != null)
                {
                    p.transform.localScale = Vector3.Lerp(Vector3.one * 0.9f, Vector3.one, p.alpha);
                    p.alpha = Mathf.Clamp01(p.alpha - RealTime.deltaTime * 6f);
                    if (p.alpha > 0f) continue;
                    else p.gameObject.SetActive(false);
                }
                mFading.RemoveAt(i);
            }

            // Only fade in the new window after the previous has faded out
            if (mFading.size == 0 && mActive != null && mActive.alpha < 1f)
            {
                if (mActive.alpha == 0f && showSound != null) NGUITools.PlaySound(showSound);
                Transform t = mActive.transform;
                t.localScale = Vector3.Lerp(Vector3.one * 0.9f, Vector3.one, mActive.alpha);

                float alpha = Mathf.Clamp01(mActive.alpha + RealTime.deltaTime * 6f);
                mActive.alpha = alpha;

                if (alpha == 1f)
                {
                    t.localScale = Vector3.one;
                    StartCoroutine(MarkAsChangedCoroutine(t));
                }

                // IMPORTANT: Starlink-specific code. You will most likely want to remove this in your game!
                if (mActive.gameObject.layer == 10)
                    t.localRotation = Quaternion.Euler(0f, -18f + mActive.alpha * 24f, 0f);
            }
        }

        /// <summary>
        /// For some reason I couldn't discern, the content of scroll views located within windows
        /// become blurry and never get un-blurry unless widgets get marked as changed.
        /// Oddly enough, this only has any effect if it's done a frame after the tween finishes,
        /// or it has absolutely no effect. Bizarre...
        /// </summary>

        System.Collections.IEnumerator MarkAsChangedCoroutine(Transform t)
        {
            yield return null;
            if (t) t.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
        }
    }
}

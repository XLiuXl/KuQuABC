using UnityEngine;
using TNet;
using VrNet.UICommon;
using VrNet.NetLogic;

namespace NetVr.UICommon
{
    [RequireComponent(typeof(UIGrid))]
    public class UIServerLists : MonoBehaviour
    {
        public GameObject itemPrefab;
        public UILabel status;

        UIGrid mGrid;
        TNLobbyClient mClient;
        List<UIServerListItems> mItems = new List<UIServerListItems>();

        void Awake() { mGrid = GetComponent<UIGrid>(); }

        /// <summary>
        /// 查找Lobby客户端并注册
        /// </summary>

        void OnEnable()
        {
            TNLobbyClient.onChange = OnListChanged;
            OnListChanged();
        }

        void OnDisable() { TNLobbyClient.onChange = null; }

        /// <summary>
        /// Add a new (or update an existing) server entry.
        /// </summary>

        bool Add(ServerList.Entry ent)
        {
            for (int i = 0; i < mItems.size; ++i)
            {
                UIServerListItems item = mItems[i];

                if (item.entry == ent)
                {
                    item.isValid = true;
                    item.MarkAsDirty();
                    return false;
                }
            }

            GameObject go = NGUITools.AddChild(gameObject, itemPrefab);
            UIServerListItems si = go.GetComponent<UIServerListItems>();
            si.entry = ent;
            si.MarkAsDirty();
            si.isValid = true;
            mItems.Add(si);
            return true;
        }

        /// <summary>
        /// When the list changes, we need to create new server list entries and remove expired ones.
        /// </summary>

        void OnListChanged()
        {
            bool changed = false;

            // Mark the entire list as invalid so that we can keep track of what has been removed
            for (int i = 0; i < mItems.size; ++i)
                mItems[i].isValid = false;

            // Add all servers to the list
            for (int i = 0; i < TNLobbyClient.knownServers.list.size; ++i)
            {
                ServerList.Entry ent = TNLobbyClient.knownServers.list[i];
                //Debug.Log("ServerName:"+ent.name+"-ip:"+ent.externalAddress);
                changed |= Add(ent);
            }

            // Remove expired entries
            for (int i = mItems.size; i > 0;)
            {
                UIServerListItems item = mItems[--i];

                if (!item.isValid)
                {
                    if (GameManager.server == item.entry)
                        GameManager.server = null;
                    mItems.RemoveAt(i);
                    NGUITools.Destroy(item.gameObject);
                    changed = true;
                }
            }

            // The list has changed -- reposition everything.
            mGrid.Reposition();

            if (mItems.size == 0)
            {
                status.text = Localization.Get(TNLobbyClient.isActive ? "No servers" : "Searching");
                status.enabled = true;
                return;
            }
            status.enabled = false;
        }
    }

}
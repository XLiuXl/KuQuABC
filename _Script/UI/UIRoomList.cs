using UnityEngine;
using System.Collections;
using TNet;

namespace  VrNet.NetLogic
{
    [RequireComponent(typeof(UIGrid))]
    public class UIRoomList : MonoBehaviour
    {

        /// <summary>
        /// Info label used to display "no games available" message.
        /// </summary>

        public UILabel infoLabel;

        /// <summary>
        /// Prefab object for a single row.
        /// </summary>

        public GameObject rowPrefab;

        // When the next channel list request will be sent out
        // float mNextUpdate = 0f;
        UIGrid mGrid;

        // List of all instantiated rows
        List<UIRoomListItem> mList = new List<UIRoomListItem>();

        void Awake() { mGrid = GetComponent<UIGrid>(); }
        void OnDisable() { if (TNManager.client != null) TNManager.client.packetHandlers[(byte)Packet.ResponseChannelList] = null; }

        void Start()
        {
            infoLabel.text = Localization.Get(TNManager.isConnected ? "Retrieving" : "Not connected");
            TNManager.GetChannelList(OnGetChannels);
        }

        void OnGetChannels(List<Channel.Info> list)
        {
            bool changed = false;

            // Invalidate all entries
            for (int i = 0; i < mList.size; ++i)
                mList[i].isValid = false;

            if (list.size > 0)
            {
                for (int i = 0; i < list.size; ++i)
                {
                    if (Add(list[i])) changed = true;
                }
            }

            // Remove expired entries
            for (int i = mList.size; i > 0;)
            {
                UIRoomListItem ch = mList[--i];

                if (!ch.isValid)
                {
                    changed = true;
                    mList.RemoveAt(i);
                    NGUITools.Destroy(ch.gameObject);
                }
            }

            // Reposition all entries
            if (changed) mGrid.Reposition();

            if (mList.size == 0)
            {
                infoLabel.text = Localization.Get("No rooms");
                infoLabel.enabled = true;
            }
            else infoLabel.enabled = false;
        }

        /// <summary>
        /// Add a new channel entry. Returns 'true' if this was a new entry, 'false' if it updated an existing entry.
        /// </summary>
        /// 

        bool Add(Channel.Info info)
        {
            for (int i = 0; i < mList.size; ++i)
            {
                UIRoomListItem ch = mList[i];

                if (ch.id == info.id)
                {
                    // This is an existing item.  Update it.
                    ch.Set(info);
                    return false;
                }
            }

            GameObject go = NGUITools.AddChild(gameObject, rowPrefab);
            UIRoomListItem ent = go.GetComponent<UIRoomListItem>();
            ent.gameObject.transform.localPosition = new Vector3(0f,-75f,0f);
            ent.id = info.id;
            ent.Set(info);
            mList.Add(ent);
            return true;
        }
    }
}

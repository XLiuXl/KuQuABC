using UnityEngine;

namespace VrNet.UICommon
{

    /// <summary>
    /// This script visually animates gained experience and shows the player's current level and title.
    /// </summary>

    public class UIPlayerExperience : MonoBehaviour
    {
        public UISlider slider;
        public UILabel label;
        public UILabel title;
        public Animation anim;
        public UILabel expLabel;

        int mExp = 0;

        void Start()
        {
            mExp = PlayerProfile.experience;
            UpdateExp(mExp);
        }

        void UpdateExp(int exp)
        {
            if (label != null) label.text = PlayerProfile.GetLevelByExp(exp).ToString();
            if (title != null) title.text = PlayerProfile.GetTitleByExp(exp);
            if (slider != null) slider.value = PlayerProfile.GetProgressToNextLevel(exp);
            if (expLabel != null) expLabel.text = exp.ToString("#,##0");
        }

        void Update()
        {
            // If the experience matches, do nothing
            if (mExp == PlayerProfile.experience) return;

            if (mExp < PlayerProfile.experience)
            {
                // Experience is being gained -- gain it gradually
                int prevLevel = PlayerProfile.GetLevelByExp(mExp);
                mExp += Mathf.RoundToInt(PlayerProfile.expPerLevel * RealTime.deltaTime);
                if (mExp > PlayerProfile.experience) mExp = PlayerProfile.experience;
                int nextLevel = PlayerProfile.GetLevelByExp(mExp);

                // Level up! Play the animation, drawing the player's attention
                if (prevLevel != nextLevel)
                {
                    if (anim != null && !anim.isPlaying) anim.Play();
                    UIStatus.Show(string.Format(Localization.Get("Level up"), nextLevel));
                }
                UpdateExp(mExp);
            }
            else
            {
                if (anim != null && !anim.isPlaying) anim.Play();
                mExp = PlayerProfile.experience;
                UpdateExp(mExp);
            }
        }
    }
}

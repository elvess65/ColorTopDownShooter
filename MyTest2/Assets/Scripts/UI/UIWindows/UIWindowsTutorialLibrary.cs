using System.Collections.Generic;
using UnityEngine;

namespace mytest2.UI
{
    public class UIWindowsTutorialLibrary : MonoBehaviour
    {
        public static UIWindowsTutorialLibrary Instance;
        public Windows.UIWindow_Tutorial TutorialWindowPrefab;
        public TutorialWindowListEntry[] TutorialWindowsList;

        public enum TutorialWindowTypes
        {
            Invitation,
            FinishRound,
            FinishRoundObstacle,
            Assistant,
            DisableObstacle,
            Light,
            CameraRotation,
            RepairObstacle
        }

        private Dictionary<TutorialWindowTypes, Sprite> m_Sprites;

        private void Awake()
        {
            Instance = this;
            m_Sprites = new Dictionary<TutorialWindowTypes, Sprite>();

            for (int i = 0; i < TutorialWindowsList.Length; i++)
            {
                if (!m_Sprites.ContainsKey(TutorialWindowsList[i].Type))
                    m_Sprites.Add(TutorialWindowsList[i].Type, TutorialWindowsList[i].WindowSprite);
            }
        }

        public Sprite GetWindowSpriteByType(TutorialWindowTypes type)
        {
            if (m_Sprites.ContainsKey(type))
                return m_Sprites[type];

            return null;
        }

        [System.Serializable]
        public class TutorialWindowListEntry
        {
            public TutorialWindowTypes Type;
            public Sprite WindowSprite;
        }
    }
}

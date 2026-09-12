/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            27-Apr-26
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using UnityEngine;

namespace Tayx.Graphy.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class G_SafeArea : MonoBehaviour
    {
        [SerializeField] private bool m_conformX = true;  // Conform to screen safe area on X-axis (default true, disable to ignore)
        [SerializeField] private bool m_conformY = true;  // Conform to screen safe area on Y-axis (default true, disable to ignore)
        
        private RectTransform m_rectTransform;
        private Rect m_lastSafeArea = new Rect (0, 0, 0, 0);
        
#if UNITY_EDITOR
        private DrivenRectTransformTracker m_rectTransformTracker;
#endif

        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform> ();

            Refresh();
        }

        private void Update()
        {
            Refresh();
        }
        
#if UNITY_EDITOR
        private void OnDisable()
        {
            m_rectTransformTracker.Clear();
        }
#endif

        private void Refresh()
        {
#if UNITY_EDITOR
            // Make the rectTransform not editable in the inspector.
            m_rectTransformTracker.Clear();
            m_rectTransformTracker.Add(
                this,
                m_rectTransform,
                DrivenTransformProperties.AnchoredPosition
                | DrivenTransformProperties.SizeDelta
                | DrivenTransformProperties.AnchorMin
                | DrivenTransformProperties.AnchorMax
                | DrivenTransformProperties.Pivot
            );
#endif
            
            Rect safeArea = Screen.safeArea;

            if (safeArea != m_lastSafeArea)
                ApplySafeArea (safeArea);
        }

        private void ApplySafeArea(Rect r)
        {
            m_lastSafeArea = r;

            // Ignore x-axis?
            if (!m_conformX)
            {
                r.x = 0;
                r.width = Screen.width;
            }

            // Ignore y-axis?
            if (!m_conformY)
            {
                r.y = 0;
                r.height = Screen.height;
            }

            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            m_rectTransform.anchorMin = anchorMin;
            m_rectTransform.anchorMax = anchorMax;
        }
    }
}

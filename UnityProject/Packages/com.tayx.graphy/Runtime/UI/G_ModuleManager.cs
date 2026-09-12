/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            23-Mar-26
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using UnityEngine;
using System.Collections.Generic;

namespace Tayx.Graphy.UI
{
    public abstract class G_ModuleManager : MonoBehaviour, IMovable, IModifiableState
    {
        #region Variables -> Protected

        protected GraphyManager m_graphyManager = null;

        protected RectTransform m_rectTransform = null;
        protected Vector2 m_origPosition = Vector2.zero;
        protected Vector3 m_origScale = Vector3.one;
        protected Vector2 m_unscaledPosition = Vector2.zero;
        protected float m_scale = 1f;
        protected bool m_isFreePosition = false;

        protected List<GameObject> m_childrenGameObjects = new List<GameObject>();

        protected GraphyManager.ModuleState m_previousModuleState = GraphyManager.ModuleState.FULL;
        protected GraphyManager.ModuleState m_currentModuleState = GraphyManager.ModuleState.FULL;

        #endregion

        #region Methods -> Public

        public virtual void SetPosition( GraphyManager.ModulePosition newModulePosition, Vector2 offset )
        {
            if( newModulePosition == GraphyManager.ModulePosition.FREE )
            {
                m_isFreePosition = true;
                return;
            }

            m_isFreePosition = false;

            float xSideOffset = Mathf.Abs( m_origPosition.x ) + offset.x;
            float ySideOffset = Mathf.Abs( m_origPosition.y ) + offset.y;

            switch( newModulePosition )
            {
                case GraphyManager.ModulePosition.TOP_LEFT:

                    m_rectTransform.anchorMax = Vector2.up;
                    m_rectTransform.anchorMin = Vector2.up;
                    m_rectTransform.pivot = Vector2.up;
                    m_unscaledPosition = new Vector2( xSideOffset, -ySideOffset );

                    break;

                case GraphyManager.ModulePosition.TOP_RIGHT:

                    m_rectTransform.anchorMax = Vector2.one;
                    m_rectTransform.anchorMin = Vector2.one;
                    m_rectTransform.pivot = Vector2.one;
                    m_unscaledPosition = new Vector2( -xSideOffset, -ySideOffset );

                    break;

                case GraphyManager.ModulePosition.BOTTOM_LEFT:

                    m_rectTransform.anchorMax = Vector2.zero;
                    m_rectTransform.anchorMin = Vector2.zero;
                    m_rectTransform.pivot = Vector2.zero;
                    m_unscaledPosition = new Vector2( xSideOffset, ySideOffset );

                    break;

                case GraphyManager.ModulePosition.BOTTOM_RIGHT:

                    m_rectTransform.anchorMax = Vector2.right;
                    m_rectTransform.anchorMin = Vector2.right;
                    m_rectTransform.pivot = Vector2.right;
                    m_unscaledPosition = new Vector2( -xSideOffset, ySideOffset );

                    break;
            }

            ApplyScale();
        }

        public void SetState( GraphyManager.ModuleState state, bool silentUpdate = false )
        {
            if( !silentUpdate )
            {
                m_previousModuleState = m_currentModuleState;
            }

            m_currentModuleState = state;

            ApplyModuleState( state );
        }

        public void RestorePreviousState()
        {
            SetState( m_previousModuleState );
        }

        public void SetScale( float scale )
        {
            m_scale = scale;
            ApplyScale();
        }

        public abstract void UpdateParameters();

        public abstract void RefreshParameters();

        #endregion

        #region Methods -> Protected

        protected abstract void ApplyModuleState( GraphyManager.ModuleState state );

        protected void InitBase()
        {
            m_graphyManager = transform.root.GetComponentInChildren<GraphyManager>();

            m_rectTransform = GetComponent<RectTransform>();
            m_origPosition = m_rectTransform.anchoredPosition;
            m_origScale = m_rectTransform.localScale;
            m_unscaledPosition = m_origPosition;

            foreach( Transform child in transform )
            {
                if( child.parent == transform )
                {
                    m_childrenGameObjects.Add( child.gameObject );
                }
            }
        }

        #endregion

        #region Methods -> Private

        private void ApplyScale()
        {
            m_rectTransform.localScale = m_origScale * m_scale;

            if( !m_isFreePosition )
            {
                m_rectTransform.anchoredPosition = m_unscaledPosition * m_scale;
            }
        }

        #endregion
    }
}

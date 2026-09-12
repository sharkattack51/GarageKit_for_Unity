/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            03-Jan-18
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using UnityEngine;
using System.Collections.Generic;
using Tayx.Graphy.UI;
using Tayx.Graphy.Utils;
using UnityEngine.UI;

namespace Tayx.Graphy.Ram
{
    public class G_RamManager : G_ModuleManager
    {
        #region Variables -> Serialized Private

        [SerializeField] private GameObject m_ramGraphGameObject = null;

        [SerializeField] private List<Image> m_backgroundImages = new List<Image>();

        #endregion

        #region Variables -> Private

        private G_RamGraph m_ramGraph = null;
        private G_RamText m_ramText = null;

        #endregion

        #region Methods -> Unity Callbacks

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            UpdateParameters();
        }

        #endregion

        #region Methods -> Public

        public override void UpdateParameters()
        {
            UpdateBackground();
            UpdateGraphParameters();
            UpdateTextParameters();
        }

        public override void RefreshParameters()
        {
            UpdateParameters();
        }

        public void UpdateBackground()
        {
            foreach( var image in m_backgroundImages )
            {
                image.color = m_graphyManager.BackgroundColor;
            }

            if( !m_graphyManager.Background )
            {
                m_backgroundImages.SetAllActive( false );
                return;
            }

            switch( m_currentModuleState )
            {
                case GraphyManager.ModuleState.FULL:
                    m_backgroundImages.SetOneActive( 0 );
                    break;

                case GraphyManager.ModuleState.TEXT:
                case GraphyManager.ModuleState.BASIC:
                    m_backgroundImages.SetOneActive( 1 );
                    break;

                default:
                    m_backgroundImages.SetAllActive( false );
                    break;
            }
        }

        public void UpdateGraphParameters()
        {
            m_ramGraph.UpdateParameters();
        }

        public void UpdateGraphColors()
        {
            m_ramGraph.UpdateColors();
        }

        public void UpdateTextParameters()
        {
            m_ramText.UpdateParameters();
        }

        #endregion

        #region Methods -> Protected Override

        protected override void ApplyModuleState( GraphyManager.ModuleState state )
        {
            switch( state )
            {
                case GraphyManager.ModuleState.FULL:
                    gameObject.SetActive( true );
                    m_childrenGameObjects.SetAllActive( true );
                    SetGraphActive( true );

                    if( m_graphyManager.Background )
                    {
                        m_backgroundImages.SetOneActive( 0 );
                    }
                    else
                    {
                        m_backgroundImages.SetAllActive( false );
                    }

                    break;

                case GraphyManager.ModuleState.TEXT:
                case GraphyManager.ModuleState.BASIC:
                    gameObject.SetActive( true );
                    m_childrenGameObjects.SetAllActive( true );
                    SetGraphActive( false );

                    if( m_graphyManager.Background )
                    {
                        m_backgroundImages.SetOneActive( 1 );
                    }
                    else
                    {
                        m_backgroundImages.SetAllActive( false );
                    }

                    break;

                case GraphyManager.ModuleState.BACKGROUND:
                    gameObject.SetActive( true );
                    SetGraphActive( false );

                    m_childrenGameObjects.SetAllActive( false );
                    m_backgroundImages.SetAllActive( false );

                    break;

                case GraphyManager.ModuleState.OFF:
                    gameObject.SetActive( false );
                    break;
            }
        }

        #endregion

        #region Methods -> Private

        private void Init()
        {
            InitBase();

            m_ramGraph = GetComponent<G_RamGraph>();
            m_ramText = GetComponent<G_RamText>();
        }

        private void SetGraphActive( bool active )
        {
            m_ramGraph.enabled = active;
            m_ramGraphGameObject.SetActive( active );
        }

        #endregion
    }
}

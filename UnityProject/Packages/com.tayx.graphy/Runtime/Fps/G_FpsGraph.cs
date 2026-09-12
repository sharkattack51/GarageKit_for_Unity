/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            15-Dec-17
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using Tayx.Graphy.Graph;
using UnityEngine;
using UnityEngine.UI;

namespace Tayx.Graphy.Fps
{
    public class G_FpsGraph : G_Graph
    {
        #region Variables -> Serialized Private

        [SerializeField] private Image m_imageGraph = null;

        [SerializeField] private Shader ShaderFull = null;
        [SerializeField] private Shader ShaderLight = null;

        // This keeps track of whether Init() has run or not
        private bool m_isInitialized = false;

        #endregion

        #region Variables -> Private

        private GraphyManager m_graphyManager = null;

        private G_FpsMonitor m_fpsMonitor = null;

        private int m_resolution = 150;
        private GraphyManager.Mode m_graphyMode = GraphyManager.Mode.FULL;

        private G_GraphShader m_shaderGraph = null;

        private int[] m_fpsArray;
        private int m_writeIndex = 0;

        private int m_highestFps;

        #endregion

        #region Methods -> Unity Callbacks

        private void Update()
        {
            UpdateGraph();
        }

        #endregion

        #region Methods -> Public

        public void UpdateParameters()
        {
            if( m_shaderGraph == null )
            {
                // While Graphy is disabled (e.g. by default via Ctrl+H) and while in Editor after a Hot-Swap,
                // the OnApplicationFocus calls this while m_shaderGraph == null, throwing a NullReferenceException
                return;
            }

            bool materialChanged = UpdateMaterial();
            int resolution = m_graphyManager.FpsGraphResolution;

            if( m_shaderGraph.ShaderArrayValues == null || m_resolution != resolution )
            {
                m_resolution = resolution;
                CreatePoints();
            }
            else if( materialChanged )
            {
                m_shaderGraph.UpdateArrayValuesLength();
                m_shaderGraph.UpdatePoints();
            }

            UpdateColors();
        }

        public void UpdateColors()
        {
            if( m_shaderGraph == null )
            {
                return;
            }

            m_shaderGraph.GoodColor = m_graphyManager.GoodFPSColor;
            m_shaderGraph.CautionColor = m_graphyManager.CautionFPSColor;
            m_shaderGraph.CriticalColor = m_graphyManager.CriticalFPSColor;

            m_shaderGraph.UpdateColors();
        }

        #endregion

        #region Methods -> Protected Override

        protected override void UpdateGraph()
        {
            // Since we no longer initialize by default OnEnable(), 
            // we need to check here, and Init() if needed
            if( !m_isInitialized )
            {
                Init();
            }

            short fps = m_fpsMonitor.CurrentFPS;

            m_fpsArray[ m_writeIndex ] = fps;
            m_writeIndex = ( m_writeIndex + 1 ) % m_resolution;

            int currentMaxFps = 0;

            for( int i = 0; i < m_resolution; i++ )
            {
                if( currentMaxFps < m_fpsArray[ i ] )
                {
                    currentMaxFps = m_fpsArray[ i ];
                }
            }

            m_highestFps = m_highestFps < 1 || m_highestFps <= currentMaxFps ? currentMaxFps : m_highestFps - 1;

            m_highestFps = m_highestFps > 0 ? m_highestFps : 1;
            m_highestFps = Mathf.Max( m_highestFps, m_fpsMonitor.AverageFPS );

            if( m_shaderGraph.ShaderArrayValues == null )
            {
                m_fpsArray = new int[m_resolution];
                m_shaderGraph.ShaderArrayValues = new float[m_resolution];
            }

            for( int i = 0; i < m_resolution; i++ )
            {
                m_shaderGraph.ShaderArrayValues[ i ] = m_fpsArray[ ( m_writeIndex + i ) % m_resolution ] / (float) m_highestFps;
            }

            // Update the material values

            m_shaderGraph.UpdatePoints();

            m_shaderGraph.Average = (float) m_fpsMonitor.AverageFPS / m_highestFps;
            m_shaderGraph.UpdateAverage();

            m_shaderGraph.GoodThreshold = (float) m_graphyManager.GoodFPSThreshold / m_highestFps;
            m_shaderGraph.CautionThreshold = (float) m_graphyManager.CautionFPSThreshold / m_highestFps;
            m_shaderGraph.UpdateThresholds();
        }

        protected override void CreatePoints()
        {
            if( m_shaderGraph.ShaderArrayValues == null || m_fpsArray == null || m_fpsArray.Length != m_resolution )
            {
                m_fpsArray = new int[m_resolution];
                m_shaderGraph.ShaderArrayValues = new float[m_resolution];
                m_writeIndex = 0;
            }

            for( int i = 0; i < m_resolution; i++ )
            {
                m_shaderGraph.ShaderArrayValues[ i ] = 0;
            }

            m_shaderGraph.UpdateArrayValuesLength();
        }

        #endregion

        #region Methods -> Private

        private bool UpdateMaterial()
        {
            GraphyManager.Mode graphyMode = m_graphyManager.GraphyMode;

            if( m_isInitialized && m_graphyMode == graphyMode && m_shaderGraph.Image.material != null )
            {
                return false;
            }

            if( m_isInitialized && m_shaderGraph.Image.material != null )
            {
                Destroy( m_shaderGraph.Image.material );
            }

            switch( graphyMode )
            {
                case GraphyManager.Mode.FULL:
                    m_shaderGraph.ArrayMaxSize = G_GraphShader.ArrayMaxSizeFull;
                    m_shaderGraph.Image.material = new Material( ShaderFull );
                    break;

                case GraphyManager.Mode.LIGHT:
                    m_shaderGraph.ArrayMaxSize = G_GraphShader.ArrayMaxSizeLight;
                    m_shaderGraph.Image.material = new Material( ShaderLight );
                    break;
            }

            m_shaderGraph.InitializeShader();
            m_graphyMode = graphyMode;

            return true;
        }

        private void Init()
        {
            m_graphyManager = transform.root.GetComponentInChildren<GraphyManager>();

            m_fpsMonitor = GetComponent<G_FpsMonitor>();

            m_shaderGraph = new G_GraphShader
            {
                Image = m_imageGraph
            };

            UpdateParameters();

            m_isInitialized = true;
        }

        #endregion
    }
}
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

namespace Tayx.Graphy.Audio
{
    public class G_AudioGraph : G_Graph
    {
        #region Variables -> Serialized Private

        [SerializeField] private Image m_imageGraph = null;
        [SerializeField] private Image m_imageGraphHighestValues = null;

        [SerializeField] private Shader ShaderFull = null;
        [SerializeField] private Shader ShaderLight = null;

        private bool m_isInitialized = false;

        #endregion

        #region Variables -> Private

        private GraphyManager m_graphyManager = null;

        private G_AudioMonitor m_audioMonitor = null;

        private int m_resolution = 40;
        private GraphyManager.Mode m_graphyMode = GraphyManager.Mode.FULL;

        private G_GraphShader m_shaderGraph = null;
        private G_GraphShader m_shaderGraphHighestValues = null;

        private float[] m_graphArray;
        private float[] m_graphArrayHighestValue;

        #endregion

        #region Methods -> Unity Callbacks

        private void OnEnable()
        {
            /* ----- NOTE: ----------------------------
             * We used to Init() here regardless of
             * whether this module was enabled.
             * The reason we don't Init() here
             * anymore is that some users are on 
             * platforms that do not support the arrays 
             * in the Shaders.
             *
             * See: https://github.com/Tayx94/graphy/issues/17
             * 
             * Even though we don't Init() competely
             * here anymore, we still need 
             * m_audioMonitor for in Update()
             * --------------------------------------*/
            m_audioMonitor = GetComponent<G_AudioMonitor>();
        }

        private void Update()
        {
            if( m_audioMonitor.SpectrumDataAvailable )
            {
                UpdateGraph();
            }
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

            bool materialChanged = UpdateMaterials();
            int resolution = m_graphyManager.AudioGraphResolution;

            if( m_shaderGraph.ShaderArrayValues == null || m_resolution != resolution )
            {
                m_resolution = resolution;
                CreatePoints();
            }
            else if( materialChanged )
            {
                UpdateShaderParameters();
                m_shaderGraph.UpdatePoints();
                m_shaderGraphHighestValues.UpdatePoints();
            }
            else
            {
                UpdateColors();
            }
        }

        public void UpdateColors()
        {
            if( m_shaderGraph == null || m_shaderGraphHighestValues == null )
            {
                return;
            }

            m_shaderGraph.GoodColor = m_graphyManager.AudioGraphColor;
            m_shaderGraph.CautionColor = m_graphyManager.AudioGraphColor;
            m_shaderGraph.CriticalColor = m_graphyManager.AudioGraphColor;
            m_shaderGraph.UpdateColors();

            m_shaderGraphHighestValues.GoodColor = m_graphyManager.AudioGraphColor;
            m_shaderGraphHighestValues.CautionColor = m_graphyManager.AudioGraphColor;
            m_shaderGraphHighestValues.CriticalColor = m_graphyManager.AudioGraphColor;
            m_shaderGraphHighestValues.UpdateColors();
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

            // Current values -------------------------

            for( int i = 0; i <= m_resolution - 1; i++ )
            {
                float currentValue = GetSpectrumAverage( m_audioMonitor.Spectrum, i );

                // Uses 3 values for each bar to accomplish that look

                if( (i + 1) % 3 == 0 && i > 1 )
                {
                    float value =
                    (
                        G_AudioMonitor.dBNormalized( G_AudioMonitor.lin2dB( currentValue ) )
                        + m_graphArray[ i - 1 ]
                        + m_graphArray[ i - 2 ]
                    ) / 3;

                    m_graphArray[ i ] = value;
                    m_graphArray[ i - 1 ] = value;
                    m_graphArray[ i - 2 ] =
                        -1; // Always set the third one to -1 to leave gaps in the graph and improve readability
                }
                else
                {
                    m_graphArray[ i ] =
                        G_AudioMonitor.dBNormalized( G_AudioMonitor.lin2dB( currentValue ) );
                }
            }

            for( int i = 0; i <= m_resolution - 1; i++ )
            {
                m_shaderGraph.ShaderArrayValues[ i ] = m_graphArray[ i ];
            }

            m_shaderGraph.UpdatePoints();


            // Highest values -------------------------

            for( int i = 0; i <= m_resolution - 1; i++ )
            {
                float currentValue = GetSpectrumAverage( m_audioMonitor.SpectrumHighestValues, i );

                // Uses 3 values for each bar to accomplish that look

                if( (i + 1) % 3 == 0 && i > 1 )
                {
                    float value =
                    (
                        G_AudioMonitor.dBNormalized( G_AudioMonitor.lin2dB( currentValue ) )
                        + m_graphArrayHighestValue[ i - 1 ]
                        + m_graphArrayHighestValue[ i - 2 ]
                    ) / 3;

                    m_graphArrayHighestValue[ i ] = value;
                    m_graphArrayHighestValue[ i - 1 ] = value;
                    m_graphArrayHighestValue[ i - 2 ] =
                        -1; // Always set the third one to -1 to leave gaps in the graph and improve readability
                }
                else
                {
                    m_graphArrayHighestValue[ i ] =
                        G_AudioMonitor.dBNormalized( G_AudioMonitor.lin2dB( currentValue ) );
                }
            }

            for( int i = 0; i <= m_resolution - 1; i++ )
            {
                m_shaderGraphHighestValues.ShaderArrayValues[ i ] = m_graphArrayHighestValue[ i ];
            }

            m_shaderGraphHighestValues.UpdatePoints();
        }

        protected override void CreatePoints()
        {
            // Init Arrays
            if( m_shaderGraph.ShaderArrayValues == null
                || m_graphArray == null
                || m_shaderGraph.ShaderArrayValues.Length != m_resolution )
            {
                m_graphArray = new float[m_resolution];
                m_graphArrayHighestValue = new float[m_resolution];
                m_shaderGraph.ShaderArrayValues = new float[m_resolution];
                m_shaderGraphHighestValues.ShaderArrayValues = new float[m_resolution];
            }

            for( int i = 0; i < m_resolution; i++ )
            {
                m_shaderGraph.ShaderArrayValues[ i ] = 0;
                m_shaderGraphHighestValues.ShaderArrayValues[ i ] = 0;
            }

            UpdateShaderParameters();
        }

        #endregion

        #region Methods -> Private

        private float GetSpectrumAverage( float[] spectrum, int graphIndex )
        {
            if( spectrum == null || spectrum.Length == 0 )
            {
                return 0;
            }

            int startIndex = graphIndex * spectrum.Length / m_resolution;
            int endIndex = (graphIndex + 1) * spectrum.Length / m_resolution;

            if( endIndex <= startIndex )
            {
                return 0;
            }

            float total = 0;

            for( int i = startIndex; i < endIndex; i++ )
            {
                total += spectrum[ i ];
            }

            return total / (endIndex - startIndex);
        }

        private bool UpdateMaterials()
        {
            GraphyManager.Mode graphyMode = m_graphyManager.GraphyMode;

            if( m_isInitialized
                && m_graphyMode == graphyMode
                && m_shaderGraph.Image.material != null
                && m_shaderGraphHighestValues.Image.material != null )
            {
                return false;
            }

            if( m_isInitialized && m_shaderGraph.Image.material != null )
            {
                Destroy( m_shaderGraph.Image.material );
            }

            if( m_isInitialized && m_shaderGraphHighestValues.Image.material != null )
            {
                Destroy( m_shaderGraphHighestValues.Image.material );
            }

            switch( graphyMode )
            {
                case GraphyManager.Mode.FULL:
                    m_shaderGraph.ArrayMaxSize = G_GraphShader.ArrayMaxSizeFull;
                    m_shaderGraph.Image.material = new Material( ShaderFull );

                    m_shaderGraphHighestValues.ArrayMaxSize = G_GraphShader.ArrayMaxSizeFull;
                    m_shaderGraphHighestValues.Image.material = new Material( ShaderFull );
                    break;

                case GraphyManager.Mode.LIGHT:
                    m_shaderGraph.ArrayMaxSize = G_GraphShader.ArrayMaxSizeLight;
                    m_shaderGraph.Image.material = new Material( ShaderLight );

                    m_shaderGraphHighestValues.ArrayMaxSize = G_GraphShader.ArrayMaxSizeLight;
                    m_shaderGraphHighestValues.Image.material = new Material( ShaderLight );
                    break;
            }

            m_shaderGraph.InitializeShader();
            m_shaderGraphHighestValues.InitializeShader();

            m_graphyMode = graphyMode;

            return true;
        }

        private void UpdateShaderParameters()
        {
            UpdateColors();

            // Threshold
            m_shaderGraph.GoodThreshold = 0;
            m_shaderGraph.CautionThreshold = 0;
            m_shaderGraph.UpdateThresholds();

            m_shaderGraphHighestValues.GoodThreshold = 0;
            m_shaderGraphHighestValues.CautionThreshold = 0;
            m_shaderGraphHighestValues.UpdateThresholds();

            // Update Array
            m_shaderGraph.UpdateArrayValuesLength();
            m_shaderGraphHighestValues.UpdateArrayValuesLength();

            // Average
            m_shaderGraph.Average = 0;
            m_shaderGraph.UpdateAverage();

            m_shaderGraphHighestValues.Average = 0;
            m_shaderGraphHighestValues.UpdateAverage();
        }

        private void Init()
        {
            m_graphyManager = transform.root.GetComponentInChildren<GraphyManager>();

            m_audioMonitor = GetComponent<G_AudioMonitor>();

            m_shaderGraph = new G_GraphShader
            {
                Image = m_imageGraph
            };

            m_shaderGraphHighestValues = new G_GraphShader
            {
                Image = m_imageGraphHighestValues
            };

            UpdateParameters();

            m_isInitialized = true;
        }

        #endregion
    }
}
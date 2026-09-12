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

using UnityEngine;

namespace Tayx.Graphy.Fps
{
    public class G_FpsMonitor : MonoBehaviour
    {
        #region Variables -> Private

        private const int m_fpsSamplesCapacity = 1024;

        private float[] m_frameTimeSamples;
        private float[] m_slowestFrameTimeSamples;

        private int m_fpsSamplesCount = 0;
        private int m_indexSample = 0;

        private double m_runningFrameTime = 0;

        #endregion

        #region Properties -> Public

        public short CurrentFPS { get; private set; } = 0;
        public short AverageFPS { get; private set; } = 0;
        public short OnePercentFPS { get; private set; } = 0;
        public short Zero1PercentFps { get; private set; } = 0;

        #endregion

        #region Methods -> Unity Callbacks

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            float unscaledDeltaTime = Time.unscaledDeltaTime;

            if( unscaledDeltaTime <= 0 || float.IsNaN( unscaledDeltaTime ) || float.IsInfinity( unscaledDeltaTime ) )
            {
                return;
            }

            CurrentFPS = ToFps( 1d / unscaledDeltaTime );

            m_runningFrameTime -= m_frameTimeSamples[ m_indexSample ];
            m_frameTimeSamples[ m_indexSample ] = unscaledDeltaTime;
            m_runningFrameTime += unscaledDeltaTime;

            m_indexSample = (m_indexSample + 1) % m_fpsSamplesCapacity;

            if( m_fpsSamplesCount < m_fpsSamplesCapacity )
            {
                m_fpsSamplesCount++;
            }

            AverageFPS = ToFps( m_fpsSamplesCount / m_runningFrameTime );

            int onePercentSamples = Mathf.Max( 1, Mathf.RoundToInt( m_fpsSamplesCount * 0.01f ) );
            int zero1PercentSamples = Mathf.Max( 1, Mathf.RoundToInt( m_fpsSamplesCount * 0.001f ) );

            for( int i = 0; i < onePercentSamples; i++ )
            {
                m_slowestFrameTimeSamples[ i ] = 0;
            }

            for( int i = 0; i < m_fpsSamplesCount; i++ )
            {
                float sample = m_frameTimeSamples[ i ];

                if( sample > m_slowestFrameTimeSamples[ onePercentSamples - 1 ] )
                {
                    m_slowestFrameTimeSamples[ onePercentSamples - 1 ] = sample;

                    for( int j = onePercentSamples - 1;
                         j > 0 && m_slowestFrameTimeSamples[ j ] > m_slowestFrameTimeSamples[ j - 1 ];
                         j-- )
                    {
                        float temp = m_slowestFrameTimeSamples[ j ];
                        m_slowestFrameTimeSamples[ j ] = m_slowestFrameTimeSamples[ j - 1 ];
                        m_slowestFrameTimeSamples[ j - 1 ] = temp;
                    }
                }
            }

            double totalFrameTime = 0;

            for( int i = 0; i < onePercentSamples; i++ )
            {
                totalFrameTime += m_slowestFrameTimeSamples[ i ];

                if( i == zero1PercentSamples - 1 )
                {
                    Zero1PercentFps = ToFps( zero1PercentSamples / totalFrameTime );
                }
            }

            OnePercentFPS = ToFps( onePercentSamples / totalFrameTime );
        }

        #endregion

        #region Methods -> Public

        /// <summary>
        /// Retained for API compatibility. FPS sample parameters now update automatically.
        /// </summary>
        public void UpdateParameters()
        {
        }

        #endregion

        #region Methods -> Private

        private void Init()
        {
            m_frameTimeSamples = new float[m_fpsSamplesCapacity];

            int maxOnePercentSamples = Mathf.Max( 1, Mathf.RoundToInt( m_fpsSamplesCapacity * 0.01f ) );
            m_slowestFrameTimeSamples = new float[maxOnePercentSamples];
        }

        private short ToFps( double fps )
        {
            if( double.IsNaN( fps ) || double.IsInfinity( fps ) || fps <= 0 )
            {
                return 0;
            }

            return fps >= short.MaxValue
                ? short.MaxValue
                : (short) Mathf.RoundToInt( (float) fps );
        }

        #endregion
    }
}
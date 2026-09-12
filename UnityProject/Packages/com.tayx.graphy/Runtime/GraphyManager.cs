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

using System;
using UnityEngine;
using Tayx.Graphy.Audio;
using Tayx.Graphy.Fps;
using Tayx.Graphy.Ram;
using Tayx.Graphy.Utils;
using Tayx.Graphy.Advanced;
using Tayx.Graphy.Utils.NumString;

#if GRAPHY_NEW_INPUT
using UnityEngine.InputSystem;
#endif

namespace Tayx.Graphy
{
    /// <summary>
    /// Main class to access the Graphy API.
    /// </summary>
    public class GraphyManager : G_Singleton<GraphyManager>
    {
        protected GraphyManager()
        {
        }

        #region Enums -> Public

        public enum Mode
        {
            FULL = 0,
            LIGHT = 1
        }

        public enum ModuleType
        {
            FPS = 0,
            RAM = 1,
            AUDIO = 2,
            ADVANCED = 3
        }

        public enum ModuleState
        {
            FULL = 0,
            TEXT = 1,
            BASIC = 2,
            BACKGROUND = 3,
            OFF = 4
        }

        public enum ModulePosition
        {
            TOP_RIGHT = 0,
            TOP_LEFT = 1,
            BOTTOM_RIGHT = 2,
            BOTTOM_LEFT = 3,
            FREE = 4
        }

        public enum LookForAudioListener
        {
            ALWAYS,
            ON_SCENE_LOAD,
            NEVER
        }

        public enum ModulePreset
        {
            FPS_BASIC = 0,
            FPS_TEXT = 1,
            FPS_FULL = 2,

            FPS_TEXT_RAM_TEXT = 3,
            FPS_FULL_RAM_TEXT = 4,
            FPS_FULL_RAM_FULL = 5,

            FPS_TEXT_RAM_TEXT_AUDIO_TEXT = 6,
            FPS_FULL_RAM_TEXT_AUDIO_TEXT = 7,
            FPS_FULL_RAM_FULL_AUDIO_TEXT = 8,
            FPS_FULL_RAM_FULL_AUDIO_FULL = 9,

            FPS_FULL_RAM_FULL_AUDIO_FULL_ADVANCED_FULL = 10,
            FPS_BASIC_ADVANCED_FULL = 11
        }

        #endregion

        #region Variables -> Serialized Private

        [SerializeField] private Mode m_graphyMode = Mode.FULL;

        [SerializeField] private bool m_enableOnStartup = true;

        [SerializeField] private bool m_keepAlive = true;

        [SerializeField] private bool m_background = true;
        [SerializeField] private Color m_backgroundColor = new Color( 0, 0, 0, 0.3f );

        [Range( 0.5f, 2f )] [SerializeField] private float m_uiScale = 1f;

        [SerializeField] private bool m_enableHotkeys = true;

#pragma warning disable 0414 // Both backend-specific values must remain serialized while either backend is inactive.
        [SerializeField] private KeyCode m_toggleModeKeyCode = KeyCode.G;
#if GRAPHY_NEW_INPUT
        [SerializeField] private Key m_toggleModeInputSystemKey = Key.G;
#endif
#pragma warning restore 0414
        [SerializeField] private bool m_toggleModeCtrl = true;
        [SerializeField] private bool m_toggleModeAlt = false;

#pragma warning disable 0414 // Both backend-specific values must remain serialized while either backend is inactive.
        [SerializeField] private KeyCode m_toggleActiveKeyCode = KeyCode.H;
#if GRAPHY_NEW_INPUT
        [SerializeField] private Key m_toggleActiveInputSystemKey = Key.H;
#endif
#pragma warning restore 0414
        [SerializeField] private bool m_toggleActiveCtrl = true;
        [SerializeField] private bool m_toggleActiveAlt = false;

        [SerializeField] private ModulePosition m_graphModulePosition = ModulePosition.TOP_RIGHT;
        [SerializeField] private Vector2 m_graphModuleOffset = new Vector2( 0, 0 );

        // Fps ---------------------------------------------------------------------------

        [SerializeField] private ModuleState m_fpsModuleState = ModuleState.FULL;

        [SerializeField] private Color m_goodFpsColor = new Color32( 118, 212, 58, 255 );
        [SerializeField] private int m_goodFpsThreshold = 60;

        [SerializeField] private Color m_cautionFpsColor = new Color32( 243, 232, 0, 255 );
        [SerializeField] private int m_cautionFpsThreshold = 30;

        [SerializeField] private Color m_criticalFpsColor = new Color32( 220, 41, 30, 255 );

        [Range( 10, 300 )] [SerializeField] private int m_fpsGraphResolution = 150;

        [Range( 1, 200 )] [SerializeField] private int m_fpsTextUpdateRate = 3; // 3 updates per sec.

        // Ram ---------------------------------------------------------------------------

        [SerializeField] private ModuleState m_ramModuleState = ModuleState.FULL;

        [SerializeField] private Color m_allocatedRamColor = new Color32( 255, 190, 60, 255 );
        [SerializeField] private Color m_reservedRamColor = new Color32( 205, 84, 229, 255 );
        [SerializeField] private Color m_monoRamColor = new Color( 0.3f, 0.65f, 1f, 1 );

        [Range( 10, 300 )] [SerializeField] private int m_ramGraphResolution = 150;


        [Range( 1, 200 )] [SerializeField] private int m_ramTextUpdateRate = 3; // 3 updates per sec.

        // Audio -------------------------------------------------------------------------

        [SerializeField] private ModuleState m_audioModuleState = ModuleState.FULL;

        [SerializeField]
        private LookForAudioListener m_findAudioListenerInCameraIfNull = LookForAudioListener.ON_SCENE_LOAD;

        [SerializeField] private AudioListener m_audioListener = null;

        [SerializeField] private Color m_audioGraphColor = Color.white;

        [Range( 10, 300 )] [SerializeField] private int m_audioGraphResolution = 81;

        [Range( 1, 200 )] [SerializeField] private int m_audioTextUpdateRate = 3; // 3 updates per sec.

        [SerializeField] private FFTWindow m_FFTWindow = FFTWindow.Blackman;

        [Tooltip( "Must be a power of 2 and between 64-8192" )] [SerializeField]
        private int m_spectrumSize = 512;

        // Advanced ----------------------------------------------------------------------

        [SerializeField] private ModulePosition m_advancedModulePosition = ModulePosition.BOTTOM_LEFT;

        [SerializeField] private Vector2 m_advancedModuleOffset = new Vector2( 0, 0 );

        [SerializeField] private ModuleState m_advancedModuleState = ModuleState.FULL;

        #endregion

        #region Variables -> Private

        private const int m_minGraphResolution = 10;
        private const int m_maxGraphResolution = 300;
        private const int m_minAudioGraphResolution = 12;
        private const int m_minSpectrumSize = 64;
        private const int m_maxSpectrumSize = 8192;

        private bool m_initialized = false;
        private bool m_active = true;
        private bool m_activeStateSet = false;
        private bool m_focused = true;

        private Canvas m_canvas = null;

        private G_FpsManager m_fpsManager = null;
        private G_RamManager m_ramManager = null;
        private G_AudioManager m_audioManager = null;
        private G_AdvancedData m_advancedData = null;

        private G_FpsMonitor m_fpsMonitor = null;
        private G_RamMonitor m_ramMonitor = null;
        private G_AudioMonitor m_audioMonitor = null;

        private ModulePreset m_modulePresetState = ModulePreset.FPS_BASIC_ADVANCED_FULL;

        private static readonly int m_modulePresetCount = Enum.GetNames( typeof( ModulePreset ) ).Length;

        #endregion

        #region Properties -> Public

        public Mode GraphyMode
        {
            get => NormalizeGraphyMode( m_graphyMode );
            set
            {
                Mode graphyMode = NormalizeGraphyMode( value );

                if( m_graphyMode == graphyMode )
                {
                    return;
                }

                m_graphyMode = graphyMode;
                NormalizeGraphSettings();

                if( m_initialized )
                {
                    m_fpsManager.UpdateGraphParameters();
                    m_ramManager.UpdateGraphParameters();
                    m_audioManager.UpdateGraphParameters();
                }
            }
        }

        public bool EnableOnStartup => m_enableOnStartup;

        public bool KeepAlive => m_keepAlive;

        public bool Background
        {
            get => m_background;
            set
            {
                if( m_background == value )
                {
                    return;
                }

                m_background = value;

                if( m_initialized )
                {
                    UpdateAllBackgrounds();
                }
            }
        }

        public Color BackgroundColor
        {
            get => m_backgroundColor;
            set
            {
                if( m_backgroundColor == value )
                {
                    return;
                }

                m_backgroundColor = value;

                if( m_initialized )
                {
                    UpdateAllBackgrounds();
                }
            }
        }

        public float UIScale
        {
            get => m_uiScale;
            set
            {
                if( float.IsNaN( value ) || float.IsInfinity( value ) )
                {
                    return;
                }

                float uiScale = Mathf.Clamp( value, 0.5f, 2f );

                if( Mathf.Approximately( m_uiScale, uiScale ) )
                {
                    return;
                }

                m_uiScale = uiScale;

                if( m_initialized )
                {
                    UpdateUIScale();
                }
            }
        }

        public ModulePosition GraphModulePosition
        {
            get => m_graphModulePosition;
            set
            {
                if( m_graphModulePosition == value )
                {
                    return;
                }

                m_graphModulePosition = value;

                if( m_initialized )
                {
                    m_fpsManager.SetPosition( m_graphModulePosition, m_graphModuleOffset );
                    m_ramManager.SetPosition( m_graphModulePosition, m_graphModuleOffset );
                    m_audioManager.SetPosition( m_graphModulePosition, m_graphModuleOffset );
                }
            }
        }

        // Fps ---------------------------------------------------------------------------

        // Setters & Getters

        public ModuleState FpsModuleState
        {
            get => m_fpsModuleState;
            set => SetModuleState( ModuleType.FPS, value );
        }

        public Color GoodFPSColor
        {
            get => m_goodFpsColor;
            set
            {
                if( m_goodFpsColor == value )
                {
                    return;
                }

                m_goodFpsColor = value;

                if( m_initialized )
                {
                    m_fpsManager.UpdateGraphColors();
                }
            }
        }

        public Color CautionFPSColor
        {
            get => m_cautionFpsColor;
            set
            {
                if( m_cautionFpsColor == value )
                {
                    return;
                }

                m_cautionFpsColor = value;

                if( m_initialized )
                {
                    m_fpsManager.UpdateGraphColors();
                }
            }
        }

        public Color CriticalFPSColor
        {
            get => m_criticalFpsColor;
            set
            {
                if( m_criticalFpsColor == value )
                {
                    return;
                }

                m_criticalFpsColor = value;

                if( m_initialized )
                {
                    m_fpsManager.UpdateGraphColors();
                }
            }
        }

        public int GoodFPSThreshold
        {
            get => m_goodFpsThreshold;
            set
            {
                if( m_goodFpsThreshold == value )
                {
                    return;
                }

                m_goodFpsThreshold = value;
            }
        }

        public int CautionFPSThreshold
        {
            get => m_cautionFpsThreshold;
            set
            {
                if( m_cautionFpsThreshold == value )
                {
                    return;
                }

                m_cautionFpsThreshold = value;
            }
        }

        public int FpsGraphResolution
        {
            get => NormalizeGraphResolution( m_fpsGraphResolution );
            set
            {
                int resolution = NormalizeGraphResolution( value );

                if( m_fpsGraphResolution == resolution )
                {
                    return;
                }

                m_fpsGraphResolution = resolution;

                if( m_initialized )
                {
                    m_fpsManager.UpdateGraphParameters();
                }
            }
        }

        public int FpsTextUpdateRate
        {
            get => m_fpsTextUpdateRate;
            set
            {
                if( m_fpsTextUpdateRate == value )
                {
                    return;
                }

                m_fpsTextUpdateRate = value;

                if( m_initialized )
                {
                    m_fpsManager.UpdateTextParameters();
                }
            }
        }

        // Getters

        public float CurrentFPS => m_fpsMonitor.CurrentFPS;
        public float AverageFPS => m_fpsMonitor.AverageFPS;
        public float OnePercentFPS => m_fpsMonitor.OnePercentFPS;
        public float Zero1PercentFps => m_fpsMonitor.Zero1PercentFps;

        // Ram ---------------------------------------------------------------------------

        // Setters & Getters

        public ModuleState RamModuleState
        {
            get => m_ramModuleState;
            set => SetModuleState( ModuleType.RAM, value );
        }


        public Color AllocatedRamColor
        {
            get => m_allocatedRamColor;
            set
            {
                if( m_allocatedRamColor == value )
                {
                    return;
                }

                m_allocatedRamColor = value;

                if( m_initialized )
                {
                    m_ramManager.UpdateGraphColors();
                    m_ramManager.UpdateTextParameters();
                }
            }
        }

        public Color ReservedRamColor
        {
            get => m_reservedRamColor;
            set
            {
                if( m_reservedRamColor == value )
                {
                    return;
                }

                m_reservedRamColor = value;

                if( m_initialized )
                {
                    m_ramManager.UpdateGraphColors();
                    m_ramManager.UpdateTextParameters();
                }
            }
        }

        public Color MonoRamColor
        {
            get => m_monoRamColor;
            set
            {
                if( m_monoRamColor == value )
                {
                    return;
                }

                m_monoRamColor = value;

                if( m_initialized )
                {
                    m_ramManager.UpdateGraphColors();
                    m_ramManager.UpdateTextParameters();
                }
            }
        }

        public int RamGraphResolution
        {
            get => NormalizeGraphResolution( m_ramGraphResolution );
            set
            {
                int resolution = NormalizeGraphResolution( value );

                if( m_ramGraphResolution == resolution )
                {
                    return;
                }

                m_ramGraphResolution = resolution;

                if( m_initialized )
                {
                    m_ramManager.UpdateGraphParameters();
                }
            }
        }

        public int RamTextUpdateRate
        {
            get => m_ramTextUpdateRate;
            set
            {
                if( m_ramTextUpdateRate == value )
                {
                    return;
                }

                m_ramTextUpdateRate = value;

                if( m_initialized )
                {
                    m_ramManager.UpdateTextParameters();
                }
            }
        }

        // Getters

        public float AllocatedRam => m_ramMonitor.AllocatedRam;
        public float ReservedRam => m_ramMonitor.ReservedRam;
        public float MonoRam => m_ramMonitor.MonoRam;

        // Audio -------------------------------------------------------------------------

        // Setters & Getters

        public ModuleState AudioModuleState
        {
            get => m_audioModuleState;
            set => SetModuleState( ModuleType.AUDIO, value );
        }

        public AudioListener AudioListener
        {
            get => m_audioListener;
            set
            {
                if( m_audioListener == value )
                {
                    return;
                }

                m_audioListener = value;

                if( m_initialized )
                {
                    m_audioManager.UpdateAudioListener();
                }
            }
        }

        public LookForAudioListener FindAudioListenerInCameraIfNull
        {
            get => m_findAudioListenerInCameraIfNull;
            set
            {
                if( m_findAudioListenerInCameraIfNull == value )
                {
                    return;
                }

                m_findAudioListenerInCameraIfNull = value;

                if( m_initialized )
                {
                    m_audioManager.UpdateAudioListener();
                }
            }
        }

        public Color AudioGraphColor
        {
            get => m_audioGraphColor;
            set
            {
                if( m_audioGraphColor == value )
                {
                    return;
                }

                m_audioGraphColor = value;

                if( m_initialized )
                {
                    m_audioManager.UpdateGraphColors();
                }
            }
        }

        public int AudioGraphResolution
        {
            get => NormalizeAudioGraphResolution( m_audioGraphResolution );
            set
            {
                int resolution = NormalizeAudioGraphResolution( value );

                if( m_audioGraphResolution == resolution )
                {
                    return;
                }

                m_audioGraphResolution = resolution;

                if( m_initialized )
                {
                    m_audioManager.UpdateGraphParameters();
                }
            }
        }

        public int AudioTextUpdateRate
        {
            get => m_audioTextUpdateRate;
            set
            {
                if( m_audioTextUpdateRate == value )
                {
                    return;
                }

                m_audioTextUpdateRate = value;

                if( m_initialized )
                {
                    m_audioManager.UpdateTextParameters();
                }
            }
        }

        public FFTWindow FftWindow
        {
            get => m_FFTWindow;
            set
            {
                if( m_FFTWindow == value )
                {
                    return;
                }

                m_FFTWindow = value;

                if( m_initialized )
                {
                    m_audioManager.UpdateFftWindow();
                }
            }
        }

        public int SpectrumSize
        {
            get => NormalizeSpectrumSize( m_spectrumSize );
            set
            {
                int spectrumSize = NormalizeSpectrumSize( value );
                bool spectrumSizeChanged = m_spectrumSize != spectrumSize;

                m_spectrumSize = spectrumSize;

                int audioGraphResolution = NormalizeAudioGraphResolution( m_audioGraphResolution );
                bool audioGraphResolutionChanged = m_audioGraphResolution != audioGraphResolution;

                m_audioGraphResolution = audioGraphResolution;

                if( m_initialized )
                {
                    if( spectrumSizeChanged )
                    {
                        m_audioManager.UpdateSpectrumSize();
                    }

                    if( audioGraphResolutionChanged )
                    {
                        m_audioManager.UpdateGraphParameters();
                    }
                }
            }
        }

        // Getters

        /// <summary>
        /// Current audio spectrum from the specified AudioListener.
        /// </summary>
        public float[] Spectrum => m_audioMonitor.Spectrum;

        /// <summary>
        /// Maximum DB registered in the current spectrum.
        /// </summary>
        public float MaxDB => m_audioMonitor.MaxDB;


        // Advanced ---------------------------------------------------------------------

        // Setters & Getters

        public ModuleState AdvancedModuleState
        {
            get => m_advancedModuleState;
            set => SetModuleState( ModuleType.ADVANCED, value );
        }

        public ModulePosition AdvancedModulePosition
        {
            get => m_advancedModulePosition;
            set
            {
                if( m_advancedModulePosition == value )
                {
                    return;
                }

                m_advancedModulePosition = value;

                if( m_initialized )
                {
                    m_advancedData.SetPosition( m_advancedModulePosition, m_advancedModuleOffset );
                }
            }
        }

        #endregion

        #region Methods -> Unity Callbacks

        private void Start()
        {
            Init();
        }

        protected override void OnDestroy()
        {
            G_IntString.Dispose();
            G_FloatString.Dispose();

            base.OnDestroy();
        }

        private void Update()
        {
            if( m_focused && m_enableHotkeys )
            {
                CheckForHotkeyPresses();
            }
        }

        private void OnApplicationFocus( bool isFocused )
        {
            m_focused = isFocused;

            if( m_initialized && isFocused )
            {
                RefreshAllParameters();
            }
        }

        #endregion

        #region Methods -> Public

        public void SetModulePosition( ModuleType moduleType, ModulePosition modulePosition )
        {
            switch( moduleType )
            {
                case ModuleType.FPS:
                case ModuleType.RAM:
                case ModuleType.AUDIO:
                    GraphModulePosition = modulePosition;
                    break;

                case ModuleType.ADVANCED:
                    AdvancedModulePosition = modulePosition;
                    break;
            }
        }

        public void SetModuleMode( ModuleType moduleType, ModuleState moduleState )
        {
            SetModuleState( moduleType, moduleState );
        }

        public void ToggleModes()
        {
            if( !m_initialized )
            {
                UpdateModulePresetState();
            }

            if( (int) m_modulePresetState >= m_modulePresetCount - 1 )
            {
                m_modulePresetState = 0;
            }
            else
            {
                m_modulePresetState++;
            }

            SetPreset( m_modulePresetState );
        }

        public void SetPreset( ModulePreset modulePreset )
        {
            m_modulePresetState = modulePreset;

            switch( m_modulePresetState )
            {
                case ModulePreset.FPS_BASIC:
                    SetModuleStates( ModuleState.BASIC, ModuleState.OFF, ModuleState.OFF, ModuleState.OFF );
                    break;

                case ModulePreset.FPS_TEXT:
                    SetModuleStates( ModuleState.TEXT, ModuleState.OFF, ModuleState.OFF, ModuleState.OFF );
                    break;

                case ModulePreset.FPS_FULL:
                    SetModuleStates( ModuleState.FULL, ModuleState.OFF, ModuleState.OFF, ModuleState.OFF );
                    break;

                case ModulePreset.FPS_TEXT_RAM_TEXT:
                    SetModuleStates( ModuleState.TEXT, ModuleState.TEXT, ModuleState.OFF, ModuleState.OFF );
                    break;

                case ModulePreset.FPS_FULL_RAM_TEXT:
                    SetModuleStates( ModuleState.FULL, ModuleState.TEXT, ModuleState.OFF, ModuleState.OFF );
                    break;

                case ModulePreset.FPS_FULL_RAM_FULL:
                    SetModuleStates( ModuleState.FULL, ModuleState.FULL, ModuleState.OFF, ModuleState.OFF );
                    break;

                case ModulePreset.FPS_TEXT_RAM_TEXT_AUDIO_TEXT:
                    SetModuleStates( ModuleState.TEXT, ModuleState.TEXT, ModuleState.TEXT, ModuleState.OFF );
                    break;

                case ModulePreset.FPS_FULL_RAM_TEXT_AUDIO_TEXT:
                    SetModuleStates( ModuleState.FULL, ModuleState.TEXT, ModuleState.TEXT, ModuleState.OFF );
                    break;

                case ModulePreset.FPS_FULL_RAM_FULL_AUDIO_TEXT:
                    SetModuleStates( ModuleState.FULL, ModuleState.FULL, ModuleState.TEXT, ModuleState.OFF );
                    break;

                case ModulePreset.FPS_FULL_RAM_FULL_AUDIO_FULL:
                    SetModuleStates( ModuleState.FULL, ModuleState.FULL, ModuleState.FULL, ModuleState.OFF );
                    break;

                case ModulePreset.FPS_FULL_RAM_FULL_AUDIO_FULL_ADVANCED_FULL:
                    SetModuleStates( ModuleState.FULL, ModuleState.FULL, ModuleState.FULL, ModuleState.FULL );
                    break;

                case ModulePreset.FPS_BASIC_ADVANCED_FULL:
                    SetModuleStates( ModuleState.BASIC, ModuleState.OFF, ModuleState.OFF, ModuleState.FULL );
                    break;

                default:
                    Debug.LogWarning( "[GraphyManager]::SetPreset - Tried to set a preset that is not supported." );
                    break;
            }
        }

        public void ToggleActive()
        {
            if( !m_initialized && !m_activeStateSet )
            {
                m_active = m_enableOnStartup;
            }

            if( !m_active )
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        public void Enable()
        {
            if( !m_initialized )
            {
                m_active = true;
                m_activeStateSet = true;
                return;
            }

            if( !m_active )
            {
                m_active = true;
                ApplyModuleStates( true );
            }
        }

        public void Disable()
        {
            if( !m_initialized )
            {
                m_active = false;
                m_activeStateSet = true;
                return;
            }

            if( m_active )
            {
                m_active = false;
                ApplyDisabledState();
            }
        }

        #endregion

        #region Methods -> Private

        private void Init()
        {
            if( m_initialized )
            {
                return;
            }

            NormalizeGraphSettings();

            if( m_keepAlive )
            {
                DontDestroyOnLoad( transform.root.gameObject );
            }

            m_canvas = GetComponent<Canvas>();

            m_fpsMonitor = GetComponentInChildren<G_FpsMonitor>( true );
            m_ramMonitor = GetComponentInChildren<G_RamMonitor>( true );
            m_audioMonitor = GetComponentInChildren<G_AudioMonitor>( true );

            m_fpsManager = GetComponentInChildren<G_FpsManager>( true );
            m_ramManager = GetComponentInChildren<G_RamManager>( true );
            m_audioManager = GetComponentInChildren<G_AudioManager>( true );
            m_advancedData = GetComponentInChildren<G_AdvancedData>( true );

            m_fpsManager.SetPosition( m_graphModulePosition, m_graphModuleOffset );
            m_ramManager.SetPosition( m_graphModulePosition, m_graphModuleOffset );
            m_audioManager.SetPosition( m_graphModulePosition, m_graphModuleOffset );
            m_advancedData.SetPosition( m_advancedModulePosition, m_advancedModuleOffset );

            m_initialized = true;
            UpdateModulePresetState();

            if( !m_activeStateSet )
            {
                m_active = m_enableOnStartup;
            }

            if( !m_enableOnStartup )
            {
                // We need to enable this on startup because we disable it in GraphyManagerEditor
                m_canvas.enabled = true;
            }

            if( m_active )
            {
                ApplyModuleStates( true );
            }
            else
            {
                ApplyDisabledState();
            }

            UpdateAllParameters();
            UpdateUIScale();
        }

        public void OnValidate()
        {
            m_uiScale = float.IsNaN( m_uiScale ) || float.IsInfinity( m_uiScale )
                ? 1f
                : Mathf.Clamp( m_uiScale, 0.5f, 2f );

            NormalizeGraphSettings();

            if( m_initialized )
            {
                m_fpsManager.SetPosition( m_graphModulePosition, m_graphModuleOffset );
                m_ramManager.SetPosition( m_graphModulePosition, m_graphModuleOffset );
                m_audioManager.SetPosition( m_graphModulePosition, m_graphModuleOffset );
                m_advancedData.SetPosition( m_advancedModulePosition, m_advancedModuleOffset );

                UpdateAllParameters();
                UpdateUIScale();
                UpdateModulePresetState();

                if( m_active )
                {
                    ApplyModuleStates( true );
                }
                else
                {
                    ApplyDisabledState();
                }
            }
        }

        private int GetMaxGraphResolution()
        {
            return NormalizeGraphyMode( m_graphyMode ) == Mode.LIGHT
                ? G_GraphShader.ArrayMaxSizeLight
                : m_maxGraphResolution;
        }

        private int NormalizeGraphResolution( int resolution )
        {
            return Mathf.Clamp( resolution, m_minGraphResolution, GetMaxGraphResolution() );
        }

        private int NormalizeAudioGraphResolution( int resolution )
        {
            int maxResolution = Mathf.Min( GetMaxGraphResolution(), NormalizeSpectrumSize( m_spectrumSize ) );
            maxResolution -= maxResolution % 3;

            int clampedResolution = Mathf.Clamp( resolution, m_minAudioGraphResolution, maxResolution );
            int normalizedResolution = Mathf.RoundToInt( clampedResolution / 3f ) * 3;

            return Mathf.Clamp( normalizedResolution, m_minAudioGraphResolution, maxResolution );
        }

        private static Mode NormalizeGraphyMode( Mode graphyMode )
        {
            return graphyMode == Mode.LIGHT ? Mode.LIGHT : Mode.FULL;
        }

        private static int NormalizeSpectrumSize( int spectrumSize )
        {
            return Mathf.ClosestPowerOfTwo( Mathf.Clamp( spectrumSize, m_minSpectrumSize, m_maxSpectrumSize ) );
        }

        private void NormalizeGraphSettings()
        {
            m_graphyMode = NormalizeGraphyMode( m_graphyMode );
            m_spectrumSize = NormalizeSpectrumSize( m_spectrumSize );
            m_fpsGraphResolution = NormalizeGraphResolution( m_fpsGraphResolution );
            m_ramGraphResolution = NormalizeGraphResolution( m_ramGraphResolution );
            m_audioGraphResolution = NormalizeAudioGraphResolution( m_audioGraphResolution );
        }

        private void SetModuleState( ModuleType moduleType, ModuleState moduleState )
        {
            switch( moduleType )
            {
                case ModuleType.FPS:
                    if( m_fpsModuleState == moduleState )
                    {
                        return;
                    }

                    m_fpsModuleState = moduleState;

                    if( m_initialized && m_active )
                    {
                        m_fpsManager.SetState( moduleState );
                    }
                    break;

                case ModuleType.RAM:
                    if( m_ramModuleState == moduleState )
                    {
                        return;
                    }

                    m_ramModuleState = moduleState;

                    if( m_initialized && m_active )
                    {
                        m_ramManager.SetState( moduleState );
                    }
                    break;

                case ModuleType.AUDIO:
                    if( m_audioModuleState == moduleState )
                    {
                        return;
                    }

                    m_audioModuleState = moduleState;

                    if( m_initialized && m_active )
                    {
                        m_audioManager.SetState( moduleState );
                    }
                    break;

                case ModuleType.ADVANCED:
                    if( m_advancedModuleState == moduleState )
                    {
                        return;
                    }

                    m_advancedModuleState = moduleState;

                    if( m_initialized && m_active )
                    {
                        m_advancedData.SetState( moduleState );
                    }
                    break;
            }

            UpdateModulePresetState();
        }

        private void SetModuleStates
        (
            ModuleState fpsModuleState,
            ModuleState ramModuleState,
            ModuleState audioModuleState,
            ModuleState advancedModuleState
        )
        {
            if( ModuleStatesAre( fpsModuleState, ramModuleState, audioModuleState, advancedModuleState ) )
            {
                return;
            }

            m_fpsModuleState = fpsModuleState;
            m_ramModuleState = ramModuleState;
            m_audioModuleState = audioModuleState;
            m_advancedModuleState = advancedModuleState;
            UpdateModulePresetState();

            if( m_initialized && m_active )
            {
                ApplyModuleStates();
            }
        }

        private void UpdateModulePresetState()
        {
            if( ModuleStatesAre( ModuleState.BASIC, ModuleState.OFF, ModuleState.OFF, ModuleState.OFF ) )
            {
                m_modulePresetState = ModulePreset.FPS_BASIC;
            }
            else if( ModuleStatesAre( ModuleState.TEXT, ModuleState.OFF, ModuleState.OFF, ModuleState.OFF ) )
            {
                m_modulePresetState = ModulePreset.FPS_TEXT;
            }
            else if( ModuleStatesAre( ModuleState.FULL, ModuleState.OFF, ModuleState.OFF, ModuleState.OFF ) )
            {
                m_modulePresetState = ModulePreset.FPS_FULL;
            }
            else if( ModuleStatesAre( ModuleState.TEXT, ModuleState.TEXT, ModuleState.OFF, ModuleState.OFF ) )
            {
                m_modulePresetState = ModulePreset.FPS_TEXT_RAM_TEXT;
            }
            else if( ModuleStatesAre( ModuleState.FULL, ModuleState.TEXT, ModuleState.OFF, ModuleState.OFF ) )
            {
                m_modulePresetState = ModulePreset.FPS_FULL_RAM_TEXT;
            }
            else if( ModuleStatesAre( ModuleState.FULL, ModuleState.FULL, ModuleState.OFF, ModuleState.OFF ) )
            {
                m_modulePresetState = ModulePreset.FPS_FULL_RAM_FULL;
            }
            else if( ModuleStatesAre( ModuleState.TEXT, ModuleState.TEXT, ModuleState.TEXT, ModuleState.OFF ) )
            {
                m_modulePresetState = ModulePreset.FPS_TEXT_RAM_TEXT_AUDIO_TEXT;
            }
            else if( ModuleStatesAre( ModuleState.FULL, ModuleState.TEXT, ModuleState.TEXT, ModuleState.OFF ) )
            {
                m_modulePresetState = ModulePreset.FPS_FULL_RAM_TEXT_AUDIO_TEXT;
            }
            else if( ModuleStatesAre( ModuleState.FULL, ModuleState.FULL, ModuleState.TEXT, ModuleState.OFF ) )
            {
                m_modulePresetState = ModulePreset.FPS_FULL_RAM_FULL_AUDIO_TEXT;
            }
            else if( ModuleStatesAre( ModuleState.FULL, ModuleState.FULL, ModuleState.FULL, ModuleState.OFF ) )
            {
                m_modulePresetState = ModulePreset.FPS_FULL_RAM_FULL_AUDIO_FULL;
            }
            else if( ModuleStatesAre( ModuleState.FULL, ModuleState.FULL, ModuleState.FULL, ModuleState.FULL ) )
            {
                m_modulePresetState = ModulePreset.FPS_FULL_RAM_FULL_AUDIO_FULL_ADVANCED_FULL;
            }
            else if( ModuleStatesAre( ModuleState.BASIC, ModuleState.OFF, ModuleState.OFF, ModuleState.FULL ) )
            {
                m_modulePresetState = ModulePreset.FPS_BASIC_ADVANCED_FULL;
            }
            else
            {
                m_modulePresetState = (ModulePreset) (-1);
            }
        }

        private bool ModuleStatesAre
        (
            ModuleState fpsModuleState,
            ModuleState ramModuleState,
            ModuleState audioModuleState,
            ModuleState advancedModuleState
        )
        {
            return m_fpsModuleState == fpsModuleState
                   && m_ramModuleState == ramModuleState
                   && m_audioModuleState == audioModuleState
                   && m_advancedModuleState == advancedModuleState;
        }

        private void ApplyModuleStates( bool silentUpdate = false )
        {
            m_fpsManager.SetState( m_fpsModuleState, silentUpdate );
            m_ramManager.SetState( m_ramModuleState, silentUpdate );
            m_audioManager.SetState( m_audioModuleState, silentUpdate );
            m_advancedData.SetState( m_advancedModuleState, silentUpdate );
        }

        private void ApplyDisabledState()
        {
            m_fpsManager.SetState( ModuleState.OFF, true );
            m_ramManager.SetState( ModuleState.OFF, true );
            m_audioManager.SetState( ModuleState.OFF, true );
            m_advancedData.SetState( ModuleState.OFF, true );
        }

        private void UpdateAllBackgrounds()
        {
            m_fpsManager.UpdateBackground();
            m_ramManager.UpdateBackground();
            m_audioManager.UpdateBackground();
            m_advancedData.UpdateBackground();
        }

        private void UpdateUIScale()
        {
            m_fpsManager.SetScale( m_uiScale );
            m_ramManager.SetScale( m_uiScale );
            m_audioManager.SetScale( m_uiScale );
            m_advancedData.SetScale( m_uiScale );
        }

        private void CheckForHotkeyPresses()
        {
#if GRAPHY_NEW_INPUT && ENABLE_INPUT_SYSTEM
            // Toggle Mode ---------------------------------------
            if (m_toggleModeInputSystemKey != Key.None)
            {
                if( m_toggleModeCtrl && m_toggleModeAlt )
                {
                    if( CheckFor3KeyPress( m_toggleModeInputSystemKey, Key.LeftCtrl, Key.LeftAlt )
                        || CheckFor3KeyPress( m_toggleModeInputSystemKey, Key.RightCtrl, Key.LeftAlt )
                        || CheckFor3KeyPress( m_toggleModeInputSystemKey, Key.RightCtrl, Key.RightAlt )
                        || CheckFor3KeyPress( m_toggleModeInputSystemKey, Key.LeftCtrl, Key.RightAlt ) )
                    {
                        ToggleModes();
                    }
                }
                else if( m_toggleModeCtrl )
                {
                    if( CheckFor2KeyPress( m_toggleModeInputSystemKey, Key.LeftCtrl )
                        || CheckFor2KeyPress( m_toggleModeInputSystemKey, Key.RightCtrl ) )
                    {
                        ToggleModes();
                    }
                }
                else if( m_toggleModeAlt )
                {
                    if( CheckFor2KeyPress( m_toggleModeInputSystemKey, Key.LeftAlt )
                        || CheckFor2KeyPress( m_toggleModeInputSystemKey, Key.RightAlt ) )
                    {
                        ToggleModes();
                    }
                }
                else
                {
                    if( CheckFor1KeyPress( m_toggleModeInputSystemKey ) )
                    {
                        ToggleModes();
                    }
                }
            }

            // Toggle Active -------------------------------------
            if (m_toggleActiveInputSystemKey != Key.None)
            {
                if( m_toggleActiveCtrl && m_toggleActiveAlt )
                {
                    if( CheckFor3KeyPress( m_toggleActiveInputSystemKey, Key.LeftCtrl, Key.LeftAlt )
                        || CheckFor3KeyPress( m_toggleActiveInputSystemKey, Key.RightCtrl, Key.LeftAlt )
                        || CheckFor3KeyPress( m_toggleActiveInputSystemKey, Key.RightCtrl, Key.RightAlt )
                        || CheckFor3KeyPress( m_toggleActiveInputSystemKey, Key.LeftCtrl, Key.RightAlt ) )
                    {
                        ToggleActive();
                    }
                }

                else if( m_toggleActiveCtrl )
                {
                    if( CheckFor2KeyPress( m_toggleActiveInputSystemKey, Key.LeftCtrl )
                        || CheckFor2KeyPress( m_toggleActiveInputSystemKey, Key.RightCtrl ) )
                    {
                        ToggleActive();
                    }
                }
                else if( m_toggleActiveAlt )
                {
                    if( CheckFor2KeyPress( m_toggleActiveInputSystemKey, Key.LeftAlt )
                        || CheckFor2KeyPress( m_toggleActiveInputSystemKey, Key.RightAlt ) )
                    {
                        ToggleActive();
                    }
                }
                else
                {
                    if( CheckFor1KeyPress( m_toggleActiveInputSystemKey ) )
                    {
                        ToggleActive();
                    }
                }
            }
#else
            // Toggle Mode ---------------------------------------
            if (m_toggleModeKeyCode != KeyCode.None)
            {
                if (m_toggleModeCtrl && m_toggleModeAlt)
                {
                    if (   CheckFor3KeyPress(m_toggleModeKeyCode, KeyCode.LeftControl, KeyCode.LeftAlt)
                        || CheckFor3KeyPress(m_toggleModeKeyCode, KeyCode.RightControl, KeyCode.LeftAlt)
                        || CheckFor3KeyPress(m_toggleModeKeyCode, KeyCode.RightControl, KeyCode.RightAlt)
                        || CheckFor3KeyPress(m_toggleModeKeyCode, KeyCode.LeftControl, KeyCode.RightAlt))
                    {
                        ToggleModes();
                    }
                }
                else if (m_toggleModeCtrl)
                {
                    if (    CheckFor2KeyPress(m_toggleModeKeyCode, KeyCode.LeftControl)
                        ||  CheckFor2KeyPress(m_toggleModeKeyCode, KeyCode.RightControl))
                    {
                        ToggleModes();
                    }
                }
                else if (m_toggleModeAlt)
                {
                    if (    CheckFor2KeyPress(m_toggleModeKeyCode, KeyCode.LeftAlt)
                        ||  CheckFor2KeyPress(m_toggleModeKeyCode, KeyCode.RightAlt))
                    {
                        ToggleModes();
                    }
                }
                else
                {
                    if (CheckFor1KeyPress(m_toggleModeKeyCode))
                    {
                        ToggleModes();
                    }
                }
            }

            // Toggle Active -------------------------------------
            if (m_toggleActiveKeyCode != KeyCode.None)
            {
                if (m_toggleActiveCtrl && m_toggleActiveAlt)
                {
                    if (    CheckFor3KeyPress(m_toggleActiveKeyCode, KeyCode.LeftControl, KeyCode.LeftAlt)
                        ||  CheckFor3KeyPress(m_toggleActiveKeyCode, KeyCode.RightControl, KeyCode.LeftAlt)
                        ||  CheckFor3KeyPress(m_toggleActiveKeyCode, KeyCode.RightControl, KeyCode.RightAlt)
                        ||  CheckFor3KeyPress(m_toggleActiveKeyCode, KeyCode.LeftControl, KeyCode.RightAlt))
                    {
                        ToggleActive();
                    }
                }
                
                else if (m_toggleActiveCtrl)
                {
                    if (    CheckFor2KeyPress(m_toggleActiveKeyCode, KeyCode.LeftControl)
                        ||  CheckFor2KeyPress(m_toggleActiveKeyCode, KeyCode.RightControl))
                    {
                        ToggleActive();
                    }
                }
                else if (m_toggleActiveAlt)
                {
                    if (    CheckFor2KeyPress(m_toggleActiveKeyCode, KeyCode.LeftAlt)
                        ||  CheckFor2KeyPress(m_toggleActiveKeyCode, KeyCode.RightAlt))
                    {
                        ToggleActive();
                    }
                }
                else
                {
                    if (CheckFor1KeyPress(m_toggleActiveKeyCode))
                    {
                        ToggleActive();
                    }
                }
            }
#endif
        }

#if GRAPHY_NEW_INPUT && ENABLE_INPUT_SYSTEM
        private bool CheckFor1KeyPress( Key key )
        {
            Keyboard currentKeyboard = Keyboard.current;

            if( currentKeyboard != null )
            {
                return Keyboard.current[ key ].wasPressedThisFrame;
            }

            return false;
        }

        private bool CheckFor2KeyPress( Key key1, Key key2 )
        {
            Keyboard currentKeyboard = Keyboard.current;

            if( currentKeyboard != null )
            {
                return Keyboard.current[ key1 ].wasPressedThisFrame && Keyboard.current[ key2 ].isPressed
                       || Keyboard.current[ key2 ].wasPressedThisFrame && Keyboard.current[ key1 ].isPressed;
            }

            return false;
        }

        private bool CheckFor3KeyPress( Key key1, Key key2, Key key3 )
        {
            Keyboard currentKeyboard = Keyboard.current;

            if( currentKeyboard != null )
            {
                return Keyboard.current[ key1 ].wasPressedThisFrame && Keyboard.current[ key2 ].isPressed &&
                       Keyboard.current[ key3 ].isPressed
                       || Keyboard.current[ key2 ].wasPressedThisFrame && Keyboard.current[ key1 ].isPressed &&
                       Keyboard.current[ key3 ].isPressed
                       || Keyboard.current[ key3 ].wasPressedThisFrame && Keyboard.current[ key1 ].isPressed &&
                       Keyboard.current[ key2 ].isPressed;
            }

            return false;
        }
#else
        private bool CheckFor1KeyPress(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        private bool CheckFor2KeyPress(KeyCode key1, KeyCode key2)
        {
            return Input.GetKeyDown(key1) && Input.GetKey(key2)
                || Input.GetKeyDown(key2) && Input.GetKey(key1);
        }

        private bool CheckFor3KeyPress(KeyCode key1, KeyCode key2, KeyCode key3)
        {
            return Input.GetKeyDown(key1) && Input.GetKey(key2) && Input.GetKey(key3)
                || Input.GetKeyDown(key2) && Input.GetKey(key1) && Input.GetKey(key3)
                || Input.GetKeyDown(key3) && Input.GetKey(key1) && Input.GetKey(key2);
        }
#endif
        private void UpdateAllParameters()
        {
            m_fpsManager.UpdateParameters();
            m_ramManager.UpdateParameters();
            m_audioManager.UpdateParameters();
            m_advancedData.UpdateParameters();
        }

        private void RefreshAllParameters()
        {
            m_fpsManager.RefreshParameters();
            m_ramManager.RefreshParameters();
            m_audioManager.RefreshParameters();
            m_advancedData.RefreshParameters();
        }

        #endregion
    }
}

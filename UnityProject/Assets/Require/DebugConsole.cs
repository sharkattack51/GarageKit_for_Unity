// V.M12.D11.2010.R1
/************************************************************************
* DebugConsole.cs
* Copyright 2008-2010 By: Jeremy Hollingsworth
* (http://www.ennanzus-interactive.com)
*
* Licensed for commercial, non-commercial, and educational use. 
* 
* THIS PRODUCT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND. THE 
* LICENSOR MAKES NO WARRANTY REGARDING THE PRODUCT, EXPRESS OR IMPLIED. 
* THE LICENSOR EXPRESSLY DISCLAIMS AND THE LICENSEE HEREBY WAIVES ALL 
* WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, ALL 
* IMPLIED WARRANTIES OF MERCHANTABILITY AND ALL IMPLIED WARRANTIES OF 
* FITNESS FOR A PARTICULAR PURPOSE.
* ************************************************************************/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Provides a game-mode, multi-line console with command binding, logging and watch vars.
/// 
/// ==== Installation ====
/// Just drop this script into your project. To use from JavaScript(UnityScript), just make sure
/// you place this script in a folder such as "Plugins" so that it is compiled before your js code.
///
/// See the following Unity docs page for more info on this: 
/// http://unity3d.com/support/documentation/ScriptReference/index.Script_compilation_28Advanced29.html
/// 
/// ==== Usage (Logging) ====
/// 
/// To use, you only need to access the desired static Log functions. So, for example, to log a simple
/// message you would do the following:
/// 
/// \code
/// DebugConsole.Log( "Hello World!");
/// 
/// // Now open it
/// DebugConsole.IsOpen = true;
/// \endcode
/// 
/// Those static methods will automatically ensure that the console has been set up in your scene for you,
/// so no need to worry about attaching this script to anything.
/// 
/// See the comments for the other static functions below for details on their use.
/// 
/// ==== Usage (DebugCommand Binding) ====
/// 
/// To use command binding, you create a function to handle the command, then you register that function
/// along with the string used to invoke it with the console.
/// 
/// So, for example, if you want to have a command called "ShowFPS", you would first create the handler like 
/// this:
/// 
/// \code
/// // JavaScript
/// function ShowFPSCommand(args)
/// {
///     //...
/// }
/// 
/// // C#
/// public void ShowFPSCommand(params string[] args)
/// {
///     //...
/// }
/// \endcode
/// 
/// Then, to register the command with the console to be run when "ShowFPS" is typed, you would do the following:
/// 
/// \code
/// DebugConsole.RegisterCommand( "ShowFPS", ShowFPSCommand);
/// \endcode
/// 
/// That's it! Now when the user types "ShowFPS" in the console and hits enter, your function will be run.
/// 
/// If you wish to capture input entered after the command text, the args array will contain every space-separated
/// block of text the user entered after the command. "SetFOV 90" would pass the string "90" to the SetFOV command.
///  
/// Note: Typing "/?" followed by enter will show the list of currently-registered commands.
/// 
/// ==== Usage (Watch Vars) ===
/// 
/// For the Watch Vars feature, you need to use one of the provided (or your own subclass of WatchVar) to store
/// the value of your variable in your project. You then register that WatchVar with the console for tracking.
/// 
/// Example:
/// \code
/// // JavaScript
/// var myWatchInt = new WatchInt("PowerupCount");
/// 
/// myWatchInt.Value = 23;
/// 
/// DebugConsole.RegisterWatchVar(myWatchInt.Name, myWatchInt);
/// \endcode
/// 
/// As you use that WatchInt to store your value through the project, its live value will be shown in the console.
/// 
/// If you subclass WatchVar, you can create your own WatchVars to represent more types than are currently built-in.
/// </summary>
public class DebugConsole : MonoBehaviour
{
    private const float VERSION = 2.0F;

    /// <summary>
    /// This is the signature for the DebugCommand delegate if you use the command binding.
    /// 
    /// So, if you have a JavaScript function named "SetFOV", that you wanted run when typing a
    /// debug command, it would have to have the following definition:
    /// 
    /// \code
    /// function SetFOV(args)
    /// {
    ///     //...
    /// }
    /// \endcode
    /// </summary>
    /// <param name="args">The text typed in the console after the name of the command.</param>
    public delegate void DebugCommand( params string[] args );
    
    /// <summary>
    /// This Enum holds the message types used to easily control the formatting and display of a message.
    /// </summary>
    public enum MessageTypes { Normal, Error, Warning, Command, System };

    /// <summary>
    /// Display modes for the console.
    /// </summary>
    public enum Mode { Log, CopyLog, WatchVars };
	
	/// <summary>
	/// 出力機能をOn/Offする
	/// </summary>
	public bool _enable = true;

    /// <summary>
    /// How many lines of text this console will display.
    /// </summary>
    public int _maxLinesForDisplay = 500;

    /// <summary>
    /// Default color of the standard display text.
    /// </summary>
    public Color _defaultTextColor = Color.white;
   
    /// <summary>
    /// Used to check (or toggle) the open state of the console.
    /// </summary>
    public static bool IsOpen
    {
        get { return DebugConsole.Instance._isOpen; }
        set { DebugConsole.Instance._isOpen = value; }
    }

    /// <summary>
    /// Static instance of the console. 
    /// 
    /// When you want to access the console without a direct
    /// reference (which you do in mose cases), use DebugConsole.Instance and the required
    /// GameObject initialization will be done for you.
    /// </summary>
    public static DebugConsole Instance
    {
        get
        {
            if ( _instance == null )
            {
                _instance = FindObjectOfType( typeof( DebugConsole ) ) as DebugConsole;
                if ( _instance == null )
                {
                    GameObject console = new GameObject();
                    console.AddComponent<DebugConsole>(  );
                    console.name = "Debug Console";
                    _instance = FindObjectOfType( typeof( DebugConsole ) ) as DebugConsole;
                }
            }

            return _instance;
        }
    }

    private Mode _mode = Mode.Log;
	public Mode mode { get{ return _mode; } set{ _mode = value; } }
    private static DebugConsole _instance = null;
    private Hashtable _cmdTable;
    private Hashtable _watchVarTable;
    private StringBuilder _displayString;    
    private string _inputString = "";

    private Rect _windowRect;
    private Vector2 _logScrollPos = Vector2.zero;
    private Vector2 _watchVarsScrollPos = Vector2.zero;
    private bool _isOpen;

    /// <summary>
    /// Represents a single message, with formatting options.
    /// </summary>
    private struct Message
    {
        public string MessageText
        {
            get { return _messageText; }
            set { _messageText = value; }
        }

        public MessageTypes MessageType
        {
            get { return _messageType; }
            set { _messageType = value; }
        }

        public Color DisplayColor
        {
            get { return _displayColor; }
            set { _displayColor = value; }
        }

        public bool UseCustomColor
        {
            get { return _useCustomColor; }
            set { _useCustomColor = value; }
        }

        private string _messageText;       
        private MessageTypes _messageType;        
        private Color _displayColor;       
        private bool _useCustomColor;

        public Message( string messageText )
        {
            this._messageText = messageText;
            this._messageType = MessageTypes.Normal;
            this._displayColor = new Color();
            this._useCustomColor = false;
        }

        public Message( string messageText, MessageTypes messageType )
        {
            this._messageText = messageText;
            this._messageType = messageType;
            this._displayColor = new Color();
            this._useCustomColor = false;
        }

        public Message( string messageText, MessageTypes messageType, Color displayColor )
        {
            this._messageText = messageText;
            this._messageType = messageType;
            this._displayColor = displayColor;
            this._useCustomColor = true;
        }

        public Message( string messageText, Color displayColor )
        {
            this._messageText = messageText;
            this._messageType = MessageTypes.Normal;
            this._displayColor = displayColor;
            this._useCustomColor = true;
        }
    }

    private List<Message> _messages;

    void Awake()
    {
        _instance = this;

        //_messages = new List<Message>();
		
		//リストの最大数を設定する
		_messages = new List<Message>(_maxLinesForDisplay);
		
        _windowRect = new Rect( 100.0F, 100.0F, 450.0F, 600.0F );
        _cmdTable = new Hashtable();
        _watchVarTable = new Hashtable();      
        _displayString = new StringBuilder();
		
		/*
        _messages.Add( new Message( " DebugConsole version " + VERSION.ToString( "F2" ), MessageTypes.System ) );
        _messages.Add( new Message( " Copyright 2008-2010 Jeremy Hollingsworth ", MessageTypes.System ) );
        _messages.Add( new Message( " Ennanzus-Interactive.com ", MessageTypes.System ) );
        _messages.Add( new Message( " " ) );
         */
		
        this.RegisterCommandCallback( "close", CMDClose );
        this.RegisterCommandCallback( "clear", CMDClear );
    }

    void OnGUI()
    {
        if ( _isOpen )
        {
			GUI.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			
            _windowRect = GUI.Window( -1111, _windowRect, MainWindow, "Debug Console  v" + VERSION.ToString( "F2" ) + " Ennanzus-Interactive.com" );
            GUI.BringWindowToFront( -1111 );
        }
    }

    #region StaticAccessors

    /// <summary>
    /// Prints a message string to the console.
    /// </summary>
    /// <param name="text">Message to print.</param>
    public static void Log( string text )
    {
        DebugConsole.Instance.LogMessage( text );
    }

    /// <summary>
    /// Prints a message string to the console.
    /// </summary>
    /// <param name="text">Message to print.</param>
    /// <param name="messageType">The MessageType of the message. Used to provide 
    /// formatting in order to distinguish between message types.</param>
    public static void Log( string text, MessageTypes messageType )
    {
        DebugConsole.Instance.LogMessage( text, messageType );
    }

    /// <summary>
    /// Prints a message string to the console.
    /// </summary>
    /// <param name="text">Message to print.</param>
    /// <param name="displayColor">The text color to use when displaying the message.</param>
    public static void Log( string text, Color displayColor )
    {
        DebugConsole.Instance.LogMessage( text, displayColor );
    }

    /// <summary>
    /// Prints a message string to the console.
    /// </summary>
    /// <param name="text">Messate to print.</param>
    /// <param name="messageType">The MessageType of the message. Used to provide 
    /// formatting in order to distinguish between message types.</param>
    /// <param name="displayColor">The color to use when displaying the message.</param>
    /// <param name="useCustomColor">Flag indicating if the displayColor value should be used or 
    /// if the default color for the message type should be used instead.</param>
    public static void Log( string text, MessageTypes messageType, Color displayColor, bool useCustomColor )
    {
        DebugConsole.Instance.LogMessage( text, messageType, displayColor, useCustomColor );
    }

    /// <summary>
    /// Prints a message string to the console using the "Warning" message type formatting.
    /// </summary>
    /// <param name="text">Message to print.</param>
    public static void LogWarning( string text )
    {
        DebugConsole.Instance.LogMessage( text, MessageTypes.Warning );
    }

    /// <summary>
    /// Prints a message string to the console using the "Error" message type formatting.
    /// </summary>
    /// <param name="text">Message to print.</param>
    public static void LogError( string text )
    {
        DebugConsole.Instance.LogMessage( text, MessageTypes.Error );
    }

    /// <summary>
    /// Clears all console output.
    /// </summary>
    public static void Clear()
    {
        DebugConsole.Instance.ClearLog();
    }

    /// <summary>
    /// Registers a debug command that is "fired" when the specified command string is entered.
    /// </summary>
    /// <param name="commandString">The string that represents the command. For example: "FOV"</param>
    /// <param name="commandCallback">The method/function to call with the commandString is entered. 
    /// For example: "SetFOV"</param>
    public static void RegisterCommand( string commandString, DebugCommand commandCallback )
    {
        DebugConsole.Instance.RegisterCommandCallback( commandString, commandCallback );
    }
    
    /// <summary>
    /// Removes a previously-registered debug command.
    /// </summary>
    /// <param name="commandString">The string that represents the command.</param>
    public static void UnRegisterCommand( string commandString )
    {
        DebugConsole.Instance.UnRegisterCommandCallback( commandString );
    }

    /// <summary>
    /// Registers a named "watch var" for monitoring.
    /// </summary>
    /// <param name="name">Name of the watch var to be shown in the console.</param>
    /// <param name="watchVar">The WatchVar instance you want to monitor.</param>
    public static void RegisterWatchVar( string name, WatchVar watchVar )
    {
        DebugConsole.Instance.AddWatchVarToTable( name, watchVar );
    }

    /// <summary>
    /// Removes a previously-registered watch var.
    /// </summary>
    /// <param name="name">Name of the watch var you wish to remove.</param>
    public static void UnRegisterWatchVar( string name )
    {
        DebugConsole.Instance.RemoveWatchVarFromTable( name );
    }

    //==== Built-in example DebugCommand handlers ====
    public void CMDClose( params string[] args )
    {
        _isOpen = false;
    }

    public void CMDClear( params string[] args )
    {
        this.ClearLog();
    }

    #endregion

    #region InternalFunctionality

    private void MainWindow( int windowID )
    {
        if ( _mode == Mode.Log )
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space( 5.0F );

            GUILayout.BeginVertical();

            GUILayout.Space( 5.0F );

            GUILayout.Label( "Normal Log:" );

            _logScrollPos = GUILayout.BeginScrollView( _logScrollPos );

            DisplayNormalLog();

            GUILayout.EndScrollView();

            GUILayout.Space( 4.0F );

            GUILayout.BeginHorizontal();
            _inputString = GUILayout.TextField( _inputString );

            if ( GUILayout.Button( "Enter", GUILayout.Width( 70.0F ), GUILayout.Height( 20.0F ) ) )
            {
                EvalInputString( _inputString );
                _inputString = "";
            }

            GUILayout.EndHorizontal();

            GUILayout.Space( 4.0F );

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if ( GUILayout.Button( "Copy Log", GUILayout.Width( 95.0F ), GUILayout.Height( 20.0F ) ) )
            {
                _mode = Mode.CopyLog;
            }

            if ( GUILayout.Button( "Watch Vars", GUILayout.Width( 95.0F ), GUILayout.Height( 20.0F ) ) )
            {
                _mode = Mode.WatchVars;
            }
			
			if ( GUILayout.Button( "Clear", GUILayout.Width( 95.0F ), GUILayout.Height( 20.0F ) ) )
            {
                Clear();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        else if ( _mode == Mode.CopyLog )
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space( 5.0F );

            GUILayout.BeginVertical();

            GUILayout.Space( 5.0F );

            GUILayout.Label( "Copy Log:" );
            _logScrollPos = GUILayout.BeginScrollView( _logScrollPos );

            BuildDisplayString();

            GUILayout.TextArea( _displayString.ToString(), GUILayout.ExpandWidth( false ), GUILayout.ExpandHeight( true ), GUILayout.Width( 255.0F ) );
            GUILayout.EndScrollView();

            GUILayout.Space( 4.0F );

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if ( GUILayout.Button( "Normal Mode", GUILayout.Width( 95.0F ), GUILayout.Height( 20.0F ) ) )
            {
                _mode = Mode.Log;
            }

            if ( GUILayout.Button( "Watch Vars", GUILayout.Width( 95.0F ), GUILayout.Height( 20.0F ) ) )
            {
                _mode = Mode.WatchVars;
            }
			
			if ( GUILayout.Button( "Clear", GUILayout.Width( 95.0F ), GUILayout.Height( 20.0F ) ) )
            {
                Clear();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        else if ( _mode == Mode.WatchVars )
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space( 5.0F );

            GUILayout.BeginVertical();

            GUILayout.Space( 5.0F );

            GUILayout.Label( "Watched Vars:" );

            _watchVarsScrollPos = GUILayout.BeginScrollView( _watchVarsScrollPos );

            foreach ( string key in _watchVarTable.Keys )
            {
                GUILayout.Space( 2.0F );

                GUILayout.BeginHorizontal();
                GUILayout.Label( ( (WatchVar)_watchVarTable[key] ).Name + ": " );
                GUILayout.FlexibleSpace();
                GUILayout.Label( ( (WatchVar)_watchVarTable[key] ).GetValue().ToString() );
                GUILayout.Space( 2.0F );
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.Space( 4.0F );

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if ( GUILayout.Button( "Normal Mode", GUILayout.Width( 95.0F ), GUILayout.Height( 20.0F ) ) )
            {
                _mode = Mode.Log;
            }

            if ( GUILayout.Button( "Copy Log", GUILayout.Width( 95.0F ), GUILayout.Height( 20.0F ) ) )
            {
                _mode = Mode.CopyLog;
            }
			
			if ( GUILayout.Button( "Clear", GUILayout.Width( 95.0F ), GUILayout.Height( 20.0F ) ) )
            {
                Clear();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }


        GUI.DragWindow();
    }  

    //--- Local version. Use the static version above instead.
    public void LogMessage( string text )
    {
        LogMessage( text, MessageTypes.Normal, Color.white, false );
    }

    //--- Local version. Use the static version above instead.
    public void LogMessage( string text, MessageTypes messageType )
    {
        LogMessage( text, messageType, Color.white, false );
    }

    //--- Local version. Use the static version above instead.
    public void LogMessage( string text, Color displayColor )
    {
        LogMessage( text, MessageTypes.Normal, displayColor, true );
    }

    //--- Local version. Use the static version above instead.
    public void LogMessage( string text, MessageTypes messageType, Color displayColor, bool useCustomColor )
    {
		//出力機能の有効化をチェックする
		if( !_enable )
			return;
		
        if ( _messages != null )
        {
			//リスト数が最大の場合
			if(( _messages.Count + 1) > _maxLinesForDisplay )
				_messages.RemoveAt(0);
			
            if ( useCustomColor )
            {
                _messages.Add( new Message( text, messageType, displayColor ) );
            }
            else
            {
                _messages.Add( new Message( text, messageType ) );
            }

            _logScrollPos = new Vector2( _logScrollPos.x, 50000.0F );
        }
    }

    //--- Local version. Use the static version above instead.
    public void ClearLog()
    {
        _messages.Clear();   
    }
      
    //--- Local version. Use the static version above instead.
    public void RegisterCommandCallback( string commandString, DebugCommand commandCallback )
    {
        _cmdTable.Add( commandString.ToLower(), new DebugCommand( commandCallback ) );
    }
        
    //--- Local version. Use the static version above instead.
    public void UnRegisterCommandCallback( string commandString )
    {
        if ( _cmdTable.ContainsKey( commandString.ToLower() ) )
        {
            _cmdTable.Remove( commandString );
        }
    }
       
    //--- Local version. Use the static version above instead.
    public void AddWatchVarToTable( string name, WatchVar watchVar )
    {
        _watchVarTable.Add( name, watchVar );
    }   
       
    //--- Local version. Use the static version above instead.
    public void RemoveWatchVarFromTable( string name )
    {
        if ( _watchVarTable.ContainsKey( name ) )
        {
            _watchVarTable.Remove( name );
        }
    }

    private void DisplayNormalLog()
    {
        if ( _messages != null && _messages.Count > 0 )
        {
            foreach ( Message m in _messages )
            {
                // Default text color                
                Color displayColor = _defaultTextColor;

                if ( m.UseCustomColor )
                {
                    displayColor = m.DisplayColor;
                }
                else if ( m.MessageType == MessageTypes.Error )
                {
                    displayColor = Color.red;
                }
                else if ( m.MessageType == MessageTypes.Warning )
                {
                    displayColor = Color.yellow;
                }
                else if ( m.MessageType == MessageTypes.System )
                {
                    displayColor = Color.green;
                }
                else if ( m.MessageType == MessageTypes.Command )
                {
                    displayColor = Color.magenta;
                }

                Color oldColor = GUI.color;
                GUI.color = displayColor;
                GUILayout.Label( ">> " + m.MessageText );
                GUI.color = oldColor;
            }
        }
    }

    private void BuildDisplayString()
    {
        if ( _messages != null && _messages.Count > 0 )
        {
            _displayString = new StringBuilder();

            foreach ( Message m in _messages )
            {
                string messageTypeString = "";

                if ( m.UseCustomColor == false )
                {
                    if ( m.MessageType == MessageTypes.Error )
                    {
                        messageTypeString = "error";
                    }
                    else if ( m.MessageType == MessageTypes.Warning )
                    {
                        messageTypeString = "warning";
                    }
                    else if ( m.MessageType == MessageTypes.System )
                    {
                        messageTypeString = "system";
                    }
                    else if ( m.MessageType == MessageTypes.Command )
                    {
                        messageTypeString = "command";
                    }
                }
                else
                {
                    messageTypeString = "customColor(" + m.DisplayColor.ToString() + ")";
                }

                if ( !string.IsNullOrEmpty( messageTypeString ) )
                {
                    _displayString.AppendLine(">> [" + messageTypeString + "]" + m.MessageText + "[/" + messageTypeString + "]" );
                }
                else
                {
                    _displayString.AppendLine( ">> " + m.MessageText );
                }
            }
        }
    }
       
    private void EvalInputString( string inputString )
    {
        if ( inputString.ToLower() == "/?" )
        {
            Log( "Command List: ", MessageTypes.System );

            foreach ( string key in _cmdTable.Keys )
            {
                Log( key, MessageTypes.System );
            }

            Log( "End Of Command List", MessageTypes.System );
            return;
        }                

        string[] seperators = new string[1];
        seperators[0] = " ";
        string[] input = inputString.Split( seperators, StringSplitOptions.None );

        string[] args = new string[input.Length - 1];
        
        for ( int x = 0; x < args.Length; x++ )
        {
            args[x] = input[x + 1].ToLower();
        }

        if ( _cmdTable.ContainsKey( input[0].ToLower() ) )
        {
            ( (DebugCommand)_cmdTable[input[0].ToLower()] ).Invoke( args );
            Log( inputString, MessageTypes.Command );
        }
        else
        {
            Log( "***Unknown Command***", MessageTypes.System );
        }
    }

    #endregion
}

/// <summary>
/// Base class for all WatchVars. Provides base functionality.
/// </summary>
public abstract class WatchVar
{
    /// <summary>
    /// Name of the WatchVar.
    /// </summary>
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    private string _name = "Default WatchVar";

    public WatchVar( string name )
    {
        this._name = name;
        DebugConsole.RegisterWatchVar( _name, this );
    }

    public abstract object GetValue();        
}

/// <summary>
/// A WatchVar designed to monitor a float type variable.
/// </summary>
public class WatchFloat : WatchVar
{
    /// <summary>
    /// Gets or sets the value of this WatchFloat.
    /// </summary>
    public float Value
    {
        get { return _value; }
        set { _value = value; }
    }

    private float _value;

    public WatchFloat( string name )
        : base( name )
    {
        //...
    }

    public override object GetValue()
    {
        return (object)_value;
    }
}

/// <summary>
/// A WatchVar designed to monitor an int type variable.
/// </summary>
public class WatchInt : WatchVar
{
    /// <summary>
    /// Gets or sets the value of this WatchInt.
    /// </summary>
    public int Value
    {
        get { return _value; }
        set { _value = value; }
    }

    private int _value;

    public WatchInt( string name )
        : base( name )
    {
        //...
    }

    public override object GetValue()
    {
        return (object)_value;
    }
}

/// <summary>
/// A WatchVar designed to monitor a boolean type variable.
/// </summary>
public class WatchBool : WatchVar
{
    /// <summary>
    /// Gets or sets the value of this WatchBool.
    /// </summary>
    public bool Value
    {
        get { return _value; }
        set { _value = value; }
    }

    private bool _value;

    public WatchBool( string name )
        : base( name )
    {
        //...
    }

    public override object GetValue()
    {
        return (object)_value;
    }
}

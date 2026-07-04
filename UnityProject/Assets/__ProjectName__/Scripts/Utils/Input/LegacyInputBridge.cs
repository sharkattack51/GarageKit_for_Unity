#define USE_INPUTSYSTEM

using System.Collections.Generic;
using UnityEngine;
#if USE_INPUTSYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
#endif

namespace GarageKit.LegacyInputBridge
{
    public class Input
    {
#if USE_INPUTSYSTEM
        private static Dictionary<KeyCode, Key> KeyCodeToKey = new Dictionary<KeyCode, Key>()
        {
            { KeyCode.Space, Key.Space },
            { KeyCode.Return, Key.Enter },
            { KeyCode.Tab, Key.Tab },
            { KeyCode.BackQuote, Key.Backquote },
            { KeyCode.Quote, Key.Quote },
            { KeyCode.Semicolon, Key.Semicolon },
            { KeyCode.Comma, Key.Comma },
            { KeyCode.Period, Key.Period },
            { KeyCode.Slash, Key.Slash },
            { KeyCode.Backslash, Key.Backslash },
            { KeyCode.LeftBracket, Key.LeftBracket },
            { KeyCode.RightBracket, Key.RightBracket },
            { KeyCode.Minus, Key.Minus },
            { KeyCode.Equals, Key.Equals },
            { KeyCode.A, Key.A },
            { KeyCode.B, Key.B },
            { KeyCode.C, Key.C },
            { KeyCode.D, Key.D },
            { KeyCode.E, Key.E },
            { KeyCode.F, Key.F },
            { KeyCode.G, Key.G },
            { KeyCode.H, Key.H },
            { KeyCode.I, Key.I },
            { KeyCode.J, Key.J },
            { KeyCode.K, Key.K },
            { KeyCode.L, Key.L },
            { KeyCode.M, Key.M },
            { KeyCode.N, Key.N },
            { KeyCode.O, Key.O },
            { KeyCode.P, Key.P },
            { KeyCode.Q, Key.Q },
            { KeyCode.R, Key.R },
            { KeyCode.S, Key.S },
            { KeyCode.T, Key.T },
            { KeyCode.U, Key.U },
            { KeyCode.V, Key.V },
            { KeyCode.W, Key.W },
            { KeyCode.X, Key.X },
            { KeyCode.Y, Key.Y },
            { KeyCode.Z, Key.Z },
            { KeyCode.Alpha1, Key.Digit1 },
            { KeyCode.Alpha2, Key.Digit2 },
            { KeyCode.Alpha3, Key.Digit3 },
            { KeyCode.Alpha4, Key.Digit4 },
            { KeyCode.Alpha5, Key.Digit5 },
            { KeyCode.Alpha6, Key.Digit6 },
            { KeyCode.Alpha7, Key.Digit7 },
            { KeyCode.Alpha8, Key.Digit8 },
            { KeyCode.Alpha9, Key.Digit9 },
            { KeyCode.Alpha0, Key.Digit0 },
            { KeyCode.LeftShift, Key.LeftShift },
            { KeyCode.RightShift, Key.RightShift },
            { KeyCode.LeftAlt, Key.LeftAlt },
            { KeyCode.RightAlt, Key.RightAlt },
            //{ KeyCode.AltGr, Key.RightAlt },
            { KeyCode.LeftControl, Key.LeftCtrl },
            { KeyCode.RightControl, Key.RightCtrl },
            { KeyCode.LeftMeta, Key.LeftMeta },
            { KeyCode.RightMeta, Key.RightMeta },
            { KeyCode.LeftWindows, Key.LeftMeta },
            { KeyCode.RightWindows, Key.RightMeta },
            //{ KeyCode.LeftApple, Key.LeftMeta },
            //{ KeyCode.RightApple, Key.RightMeta },
            //{ KeyCode. LeftCommand, Key.LeftMeta },
            //{ KeyCode.RightCommand, Key.RightMeta },
            { KeyCode.Menu, Key.ContextMenu },
            { KeyCode.Escape, Key.Escape },
            { KeyCode.LeftArrow, Key.LeftArrow },
            { KeyCode.RightArrow, Key.RightArrow },
            { KeyCode.UpArrow, Key.UpArrow },
            { KeyCode.DownArrow, Key.DownArrow },
            { KeyCode.Backspace, Key.Backspace },
            { KeyCode.PageDown, Key.PageDown },
            { KeyCode.PageUp, Key.PageUp },
            { KeyCode.Home, Key.Home },
            { KeyCode.End, Key.End },
            { KeyCode.Insert, Key.Insert },
            { KeyCode.Delete, Key.Delete },
            { KeyCode.CapsLock, Key.CapsLock },
            { KeyCode.Numlock, Key.NumLock },
            { KeyCode.Print, Key.PrintScreen },
            { KeyCode.ScrollLock, Key.ScrollLock },
            { KeyCode.Pause, Key.Pause },
            { KeyCode.KeypadEnter, Key.NumpadEnter },
            { KeyCode.KeypadDivide, Key.NumpadDivide },
            { KeyCode.KeypadMultiply, Key.NumpadMultiply },
            { KeyCode.KeypadPlus, Key.NumpadPlus },
            { KeyCode.KeypadMinus, Key.NumpadMinus },
            { KeyCode.KeypadPeriod, Key.NumpadPeriod },
            { KeyCode.KeypadEquals, Key.NumpadEquals },
            { KeyCode.Keypad0, Key.Numpad0 },
            { KeyCode.Keypad1, Key.Numpad1 },
            { KeyCode.Keypad2, Key.Numpad2 },
            { KeyCode.Keypad3, Key.Numpad3 },
            { KeyCode.Keypad4, Key.Numpad4 },
            { KeyCode.Keypad5, Key.Numpad5 },
            { KeyCode.Keypad6, Key.Numpad6 },
            { KeyCode.Keypad7, Key.Numpad7 },
            { KeyCode.Keypad8, Key.Numpad8 },
            { KeyCode.Keypad9, Key.Numpad9 },
            { KeyCode.F1, Key.F1 },
            { KeyCode.F2, Key.F2 },
            { KeyCode.F3, Key.F3 },
            { KeyCode.F4, Key.F4 },
            { KeyCode.F5, Key.F5 },
            { KeyCode.F6, Key.F6 },
            { KeyCode.F7, Key.F7 },
            { KeyCode.F8, Key.F8 },
            { KeyCode.F9, Key.F9 },
            { KeyCode.F10, Key.F10 },
            { KeyCode.F11, Key.F11 },
            { KeyCode.F12, Key.F12 },
        };
#endif

        public static bool GetKeyDown(KeyCode keyCode)
        {
#if USE_INPUTSYSTEM
            return Keyboard.current[KeyCodeToKey[keyCode]].wasPressedThisFrame;
#else
            return UnityEngine.Input.GetKeyDown(keyCode);
#endif
        }

        public static bool GetKeyUp(KeyCode keyCode)
        {
#if USE_INPUTSYSTEM
            return Keyboard.current[KeyCodeToKey[keyCode]].wasReleasedThisFrame;
#else
            return UnityEngine.Input.GetKeyUp(keyCode);
#endif
        }

        public static bool GetKey(KeyCode keyCode)
        {
#if USE_INPUTSYSTEM
            return Keyboard.current[KeyCodeToKey[keyCode]].isPressed;
#else

            return UnityEngine.Input.GetKey(keyCode);
#endif
        }

        public static bool GetMouseButtonDown(int btn)
        {
#if USE_INPUTSYSTEM
            return btn switch
            {
                0 => Mouse.current.leftButton.wasPressedThisFrame,
                1 => Mouse.current.rightButton.wasPressedThisFrame,
                2 => Mouse.current.middleButton.wasPressedThisFrame,
                _ => Mouse.current.leftButton.wasPressedThisFrame
            };
#else
            return UnityEngine.Input.GetMouseButtonDown(btn);
#endif
        }

        public static bool GetMouseButtonUp(int btn)
        {
#if USE_INPUTSYSTEM
            return btn switch
            {
                0 => Mouse.current.leftButton.wasReleasedThisFrame,
                1 => Mouse.current.rightButton.wasReleasedThisFrame,
                2 => Mouse.current.middleButton.wasReleasedThisFrame,
                _ => Mouse.current.leftButton.wasReleasedThisFrame
            };
#else
            return UnityEngine.Input.GetMouseButtonUp(btn);
#endif
        }

        public static bool GetMouseButton(int btn)
        {
#if USE_INPUTSYSTEM
            return btn switch
            {
                0 => Mouse.current.leftButton.isPressed,
                1 => Mouse.current.rightButton.isPressed,
                2 => Mouse.current.middleButton.isPressed,
                _ => Mouse.current.leftButton.isPressed
            };
#else
            return UnityEngine.Input.GetMouseButton(btn);
#endif
        }

        public static Vector3 mousePosition
        {
            get {
#if USE_INPUTSYSTEM
                return (Vector3)Mouse.current.position.value;
#else
                return UnityEngine.Input.mousePosition;
#endif
            }
        }

        public static Vector2 mousePositionDelta
        {
            get {
#if USE_INPUTSYSTEM
                return Mouse.current.delta.ReadValue();
#else
                return UnityEngine.Input.mousePositionDelta;
#endif
            }
        }

        public static Vector2 mouseScrollDelta
        {
            get {
#if USE_INPUTSYSTEM
                return Mouse.current.scroll.value;
#else
                return UnityEngine.Input.mouseScrollDelta;
#endif
            }
        }

        public static float GetAxisRaw(string axisName)
        {
#if USE_INPUTSYSTEM
            return axisName switch
            {
                "Mouse X" => Mouse.current.delta.ReadValue().x,
                "Mouse Y" => Mouse.current.delta.ReadValue().y,
                _ => 0.0f
            };
#else
            return UnityEngine.Input.GetAxisRaw(axisName);
#endif
        }

        public static int touchCount
        {
            get {
#if USE_INPUTSYSTEM
                if(!EnhancedTouchSupport.enabled)
                    EnhancedTouchSupport.Enable();
                return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;
#else
                return UnityEngine.Input.touchCount;
#endif
            }
        }

        public static UnityEngine.Touch GetTouch(int index)
        {
#if USE_INPUTSYSTEM
            if(!EnhancedTouchSupport.enabled)
                EnhancedTouchSupport.Enable();
            if(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > index)
            {
                UnityEngine.InputSystem.EnhancedTouch.Touch touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[index];
                return new UnityEngine.Touch()
                {
                    deltaPosition = touch.delta,
                    deltaTime = (float)(touch.startTime - touch.time),
                    phase = touch.phase switch
                    {
                        UnityEngine.InputSystem.TouchPhase.None => UnityEngine.TouchPhase.Canceled,
                        UnityEngine.InputSystem.TouchPhase.Began => UnityEngine.TouchPhase.Began,
                        UnityEngine.InputSystem.TouchPhase.Moved => UnityEngine.TouchPhase.Moved,
                        UnityEngine.InputSystem.TouchPhase.Ended => UnityEngine.TouchPhase.Ended,
                        UnityEngine.InputSystem.TouchPhase.Canceled => UnityEngine.TouchPhase.Canceled,
                        UnityEngine.InputSystem.TouchPhase.Stationary => UnityEngine.TouchPhase.Stationary,
                        _ => UnityEngine.TouchPhase.Canceled
                    },
                    position = touch.screenPosition,
                    tapCount = touch.tapCount,

                };
            }
            else
                return new UnityEngine.Touch();
#else
            return UnityEngine.Input.GetTouch(index);
#endif
        }
    }
}

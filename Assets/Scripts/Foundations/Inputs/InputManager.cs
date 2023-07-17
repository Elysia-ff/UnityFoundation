using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Inputs
{
    public partial class InputManager : MonoBehaviour
    {
        public delegate void OnAxisEvent(float value);
        public delegate void OnKeyEvent(EModifier modifier);
        public delegate void OnMouseEvent(Vector2 position, EModifier modifier);

        private class AxisEventValue
        {
            public OnAxisEvent events;
        }

        private class KeyEventValue
        {
            public float lastPressedTime;
            public float lastReleasedTime;
            public int tapCount;
            public OnKeyEvent[] events = new OnKeyEvent[(int)EInputType.Count];
        }

        private class MouseEventValue
        {
            public float lastPressedTime;
            public Vector2 lastPressedPosition;
            public float lastReleasedTime;
            public Vector2 lastReleasedPosition;
            public int tapCount;
            public Vector2 lastTapPosition;
            public OnMouseEvent[] events = new OnMouseEvent[(int)EInputType.Count];
        }

        private readonly Dictionary<string, AxisEventValue> _axisEvents = new Dictionary<string, AxisEventValue>();
        private readonly Dictionary<KeyCode, KeyEventValue> _keyEvents = new Dictionary<KeyCode, KeyEventValue>();
        private readonly Dictionary<int, MouseEventValue> _mouseEvents = new Dictionary<int, MouseEventValue>();

        private const float TAP_THRESHOLD = 0.2f;
        private const float TAP_DELTA_THRESHOLD = 100f; // squared
        private const float DOUBLE_TAP_THRESHOLD = 0.2f;

        public void BindAxis(string axis, OnAxisEvent onAxisEvent)
        {
            if (!_axisEvents.TryGetValue(axis, out AxisEventValue value))
            {
                value = new AxisEventValue();
                _axisEvents.Add(axis, value);
            }

            value.events += onAxisEvent;
        }

        public void UnbindAxis(string axis, OnAxisEvent onAxisEvent)
        {
            if (!_axisEvents.TryGetValue(axis, out AxisEventValue value))
            {
                return;
            }

            value.events -= onAxisEvent;
        }

        public void BindKey(KeyCode key, EInputType inputType, OnKeyEvent onKeyEvent)
        {
            if (!_keyEvents.TryGetValue(key, out KeyEventValue value))
            {
                value = new KeyEventValue();
                _keyEvents.Add(key, value);
            }

            value.events[(int)inputType] += onKeyEvent;
        }

        public void UnbindKey(KeyCode key, EInputType inputType, OnKeyEvent onKeyEvent)
        {
            if (!_keyEvents.TryGetValue(key, out KeyEventValue value))
            {
                return;
            }

            value.events[(int)inputType] -= onKeyEvent;
        }

        public void BindMouse(int button, EInputType inputType, OnMouseEvent onMouseEvent)
        {
            if (!_mouseEvents.TryGetValue(button, out MouseEventValue value))
            {
                value = new MouseEventValue();
                _mouseEvents.Add(button, value);
            }

            value.events[(int)inputType] += onMouseEvent;
        }

        public void UnbindMouse(int button, EInputType inputType, OnMouseEvent onMouseEvent)
        {
            if (!_mouseEvents.TryGetValue(button, out MouseEventValue value))
            {
                return;
            }

            value.events[(int)inputType] -= onMouseEvent;
        }

        private void Update()
        {
            #region Axis
            foreach (var kv in _axisEvents)
            {
                float value = Input.GetAxis(kv.Key);
                kv.Value.events?.Invoke(value);
            }
            #endregion

            #region Key
            EModifier modifier = GetModifier();

            foreach (var kv in _keyEvents)
            {
                KeyCode keyCode = kv.Key;
                KeyEventValue eventValue = kv.Value;

                if (Input.GetKeyDown(keyCode))
                {
                    eventValue.lastPressedTime = Time.time;
                    if (eventValue.lastPressedTime - eventValue.lastReleasedTime is <= 0f or > DOUBLE_TAP_THRESHOLD)
                    {
                        eventValue.tapCount = 0;
                    }

                    eventValue.events[(int)EInputType.Pressed]?.Invoke(modifier);
                }
                else
                {
                    bool isHolding = Input.GetKey(keyCode);
                    if (isHolding)
                    {
                        eventValue.events[(int)EInputType.Holding]?.Invoke(modifier);
                    }
                    else if (eventValue.lastPressedTime > eventValue.lastReleasedTime)
                    {
                        eventValue.lastReleasedTime = Time.time;
                        if (eventValue.lastReleasedTime - eventValue.lastPressedTime is > 0f and <= TAP_THRESHOLD)
                        {
                            eventValue.tapCount++;
                        }
                        else
                        {
                            eventValue.tapCount = 0;
                        }

                        eventValue.events[(int)EInputType.Released]?.Invoke(modifier);

                        switch (eventValue.tapCount)
                        {
                            case 0:
                                break;

                            case 1:
                                eventValue.events[(int)EInputType.SingleTap]?.Invoke(modifier);
                                break;

                            case 2:
                                eventValue.events[(int)EInputType.DoubleTap]?.Invoke(modifier);
                                eventValue.tapCount = 0;
                                break;

                            default:
                                Debug.Assert(false, "unreachable");
                                break;
                        }
                    }
                }
            }
            #endregion

            #region Mouse
            Vector2 mousePosition = GetMousePosition();
            foreach (var kv in _mouseEvents)
            {
                int button = kv.Key;
                MouseEventValue eventValue = kv.Value;

                if (GetMouseButtonDown(button))
                {
                    eventValue.lastPressedTime = Time.time;
                    eventValue.lastPressedPosition = mousePosition;
                    if (eventValue.lastPressedTime - eventValue.lastReleasedTime is <= 0f or > DOUBLE_TAP_THRESHOLD ||
                        (eventValue.lastPressedPosition - eventValue.lastTapPosition).sqrMagnitude >= TAP_DELTA_THRESHOLD)
                    {
                        eventValue.tapCount = 0;
                    }

                    eventValue.events[(int)EInputType.Pressed]?.Invoke(mousePosition, modifier);
                }
                else
                {
                    bool isHolding = GetMouseButton(button);
                    if (isHolding)
                    {
                        eventValue.events[(int)EInputType.Holding]?.Invoke(mousePosition, modifier);
                    }
                    else if (eventValue.lastPressedTime > eventValue.lastReleasedTime)
                    {
                        eventValue.lastReleasedTime = Time.time;
                        eventValue.lastReleasedPosition = mousePosition;
                        if (eventValue.lastReleasedTime - eventValue.lastPressedTime is > 0f and <= TAP_THRESHOLD &&
                            (eventValue.lastReleasedPosition - eventValue.lastPressedPosition).sqrMagnitude < TAP_DELTA_THRESHOLD)
                        {
                            eventValue.tapCount++;
                            eventValue.lastTapPosition = mousePosition;
                        }
                        else
                        {
                            eventValue.tapCount = 0;
                        }

                        eventValue.events[(int)EInputType.Released]?.Invoke(mousePosition, modifier);

                        switch (eventValue.tapCount)
                        {
                            case 0:
                                break;

                            case 1:
                                eventValue.events[(int)EInputType.SingleTap]?.Invoke(mousePosition, modifier);
                                break;

                            case 2:
                                eventValue.events[(int)EInputType.DoubleTap]?.Invoke(mousePosition, modifier);
                                eventValue.tapCount = 0;
                                break;

                            default:
                                Debug.Assert(false, "unreachable");
                                break;
                        }
                    }
                }
            }
            #endregion
        }

        private EModifier GetModifier()
        {
            EModifier modifier = 0;

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                modifier |= EModifier.Ctrl;
            }

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                modifier |= EModifier.Shift;
            }

            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                modifier |= EModifier.Alt;
            }

            return modifier;
        }
    }
}

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
            public readonly string axis;
            public OnAxisEvent events;

            public AxisEventValue(string _axis)
            {
                axis = _axis;
            }
        }

        private class KeyEventValue
        {
            public readonly KeyCode key;
            public float lastPressedTime;
            public float lastReleasedTime;
            public int tapCount;
            public OnKeyEvent[] events = new OnKeyEvent[(int)EInputType.Count];

            public KeyEventValue(KeyCode _key)
            {
                key = _key;
            }
        }

        private class MouseEventValue
        {
            public readonly int button;
            public float lastPressedTime;
            public Vector2 lastPressedPosition;
            public float lastReleasedTime;
            public Vector2 lastReleasedPosition;
            public int tapCount;
            public Vector2 lastTapPosition;
            public OnMouseEvent[] events = new OnMouseEvent[(int)EInputType.Count];

            public MouseEventValue(int _button)
            {
                button = _button;
            }
        }

        private readonly List<AxisEventValue> _axisEvents = new List<AxisEventValue>();
        private readonly List<KeyEventValue> _keyEvents = new List<KeyEventValue>();
        private readonly List<MouseEventValue> _mouseEvents = new List<MouseEventValue>();

        private const float TAP_THRESHOLD = 0.2f;
        private const float TAP_DELTA_THRESHOLD = 100f; // squared
        private const float DOUBLE_TAP_THRESHOLD = 0.2f;

        public void BindAxis(string axis, OnAxisEvent onAxisEvent)
        {
            AxisEventValue value = _axisEvents.Find(v => v.axis == axis);
            if (value == null)
            {
                value = new AxisEventValue(axis);
                _axisEvents.Add(value);
            }

            value.events += onAxisEvent;
        }

        public void UnbindAxis(string axis, OnAxisEvent onAxisEvent)
        {
            AxisEventValue value = _axisEvents.Find(v => v.axis == axis);
            if (value == null)
            {
                return;
            }

            value.events -= onAxisEvent;
        }

        public void BindKey(KeyCode key, EInputType inputType, OnKeyEvent onKeyEvent)
        {
            KeyEventValue value = _keyEvents.Find(v => v.key == key);
            if (value == null)
            {
                value = new KeyEventValue(key);
                _keyEvents.Add(value);
            }

            value.events[(int)inputType] += onKeyEvent;
        }

        public void UnbindKey(KeyCode key, EInputType inputType, OnKeyEvent onKeyEvent)
        {
            KeyEventValue value = _keyEvents.Find(v => v.key == key);
            if (value == null)
            {
                return;
            }

            value.events[(int)inputType] -= onKeyEvent;
        }

        public void BindMouse(int button, EInputType inputType, OnMouseEvent onMouseEvent)
        {
            MouseEventValue value = _mouseEvents.Find(v => v.button == button);
            if (value == null)
            {
                value = new MouseEventValue(button);
                _mouseEvents.Add(value);
            }

            value.events[(int)inputType] += onMouseEvent;
        }

        public void UnbindMouse(int button, EInputType inputType, OnMouseEvent onMouseEvent)
        {
            MouseEventValue value = _mouseEvents.Find(v => v.button == button);
            if (value == null)
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
                float value = Input.GetAxis(kv.axis);
                kv.events?.Invoke(value);
            }
            #endregion

            #region Key
            EModifier modifier = GetModifier();

            foreach (var kv in _keyEvents)
            {
                KeyCode keyCode = kv.key;

                if (Input.GetKeyDown(keyCode))
                {
                    kv.lastPressedTime = Time.time;
                    if (kv.lastPressedTime - kv.lastReleasedTime is <= 0f or > DOUBLE_TAP_THRESHOLD)
                    {
                        kv.tapCount = 0;
                    }

                    kv.events[(int)EInputType.Pressed]?.Invoke(modifier);
                }
                else
                {
                    bool isHolding = Input.GetKey(keyCode);
                    if (isHolding)
                    {
                        kv.events[(int)EInputType.Holding]?.Invoke(modifier);
                    }
                    else if (kv.lastPressedTime > kv.lastReleasedTime)
                    {
                        kv.lastReleasedTime = Time.time;
                        if (kv.lastReleasedTime - kv.lastPressedTime is > 0f and <= TAP_THRESHOLD)
                        {
                            kv.tapCount++;
                        }
                        else
                        {
                            kv.tapCount = 0;
                        }

                        kv.events[(int)EInputType.Released]?.Invoke(modifier);

                        switch (kv.tapCount)
                        {
                            case 0:
                                break;

                            case 1:
                                kv.events[(int)EInputType.SingleTap]?.Invoke(modifier);
                                break;

                            case 2:
                                kv.events[(int)EInputType.DoubleTap]?.Invoke(modifier);
                                kv.tapCount = 0;
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
                int button = kv.button;

                if (GetMouseButtonDown(button))
                {
                    kv.lastPressedTime = Time.time;
                    kv.lastPressedPosition = mousePosition;
                    if (kv.lastPressedTime - kv.lastReleasedTime is <= 0f or > DOUBLE_TAP_THRESHOLD ||
                        (kv.lastPressedPosition - kv.lastTapPosition).sqrMagnitude >= TAP_DELTA_THRESHOLD)
                    {
                        kv.tapCount = 0;
                    }

                    kv.events[(int)EInputType.Pressed]?.Invoke(mousePosition, modifier);
                }
                else
                {
                    bool isHolding = GetMouseButton(button);
                    if (isHolding)
                    {
                        kv.events[(int)EInputType.Holding]?.Invoke(mousePosition, modifier);
                    }
                    else if (kv.lastPressedTime > kv.lastReleasedTime)
                    {
                        kv.lastReleasedTime = Time.time;
                        kv.lastReleasedPosition = mousePosition;
                        if (kv.lastReleasedTime - kv.lastPressedTime is > 0f and <= TAP_THRESHOLD &&
                            (kv.lastReleasedPosition - kv.lastPressedPosition).sqrMagnitude < TAP_DELTA_THRESHOLD)
                        {
                            kv.tapCount++;
                            kv.lastTapPosition = mousePosition;
                        }
                        else
                        {
                            kv.tapCount = 0;
                        }

                        kv.events[(int)EInputType.Released]?.Invoke(mousePosition, modifier);

                        switch (kv.tapCount)
                        {
                            case 0:
                                break;

                            case 1:
                                kv.events[(int)EInputType.SingleTap]?.Invoke(mousePosition, modifier);
                                break;

                            case 2:
                                kv.events[(int)EInputType.DoubleTap]?.Invoke(mousePosition, modifier);
                                kv.tapCount = 0;
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

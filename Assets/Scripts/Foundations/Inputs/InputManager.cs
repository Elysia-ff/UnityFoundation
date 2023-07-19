using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Inputs
{
    public class InputManager : MonoBehaviour
    {
        public delegate void OnAxisEvent(float value);
        public delegate void OnKeyEvent(EModifier modifier);

        private class AxisEventValue
        {
            public OnAxisEvent events;
        }

        private class KeyEventValue
        {
            public float lastPressedTime;
            public float lastReleasedTime;
            public int tapCount;
            public OnKeyEvent[] events;

            public KeyEventValue()
            {
                lastPressedTime = 0f;
                lastReleasedTime = 0f;
                tapCount = 0;
                events = new OnKeyEvent[(int)EInputType.Count];
            }
        }

        private readonly Dictionary<string, AxisEventValue> _axisEvents = new Dictionary<string, AxisEventValue>();
        private readonly Dictionary<KeyCode, KeyEventValue> _keyEvents = new Dictionary<KeyCode, KeyEventValue>();

        private const float TAP_THRESHOLD = 0.2f;
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

        private void Update()
        {
            foreach (var kv in _axisEvents)
            {
                float value = Input.GetAxis(kv.Key);
                kv.Value.events?.Invoke(value);
            }

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

                    EModifier modifier = GetModifier();
                    eventValue.events[(int)EInputType.Pressed]?.Invoke(modifier);
                }
                else
                {
                    bool isHolding = Input.GetKey(keyCode);
                    if (isHolding)
                    {
                        EModifier modifier = GetModifier();
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

                        EModifier modifier = GetModifier();
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

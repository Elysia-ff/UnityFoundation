using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Inputs
{
    public class InputManager : MonoBehaviour
    {
        public delegate void OnAxisEvent(float value);
        public delegate void OnKeyEvent();

        private class AxisValue
        {
            public OnAxisEvent events;
        }

        private class EventValue
        {
            public float lastPressedTime;
            public float lastReleasedTime;
            public int tapCount;
            public OnKeyEvent[] events;

            public EventValue()
            {
                lastPressedTime = 0f;
                lastReleasedTime = 0f;
                tapCount = 0;
                events = new OnKeyEvent[(int)EInputType.Count];
            }
        }

        private readonly Dictionary<string, AxisValue> _axisEvents = new Dictionary<string, AxisValue>();
        private readonly Dictionary<KeyCode, EventValue> _keyEvents = new Dictionary<KeyCode, EventValue>();

        private const float TAP_THRESHOLD = 0.2f;
        private const float DOUBLE_TAP_THRESHOLD = 0.2f;

        public void BindAxis(string axis, OnAxisEvent onAxisEvent)
        {
            if (!_axisEvents.TryGetValue(axis, out AxisValue value))
            {
                value = new AxisValue();
                _axisEvents.Add(axis, value);
            }

            value.events += onAxisEvent;
        }

        public void UnbindAxis(string axis, OnAxisEvent onAxisEvent)
        {
            if (!_axisEvents.TryGetValue(axis, out AxisValue value))
            {
                return;
            }

            value.events -= onAxisEvent;
        }

        public void BindKey(KeyCode key, EInputType inputType, OnKeyEvent onKeyEvent)
        {
            if (!_keyEvents.TryGetValue(key, out EventValue value))
            {
                value = new EventValue();
                _keyEvents.Add(key, value);
            }

            value.events[(int)inputType] += onKeyEvent;
        }

        public void UnbindKey(KeyCode key, EInputType inputType, OnKeyEvent onKeyEvent)
        {
            if (!_keyEvents.TryGetValue(key, out EventValue value))
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
                EventValue eventValue = kv.Value;

                if (Input.GetKeyDown(keyCode))
                {
                    eventValue.lastPressedTime = Time.time;
                    if (eventValue.lastPressedTime - eventValue.lastReleasedTime is <= 0f or > DOUBLE_TAP_THRESHOLD)
                    {
                        eventValue.tapCount = 0;
                    }

                    eventValue.events[(int)EInputType.Pressed]?.Invoke();
                }
                else
                {
                    bool isHolding = Input.GetKey(keyCode);
                    if (isHolding)
                    {
                        eventValue.events[(int)EInputType.Holding]?.Invoke();
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

                        eventValue.events[(int)EInputType.Released]?.Invoke();

                        switch (eventValue.tapCount)
                        {
                            case 0:
                                break;

                            case 1:
                                eventValue.events[(int)EInputType.SingleTap]?.Invoke();
                                break;

                            case 2:
                                eventValue.events[(int)EInputType.DoubleTap]?.Invoke();
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
    }
}

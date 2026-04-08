using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public class FPSCounter : MonoBehaviour
    {
        private int _fps;
        private string _displayFPS;
        private Double _updateDisplayTime;

        private GUIStyle _guiStyle;

        private int _fontSize = 50;
        public int FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                _shouldUpdate = true;
            }
        }

        private Vector2 _position = new Vector2(20, 20);
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                _shouldUpdate = true;
            }
        }

        public Color BackgroundColor { get; set; } = Color.black;

        private Queue<int> _captureWindow = new Queue<int>(DEFAULT_CAPTURE_WINDOW_SIZE);
        private int _captureWindowSize = DEFAULT_CAPTURE_WINDOW_SIZE;
        public int CaptureWindowSize
        {
            get => _captureWindowSize;
            set
            {
                _captureWindowSize = value;
                _captureWindow = new Queue<int>(_captureWindowSize);
            }
        }

        public float UpdateDisplayInterval { get; set; } = 0.1f;

        private bool _shouldUpdate = true;
        private Rect _rect;

        private const int DEFAULT_CAPTURE_WINDOW_SIZE = 5;

        private void OnEnable()
        {
            _fps = 0;
            _displayFPS = string.Empty;
            _updateDisplayTime = Time.unscaledTimeAsDouble + UpdateDisplayInterval;
            _captureWindow.Clear();
        }

        private void Update()
        {
            if (_captureWindow.Count >= CaptureWindowSize)
            {
                _fps -= _captureWindow.Dequeue();
            }

            int fps = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            _fps += fps;
            _captureWindow.Enqueue(fps);

            if (Time.unscaledTimeAsDouble >= _updateDisplayTime)
            {
                _displayFPS = $"{(float)_fps / _captureWindow.Count:F1}";
                _updateDisplayTime = Time.unscaledTimeAsDouble + UpdateDisplayInterval;
            }
        }

        private void OnGUI()
        {
            if (_shouldUpdate)
            {
                if (_guiStyle == null)
                {
                    _guiStyle = new GUIStyle(GUI.skin.label);
                    _guiStyle.normal.background = Texture2D.whiteTexture;
                }

                _guiStyle.fontSize = FontSize;

                Vector2 size = _guiStyle.CalcSize(new GUIContent("00.0"));
                _rect = new Rect(Position.x, Position.y, size.x, size.y);

                _shouldUpdate = false;
            }

            GUI.backgroundColor = BackgroundColor;
            GUI.Label(_rect, _displayFPS, _guiStyle);
        }
    }
}

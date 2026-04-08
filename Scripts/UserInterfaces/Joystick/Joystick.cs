using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Elysia
{
    [RequireComponent(typeof(RectTransform))]
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] protected RectTransform _pointer;
        [SerializeField] private float _minDistance;
        [SerializeField] protected float _maxDistance;
        [SerializeField] private float _minValueThreshold;
        [SerializeField] private float _maxValueThreshold;
        [SerializeField] private int _unitAngleCount;

        // these variables are automatically set in Awake
        private float _sqrMinDistance;
        private float _sqrMaxDistance;

        private float _unitAngleInRadians;
        //

        protected RectTransform _rectTransform;
        private InputAction _pointAction;

        private const float MAX_ANGLE_IN_RADIANS = 2f * Mathf.PI;

        public bool IsDragging { get; private set; }
        public Vector2 InputVector { get; private set; }
        public float Value { get; private set; }

        public event System.Action OnPointerPressed;
        public event System.Action OnPointerReleased;

        protected virtual void Awake()
        {
            OnPropertyChanged();

            _rectTransform = (RectTransform)transform;

            if (_pointAction == null)
            {
                _pointAction = App.Input.UIActions.Point;
            }
        }

        protected virtual void OnDisable()
        {
            if (IsDragging)
            {
                IsDragging = false;
                ResetPointer();
                OnPointerReleased?.Invoke();
            }
        }

        public void SetPointAction(InputAction pointAction)
        {
            _pointAction = pointAction;
        }

        private void MovePointerTo(Vector2 mousePosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, mousePosition, App.Scene.UI.Camera, out Vector2 pos))
            {
                Vector2 dir = pos.normalized;
                float sqrMagnitude = pos.sqrMagnitude;
                if (sqrMagnitude < _sqrMinDistance)
                {
                    pos = Vector2.zero;
                }
                else if (sqrMagnitude > _sqrMaxDistance)
                {
                    pos = dir * _maxDistance;
                }

                float magnitude = pos.magnitude;
                if (_unitAngleCount > 0)
                {
                    float angleInRadians = Mathf.Acos(Vector2.Dot(Vector2.right, dir));
                    if (Vector2.Dot(Vector2.up, dir) < 0)
                    {
                        angleInRadians = -angleInRadians + MAX_ANGLE_IN_RADIANS;
                    }

                    int angleIdx = (int)(angleInRadians / _unitAngleInRadians + 0.5f);
                    float snappedAngleInRadians = _unitAngleInRadians * angleIdx;

                    pos = new Vector2(Mathf.Cos(snappedAngleInRadians), Mathf.Sin(snappedAngleInRadians)) * magnitude;
                }

                _pointer.localPosition = pos;
                InputVector = pos.normalized;
                Value = _minValueThreshold != _maxValueThreshold ? Mathf.Clamp01((magnitude - _minValueThreshold) / (_maxValueThreshold - _minValueThreshold)) : 1f;
            }
        }

        private void ResetPointer()
        {
            _pointer.localPosition = Vector2.zero;
            InputVector = Vector2.zero;
            Value = 0;
        }

        #region OnPointer Interfaces

        public void OnPointerDown(PointerEventData eventData)
        {
            IsDragging = true;
            MovePointerTo(_pointAction.ReadValue<Vector2>());
            OnPointerPressed?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsDragging = false;
            ResetPointer();
            OnPointerReleased?.Invoke();
        }

        #endregion

        public void OnPropertyChanged()
        {
            _sqrMinDistance = _minDistance * _minDistance;
            _sqrMaxDistance = _maxDistance * _maxDistance;
            _unitAngleInRadians = MAX_ANGLE_IN_RADIANS / _unitAngleCount;
        }

        private void Update()
        {
            if (!IsDragging)
            {
                return;
            }

            MovePointerTo(_pointAction.ReadValue<Vector2>());

#if UNITY_EDITOR
            for (int i = 0; i < _unitAngleCount; i++)
            {
                float snappedAngleInRadians = _unitAngleInRadians * i;
                Vector3 dir = new Vector3(Mathf.Cos(snappedAngleInRadians), Mathf.Sin(snappedAngleInRadians), 0);
                Vector3 start = _rectTransform.position + _rectTransform.TransformVector(dir * _minDistance);
                Vector3 end = _rectTransform.position + _rectTransform.TransformVector(dir * _maxDistance);

                Debug.DrawLine(start, end, Color.green);
            }

            Debug.DrawLine(_rectTransform.position, _pointer.position, Color.red);
#endif
        }
    }
}

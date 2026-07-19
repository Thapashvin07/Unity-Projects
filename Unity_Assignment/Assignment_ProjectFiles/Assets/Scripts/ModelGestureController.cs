using UnityEngine;
using UnityEngine.EventSystems;

    public class ModelGestureController : MonoBehaviour
    {
        [SerializeField] private RectTransform inputArea;
        [SerializeField] private Camera uiCamera;
        [SerializeField] private float rotationSpeed = 0.25f;
        [SerializeField] private bool allowVerticalRotation = true;
        [SerializeField] private float verticalClampAngle = 60f;
        [SerializeField] private float minScale = 0.3f;
        [SerializeField] private float maxScale = 3f;
        [SerializeField] private float pinchSpeed = 0.01f;
        [SerializeField] private float doubleTapMaxDelay = 0.3f;
        [SerializeField] private float resetLerpDuration = 0.35f;

        private Transform _target;
        private Vector3 _defaultScale;
        private float currentScaleMultiplier;
        private Quaternion _defaultRotation;
        private bool isDragging;
        private Vector2 _lastDragPos;
        private float pitch;
        private bool isPinching;
        private float _lastPinchDistance;
        private float _lastTapTime = -1f;
        private bool isResetting;
        private float resetElapsed;
        private Quaternion resetFromRotation;
        private Vector3 resetFromScale;

        public void SetTarget(Transform target)
        {
            _target = target;
            _defaultScale = target.localScale;
            _defaultRotation = target.localRotation;
            pitch = 0f;
            currentScaleMultiplier = 1f;
        }

        private void Update()
        {
            if (_target == null) return;

            if (isResetting)
            {
                UpdateResetLerp();
                return; 
            }

        #if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouseInput();
        #else
            HandleTouchInput();
        #endif
        }

        private bool IsInsideInputArea(Vector2 screenPos)
        {
            if (inputArea == null) return true;
            return RectTransformUtility.RectangleContainsScreenPoint(inputArea, screenPos, uiCamera);
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsInsideInputArea(Input.mousePosition)) return;                
                isDragging = true;
                _lastDragPos = Input.mousePosition;
                CheckDoubleTap();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
            else if (isDragging && Input.GetMouseButton(0))
            {
                Vector2 currentPos = Input.mousePosition;
                Vector2 delta = currentPos - _lastDragPos;
                ApplyRotationDelta(delta);
                _lastDragPos = currentPos;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.0001f && IsInsideInputArea(Input.mousePosition))
            {
                ApplyScaleDelta(scroll * 100f);
            }
        }
        private void HandleTouchInput()
        {
            if (Input.touchCount == 1)
            {
                isPinching = false;
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (!IsInsideInputArea(touch.position)) return;
                        isDragging = true;
                        _lastDragPos = touch.position;
                        CheckDoubleTap();
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (isDragging)
                        {
                            Vector2 delta = touch.position - _lastDragPos;
                            ApplyRotationDelta(delta);
                            _lastDragPos = touch.position;
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        isDragging = false;
                        break;
                }
            }
            else if (Input.touchCount == 2)
            {
                isDragging = false;
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);
                Vector2 midpoint = (t0.position + t1.position) * 0.5f;
                if (!isPinching && !IsInsideInputArea(midpoint)) return;

                float currentDistance = Vector2.Distance(t0.position, t1.position);

                if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began || !isPinching)
                {
                    isPinching = true;
                    _lastPinchDistance = currentDistance;
                }
                else
                {
                    float delta = currentDistance - _lastPinchDistance;
                    ApplyScaleDelta(delta);
                    _lastPinchDistance = currentDistance;
                }
            }
            else
            {
                isDragging = false;
                isPinching = false;
            }
        }

        private void ApplyRotationDelta(Vector2 pixelDelta)
        {
            float yRot = pixelDelta.x * rotationSpeed;
            _target.Rotate(Vector3.up, -yRot, Space.World);

            if (allowVerticalRotation)
            {
                float pitchDelta = pixelDelta.y * rotationSpeed;
                float newPitch = Mathf.Clamp(pitch + pitchDelta, -verticalClampAngle, verticalClampAngle);
                float appliedDelta = newPitch - pitch;
                pitch = newPitch;
                _target.Rotate(Vector3.right, appliedDelta, Space.Self);
            }
        }

        private void ApplyScaleDelta(float pixelDelta)
        {
            float multiplierDelta = pixelDelta * pinchSpeed;
            currentScaleMultiplier = Mathf.Clamp(currentScaleMultiplier + multiplierDelta, minScale, maxScale);

            _target.localScale = new Vector3(
                _defaultScale.x * currentScaleMultiplier,
                _defaultScale.y * currentScaleMultiplier,
                _defaultScale.z * currentScaleMultiplier
            );
        }

        private void CheckDoubleTap()
        {
            float now = Time.time;
            if (_lastTapTime > 0f && (now - _lastTapTime) <= doubleTapMaxDelay)
            {
                StartReset();
                _lastTapTime = -1f;
            }
            else
            {
                _lastTapTime = now;
            }
        }

        private void StartReset()
        {
            
            isResetting = true;
            resetElapsed = 0f;
            resetFromRotation = _target.localRotation;
            resetFromScale = _target.localScale;
            pitch = 0f;
            isDragging = false;
            isPinching = false;
        }

        public void UpdateResetLerp()
        {
            resetElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(resetElapsed / resetLerpDuration);
            float smoothT = t * t * (3f - 2f * t);

            _target.localRotation = Quaternion.Slerp(resetFromRotation, _defaultRotation, smoothT);
            _target.localScale = Vector3.Lerp(resetFromScale, _defaultScale, smoothT);

            if (t >= 1f)
                
                isResetting = false;
        }
}
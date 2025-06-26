using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Framework.Components
{
    /**
     * Movement component is responsible for handling all the movement related logic of the game object.
     * It supports both physics-based and kinematic movement.
     *
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Mention date of latest modification"
     * Modified By: "Mention name of latest modifier"
     */

    public class MovementComponent : BaseComponent
    {
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private bool usePhysics = false;
        [SerializeField] private bool useFriction = false;
        [SerializeField] private bool debug = false;

        public Rigidbody RigidBody
        {
            get => rigidBody;
            set => rigidBody = value;
        }

        [SerializeField] public float maxWalkSpeed = 5f;
        [SerializeField] public float acceleration = 10f;
        [SerializeField] public float deceleration = 10f;
        public Vector3 Velocity { get; set; } = Vector3.zero; // Consider removing if unused
        [SerializeField] public float jumpForce = 5f;
        [SerializeField] public float groundFriction = 2f;
        [SerializeField] public float maxFallSpeed = 20f;
        [SerializeField] public float gravity = 9.81f;

        private Vector3 _currentVelocity = Vector3.zero;
        private Vector3 _velocitySmooth = Vector3.zero; // For SmoothDamp
        private Vector3 _currentAcceleration = Vector3.zero;
        private Vector3 _currentDirection = Vector3.zero;
        private Vector3 _inputDirection = Vector3.zero;
        private bool _isGrounded = true;
        private bool _isJumping = false;
        private bool _isFalling = false;
        private float _lastGroundedTime = 0f;

        private const float RAYCAST_OFFSET = 0.1f;
        private const float GROUND_CHECK_BUFFER = 0.1f;

        public bool IsGrounded() => _isGrounded;

        public bool IsJumping() => _isJumping;

        public bool IsFalling() => _isFalling;

        public Vector3 GetCurrentVelocity() => usePhysics ? rigidBody.linearVelocity : _currentVelocity;

        public Vector3 GetCurrentAcceleration() => _currentAcceleration;

        public Vector3 GetCurrentDirection() => _currentDirection;

        public void SetInputDirection(Vector3 direction)
        {
            _inputDirection = direction.normalized;
        }

        public void Move(Vector3 direction)
        {
            SetInputDirection(direction);
        }

        public void Jump()
        {
            if (!_isGrounded || Time.time < _lastGroundedTime + GROUND_CHECK_BUFFER) return;
            _isJumping = true;
            if (!usePhysics)
            {
                _currentVelocity.y = jumpForce;
            }
        }

        private void PhysicsUpdate(float fixedDeltaTime)
        {
            // Ground check
            var origin = rigidBody.position + Vector3.up * RAYCAST_OFFSET;
            var checkDistance = groundCheckDistance + RAYCAST_OFFSET;
            bool wasGrounded = _isGrounded;
            _isGrounded = Physics.Raycast(origin, Vector3.down, checkDistance, groundLayerMask, QueryTriggerInteraction.Ignore);
            if (_isGrounded) _lastGroundedTime = Time.time;

            // Desired movement
            var desiredVelocity = _inputDirection * maxWalkSpeed;
            var currentVelocity = rigidBody.linearVelocity; // Unity recommends `velocity`, not `linearVelocity` here
            var velocityChange = desiredVelocity - currentVelocity;
            velocityChange.y = 0; // Don't affect vertical velocity here

            // Use raw delta instead of normalized
            float accel = _inputDirection.sqrMagnitude > 0.01f ? acceleration : deceleration;
            if (useFriction && _isGrounded && _inputDirection.sqrMagnitude < 0.01f)
            {
                accel += groundFriction;
            }

            Vector3 force = velocityChange * accel * rigidBody.mass;
            rigidBody.AddForce(force, ForceMode.Force);

            // Jumping logic
            if (_isJumping && _isGrounded && !wasGrounded)
            {
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                _isJumping = false;
            }

            // Stick-to-ground force (optional but helps)
            if (_isGrounded && rigidBody.linearVelocity.y < 0f)
            {
                rigidBody.AddForce(Vector3.down * 5f, ForceMode.Acceleration);
            }

            // Cap fall speed
            if (rigidBody.linearVelocity.y < -maxFallSpeed)
            {
                var vel = rigidBody.linearVelocity;
                vel.y = -maxFallSpeed;
                rigidBody.linearVelocity = vel;
            }

            // Update states
            var horizontalVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
            _currentAcceleration = velocityChange / fixedDeltaTime;
            _currentDirection = horizontalVelocity.sqrMagnitude > 0.0001f ? horizontalVelocity.normalized : Vector3.zero;
            _isFalling = !_isGrounded && rigidBody.linearVelocity.y < -0.1f;

            // Debug (optional)
            if (debug) Debug.Log($"[Physics] InputDir: {_inputDirection}, Force: {force.magnitude}, Vel: {rigidBody.linearVelocity}");
        }

        private void KinematicUpdate(float deltaTime)
        {
            // Ground check with debug ray
            var origin = transform.position + Vector3.up * RAYCAST_OFFSET;
            var checkDistance = groundCheckDistance + RAYCAST_OFFSET;
            bool wasGrounded = _isGrounded;
            _isGrounded = Physics.Raycast(origin, Vector3.down, checkDistance, groundLayerMask, QueryTriggerInteraction.Ignore);
            if (_isGrounded) _lastGroundedTime = Time.time;

            // Smoothly approach desired velocity
            var desiredVelocity = _inputDirection * maxWalkSpeed;
            _currentVelocity = Vector3.SmoothDamp(_currentVelocity, desiredVelocity, ref _velocitySmooth, 0.1f);

            // Apply deceleration when no input
            if (_inputDirection.sqrMagnitude < 0.01f)
            {
                float totalDecel = deceleration + (useFriction && _isGrounded ? groundFriction : 0f);
                _currentVelocity = Vector3.MoveTowards(_currentVelocity, Vector3.zero, totalDecel * deltaTime);
            }

            // Apply gravity and cap fall speed
            if (!_isGrounded)
            {
                _currentVelocity.y -= gravity * deltaTime;
                _currentVelocity.y = Mathf.Max(_currentVelocity.y, -maxFallSpeed);
                _isFalling = true;
            }
            else
            {
                _currentVelocity.y = 0f; // Snap to zero when grounded
                _isJumping = false; // Reset jumping
            }

            transform.position += _currentVelocity * deltaTime;

            _currentDirection = _currentVelocity.sqrMagnitude > 0.001f ? _currentVelocity.normalized : Vector3.zero;
            _currentAcceleration = (desiredVelocity - _currentVelocity) / deltaTime;

            if (debug) Debug.Log($"Kinematic Velocity: {_currentVelocity.magnitude}, Input: {_inputDirection.magnitude}, Grounded: {_isGrounded}");
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (!usePhysics)
            {
                KinematicUpdate(deltaTime);
            }
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            base.FixedTick(fixedDeltaTime);
            if (usePhysics && rigidBody)
            {
                if (!rigidBody) Debug.LogError("Rigidbody is null, you absolute legend!", this);
                PhysicsUpdate(fixedDeltaTime);
            }
        }

        protected override void Start()
        {
            base.Start();
            if (usePhysics && !rigidBody)
            {
                Debug.LogError("Rigidbody missing on MovementComponent!", this);
                return;
            }
            if (rigidBody)
            {
                rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rigidBody.angularDamping = 5f;
            }
        }
    }
}
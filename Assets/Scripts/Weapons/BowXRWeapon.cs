using Constants;
using UnityEngine;
using Weapons.Projectiles;

namespace Weapons
{
    /// <summary>
    /// Bow weapon implementation for VR. 
    /// Supports draw mechanics based on hand distance and fires projectiles with pooling.
    /// VR-optimized with minimal allocations.
    /// </summary>
    public class BowXRWeapon : XRWeaponBase
    {
        #region Fields

        [Header("Bow Settings")]
        [SerializeField] private ProjectileData arrowData;
        [SerializeField] private Transform arrowSpawnPoint;
        [SerializeField] private Transform stringAttachPoint;
        
        [Header("Draw Settings")]
        [SerializeField] private float maxDrawDistance = 0.6f;
        [SerializeField] private float drawTime = GameConstants.DefaultBowDrawTime;
        [SerializeField] private float minDrawStrength = GameConstants.MinBowDrawStrength;
        
        [Header("Arrow Pool")]
        [SerializeField] private int arrowPoolSize = 10;

        private float _currentDrawStrength;
        private bool _isDrawing;
        private Transform _drawHand;
        
        // Object pool for arrows - VR performance optimization
        private Projectile[] _arrowPool;
        private int _currentArrowIndex;

        // Cached references
        private Transform _transform;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current draw strength (0 to 1).
        /// </summary>
        public float DrawStrength => _currentDrawStrength;

        /// <summary>
        /// Returns true if the bow is currently being drawn.
        /// </summary>
        public bool IsDrawing => _isDrawing;

        /// <summary>
        /// Returns true if the bow can fire (sufficient draw strength).
        /// </summary>
        public bool CanFire => _currentDrawStrength >= minDrawStrength && CanAttack;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _transform = transform;
            InitializeArrowPool();
        }

        private void Update()
        {
            if (!IsHeld) return;

            if (_isDrawing)
            {
                UpdateDrawStrength();
            }
        }

        #endregion

        #region Arrow Pool

        /// <summary>
        /// Initializes the arrow object pool for VR performance.
        /// Pre-instantiates arrows to avoid runtime allocations.
        /// </summary>
        private void InitializeArrowPool()
        {
            if (arrowData == null || arrowData.Prefab == null) return;

            _arrowPool = new Projectile[arrowPoolSize];
            
            for (int i = 0; i < arrowPoolSize; i++)
            {
                var arrowGO = Instantiate(arrowData.Prefab, _transform);
                arrowGO.SetActive(false);
                
                var projectile = arrowGO.GetComponent<Projectile>();
                if (projectile == null)
                    projectile = arrowGO.AddComponent<Projectile>();
                
                projectile.Initialize(arrowData);
                _arrowPool[i] = projectile;
            }
        }

        /// <summary>
        /// Gets the next available arrow from the pool.
        /// </summary>
        /// <returns>An arrow projectile or null if pool is exhausted.</returns>
        private Projectile GetArrowFromPool()
        {
            if (_arrowPool == null || _arrowPool.Length == 0) return null;

            // Find an inactive arrow
            for (int i = 0; i < arrowPoolSize; i++)
            {
                _currentArrowIndex = (_currentArrowIndex + 1) % arrowPoolSize;
                var arrow = _arrowPool[_currentArrowIndex];
                
                if (arrow != null && !arrow.gameObject.activeInHierarchy)
                {
                    return arrow;
                }
            }

            // All arrows in use - reuse oldest (forced recycle for VR performance)
            _currentArrowIndex = (_currentArrowIndex + 1) % arrowPoolSize;
            var oldestArrow = _arrowPool[_currentArrowIndex];
            oldestArrow?.OnDespawn();
            return oldestArrow;
        }

        #endregion

        #region Draw Mechanics

        /// <summary>
        /// Starts drawing the bow. Call when player grabs the string.
        /// </summary>
        /// <param name="drawHandTransform">Transform of the hand pulling the string.</param>
        public void StartDraw(Transform drawHandTransform)
        {
            if (!IsHeld) return;
            
            _isDrawing = true;
            _drawHand = drawHandTransform;
            _currentDrawStrength = 0f;
        }

        /// <summary>
        /// Stops drawing and fires if sufficient draw strength.
        /// </summary>
        public void ReleaseDraw()
        {
            if (_isDrawing && CanFire)
            {
                FireArrow();
            }
            
            _isDrawing = false;
            _drawHand = null;
            _currentDrawStrength = 0f;
        }

        /// <summary>
        /// Updates draw strength based on hand distance from bow.
        /// </summary>
        private void UpdateDrawStrength()
        {
            if (_drawHand == null || stringAttachPoint == null)
            {
                // Fallback: time-based draw
                _currentDrawStrength = Mathf.Clamp01(_currentDrawStrength + Time.deltaTime / drawTime);
                return;
            }

            // Calculate draw based on distance between bow and draw hand
            var distance = Vector3.Distance(stringAttachPoint.position, _drawHand.position);
            _currentDrawStrength = Mathf.Clamp01(distance / maxDrawDistance);
        }

        #endregion

        #region Fire

        /// <summary>
        /// Fires an arrow with current draw strength.
        /// </summary>
        private void FireArrow()
        {
            if (arrowSpawnPoint == null) return;

            var arrow = GetArrowFromPool();
            if (arrow == null) return;

            // Unparent arrow from bow
            arrow.transform.SetParent(null);
            
            // Launch with draw strength as damage multiplier
            var launchDir = arrowSpawnPoint.forward;
            arrow.Launch(arrowSpawnPoint.position, launchDir, _currentDrawStrength);

            // Haptic feedback
            TriggerHapticFeedback();
        }

        #endregion

        #region Actions

        /// <summary>
        /// Primary action - increases draw if holding, can be used for quick shots.
        /// </summary>
        public override void PrimaryAction()
        {
            if (!_isDrawing)
            {
                // Start a time-based draw without tracking hand
                _isDrawing = true;
                _drawHand = null;
            }
            
            _currentDrawStrength = Mathf.Clamp01(_currentDrawStrength + Time.deltaTime / drawTime);
        }

        /// <summary>
        /// Secondary action - releases the arrow.
        /// </summary>
        public override void SecondaryAction()
        {
            ReleaseDraw();
        }

        #endregion

        #region Lifecycle

        protected override void OnEquipped()
        {
            base.OnEquipped();
            _currentDrawStrength = 0f;
            _isDrawing = false;
        }

        protected override void OnUnequipped()
        {
            base.OnUnequipped();
            _currentDrawStrength = 0f;
            _isDrawing = false;
            _drawHand = null;
        }

        #endregion
    }
}

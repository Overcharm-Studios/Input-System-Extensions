using UnityEngine;
using UnityEngine.InputSystem;

namespace Overcharm.InputSystemExtensions
{
    /// <summary>
    /// Derive from <see cref="InputActionConverter"/> to implement a <see cref="Behaviour"/> that relays (and/or converts) <see cref="InputAction"/> inputs to a target destination.
    /// </summary>
    public abstract class InputActionConverter : MonoBehaviour
    {
        // # ================================ # //
        // # ------ Fields + Properties ----- # //
        // # ================================ # //

        [Tooltip("The target InputAction that will have its inputs read from.")]
        [SerializeField] private InputActionReference _origin;

        [Tooltip("The destination InputAction that will have its inputs written to.")]
        [SerializeField] private InputActionReference _destination;

        public InputAction InputOrigin => _origin;

        public InputAction InputDestination => _destination;

        // # ================================ # //
        // # --------- Unity Methods -------- # //
        // # ================================ # //

        #region Awake
        protected void Awake()
        {
            // Warnings -
            if (_origin == null) { Debug.LogError($"{nameof(InputActionConverter)} is missing a {nameof(InputActionReference)} reference.", this); }
            if (_destination == null) { Debug.LogError($"{nameof(InputActionConverter)} is missing a {nameof(InputActionReference)} reference.", this); }

            // Enable -
            if (_origin.asset != null) _origin.asset.Enable();
            if (_destination.asset != null) _destination.asset.Enable();
        }
        #endregion

        #region OnEnable
        protected void OnEnable()
        {
            if (_origin == null) return;
            if (_destination == null) return;

            _origin.action.performed += OnConvertToPerformed;
        }
        #endregion

        #region OnDisable
        protected void OnDisable()
        {
            if (_origin == null) return;
            if (_destination == null) return;

            _origin.action.performed -= OnConvertToPerformed;
        }
        #endregion

        // # ================================ # //
        // # -------- Private Methods ------- # //
        // # ================================ # //

        private void OnConvertToPerformed(InputAction.CallbackContext obj)
        {
            if (!OnValidateOrigin(obj.action, out var message_1)) 
            { 
                Debug.LogError($"The '{nameof(InputOrigin)}' {nameof(InputAction)} is not considerd valid! {message_1} \nDisabling self..."); 
                enabled = false;
                return; 
            }

            if (!OnValidateDestination(obj.action, out var message_2))
            {
                Debug.LogError($"The '{nameof(InputDestination)}' {nameof(InputAction)} is not considerd valid! {message_2} \nDisabling self...");
                enabled = false;
                return;
            }

            OnInputPerformed(obj);
        }

        // # ================================ # //
        // # ---------- Abstracts ----------- # //
        // # ================================ # //

        // Properties -

        public abstract string ImplementationMessage { get; } 

        // Operation -

        #region OnInputPerformed
        /// <summary>
        /// A callback that is invoked whenever the '<see cref="InputOrigin"/>' <see cref="InputAction"/> performs an input.
        /// <para/>
        /// Use this callback to modify, convert, and relay inputs into the '<see cref="InputDestination"/>'.
        /// </summary>
        /// <param name="context"></param>
        public abstract void OnInputPerformed(InputAction.CallbackContext context);
        #endregion

        // Validation -

        #region OnValidateOrigin
        /// <summary>
        /// Validation callback for '<see cref="InputOrigin"/>'.
        /// <para/>
        /// Use this callback to ensure that it is the reference is of the expected control type.
        /// </summary>
        /// <returns> True if the <paramref name="origin"/> is considered valid. </returns>
        public virtual bool OnValidateOrigin(InputAction origin, out string message) { message = default; return true; }
        #endregion

        #region OnValidateDestination
        /// <summary>
        /// Validation callback for '<see cref="InputOrigin"/>'.
        /// <para/>
        /// Use this callback to ensure that it is the reference is of the expected control type.
        /// </summary>
        /// <returns> True if the <paramref name="destination"/> is considered valid. </returns>
        public virtual bool OnValidateDestination(InputAction destination, out string message) { message = default; return true; }
        #endregion
    }
}


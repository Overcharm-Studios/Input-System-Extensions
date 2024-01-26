using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Controls;
using System;
using System.Collections.ObjectModel;

namespace Overcharm.InputSystemExtensions
{
    /// <summary>
    /// A special <see cref="Behaviour"/> that simulates an <see cref="InputDevice"/> along with its <see cref="InputControl"/>s.
    /// <para/>
    /// Derive from <see cref="InputDeviceSimulator{T}"/> to implement a simulated <see cref="InputDevice"/> proxy.
    /// </summary>
    public abstract class InputDeviceSimulator : MonoBehaviour
    {
        // # ================================ # //
        // # ------ Fields + Properties ----- # //
        // # ================================ # //

        #region Actions
        /// <summary>
        /// The input actions to be simulated on the <see cref="InputDevice"/>.
        /// </summary>
        private readonly List<InputAction> _actions = new();

        /// <inheritdoc cref="_actions"/>
        public ReadOnlyCollection<InputAction> Actions => _actions.AsReadOnly();
        #endregion

        // # ================================ # //
        // # --------- Unity Methods -------- # //
        // # ================================ # //

        protected void Awake() { }

        protected void OnEnable()
        {
            InputSystem.onAfterUpdate += OnInputSystemUpdate;

            _actions.Enable();
        }

        protected void OnDisable()
        {
            InputSystem.onAfterUpdate -= OnInputSystemUpdate;

            _actions.Disable();
        }

        protected void OnDestroy() { }

        // # ================================ # //
        // # ---------- Abstracts ----------- # //
        // # ================================ # //

        #region TargetDeviceType
        /// <summary>
        /// The type of device to be generated and simulated by the simulator.
        /// </summary>
        public abstract Type TargetDeviceType { get; }
        #endregion

        #region TargetDeviceName
        /// <summary>
        /// The name for the simulated device to have.
        /// </summary>
        public virtual string TargetDeviceName { get => TargetDeviceType?.Name; }
        #endregion

        #region Device
        /// <summary>
        /// The device instance associated with the <see cref="InputDeviceSimulator{T}"/>.
        /// </summary>
        public abstract InputDevice Device { get; }
        #endregion

        //

        protected abstract void OnInputSystemUpdate();

        // # ================================ # //
        // # ------ Protected Methods ------- # //
        // # ================================ # //

        #region RegisterInputAction
        /// <summary>
        /// Registers 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected bool TryRegisterInputAction(InputAction action)
        {
#if (DEBUG)
            if (action == null) return false;
#endif

            _actions.Add(action);
            action.Enable();

            return true;
        }
        #endregion

        #region UnregisterInputAction
        protected bool TryUnregisterInputAction(InputAction action)
        {
#if (DEBUG)
            if (action == null) return false;
#endif

            action.Disable();
            return _actions.Remove(action);
        }
        #endregion

        #region IsRegistered
        protected bool IsRegistered(InputAction action)
        {
            return _actions.Contains(action);
        }
        #endregion

        // Utility

        #region TryPressButton
        protected static bool TryPressButton(InputAction action, bool isPressed)
        {
            if (action == null) return false;

            if (isPressed)
            {
                action.WriteValue<float>(1, true);
                return true;
            }
            else
            {
                action.WriteValue<float>(0, true);
                return false;
            }
        }
        #endregion

        #region TryPressButton
        protected static bool TrySetFloatValue(InputAction action, float value)
        {
            if (action == null) return false;

            action.WriteValue<float>(value, true);
            return true;
        }
        #endregion
    }
}
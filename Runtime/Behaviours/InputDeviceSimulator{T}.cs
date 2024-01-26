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
    /// <inheritdoc cref="InputDeviceSimulator"/>
    /// <typeparam name="T"> The type of input device to simulate. </typeparam>
    public abstract class InputDeviceSimulator<T> : InputDeviceSimulator where T : InputDevice
    {
        // # ================================ # //
        // # ------ Fields + Properties ----- # //
        // # ================================ # //

        #region DeviceTyped
        /// <inheritdoc cref="InputDeviceSimulator.Device"/>
        private T _deviceTyped;

        /// <inheritdoc cref="_deviceTyped"/>
        public T DeviceTyped => _deviceTyped;
        #endregion

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

        new protected void Awake()
        {
            base.Awake();
            SetupInputDevice();
        }

        new protected void OnDestroy()
        {
            base.OnDestroy();
            DestroyInputDevice();
        }

        // # ================================ # //
        // # ------- Implementations -------- # //
        // # ================================ # //

        public override InputDevice Device => _deviceTyped;

        // # ================================ # //
        // # ---------- Abstracts ----------- # //
        // # ================================ # //

        public virtual string DeviceName { get => nameof(T); }

        // Methods 

        protected abstract void OnDeviceSetup();

        protected abstract void OnDeviceDestroy();

        // # ================================ # //
        // # -------- Private Methods ------- # //
        // # ================================ # //

        #region SetupInputDevice
        private void SetupInputDevice()
        {
#if (DEBUG)
            if (_deviceTyped != null) throw new Exception($"The {nameof(InputDevice)} has already been setup.");
#endif

            // Setup Device —
            _deviceTyped = InputSystem.AddDevice<T>(DeviceName);

            if (_deviceTyped == null)
            {
                Debug.LogError($"Failed to create {nameof(T)}.", this);
                return;
            }

            OnDeviceSetup();
        }
        #endregion

        #region DestroyInputDevice
        private void DestroyInputDevice()
        {
            if (_deviceTyped == null) return; // Null device — do nothing.

            OnDeviceDestroy();

            InputSystem.RemoveDevice(_deviceTyped);
        }
        #endregion
    }
}
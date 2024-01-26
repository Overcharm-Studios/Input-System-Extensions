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

namespace Overcharm.InputSystemExtensions
{
    /// <summary>
    /// An enum that represents all of the available <see cref="InputDeviceType"/>.
    /// <para/>
    /// Serialization integrity is NOT gauranteed.
    /// </summary>
    public enum InputDeviceType
    {
        Gamepad,
        HID,
        Joystick,
        Keyboard,
        Pointer,
        Sensor,
        TrackedDevice,
    }

    public static class InputDeviceTypesUtils
    {
        #region ToDeviceType
        /// <summary>
        /// Returns the <see cref="Type"/> that correlates with the provided <see cref="InputDeviceType"/>.
        /// </summary>
        public static Type ToDeviceType(this InputDeviceType type)
        {
            return type switch
            {
                InputDeviceType.Gamepad => typeof(Gamepad),
                InputDeviceType.HID => typeof(HID),
                InputDeviceType.Joystick => typeof(Joystick),
                InputDeviceType.Keyboard => typeof(Keyboard),
                InputDeviceType.Pointer => typeof(Pointer),
                InputDeviceType.Sensor => typeof(Sensor),
                InputDeviceType.TrackedDevice => typeof(TrackedDevice),
                _ => null,
            };
        }
        #endregion

        #region AddDeviceToSystem
        /// <summary>
        /// Creates a new <see cref="InputDevice"/> that correlates with the provided <see cref="InputDeviceType"/> and adds it into the <see cref="InputSystem"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        /// The newly created <see cref="InputDevice"/>.
        /// </returns>
        public static InputDevice AddDeviceToSystem(this InputDeviceType type)
        {
            switch (type)
            {
                case InputDeviceType.Gamepad: return InputSystem.AddDevice<Gamepad>();

                case InputDeviceType.HID: return InputSystem.AddDevice<HID>();

                case InputDeviceType.Joystick: return InputSystem.AddDevice<Joystick>();

                case InputDeviceType.Keyboard: return InputSystem.AddDevice<Keyboard>();

                case InputDeviceType.Pointer: return InputSystem.AddDevice<Pointer>();

                case InputDeviceType.Sensor: return InputSystem.AddDevice<Sensor>();

                case InputDeviceType.TrackedDevice: return InputSystem.AddDevice<TrackedDevice>();

                default: return null;
            }

            /*
            InputDevice inputDevice = null;

            if (type == InputDeviceTypes.Gamepad)
            {
                inputDevice = InputSystem.AddDevice<Keyboard>();
                return inputDevice;
            }

            if (type == InputDeviceTypes.HID)
            {
                inputDevice = InputSystem.AddDevice<HID>();
                return inputDevice;
            }

            if (type == InputDeviceTypes.Joystick)
            {
                inputDevice = InputSystem.AddDevice<Joystick>();
                return inputDevice;
            }

            if (type == InputDeviceTypes.Keyboard)
            {
                inputDevice = InputSystem.AddDevice<Keyboard>();
                return inputDevice;
            }

            if (type == InputDeviceTypes.Pointer)
            {
                inputDevice = InputSystem.AddDevice<Pointer>();
                return inputDevice;
            }

            if (type == InputDeviceTypes.Sensor)
            {
                inputDevice = InputSystem.AddDevice<Sensor>();
                return inputDevice;
            }

            if (type == InputDeviceTypes.TrackedDevice)
            {
                inputDevice = InputSystem.AddDevice<TrackedDevice>();
                return inputDevice;
            }
            */
        }
        #endregion
    }
}



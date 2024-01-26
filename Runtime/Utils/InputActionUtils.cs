using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System;

namespace Overcharm.InputSystemExtensions 
{
    public static class InputActionUtils
    {
        // # ================================ # //
        // # ------- Direct Extensions ------ # //
        // # ================================ # //

        // Relaying -

        #region RelayValue
        /// <summary>
        /// Reads the value form the provided <paramref name="target"/> and writes that value into the provided <paramref name="destination"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="destination"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void RelayValue<T>(this InputAction target, InputAction destination) where T : struct
        {
#if (DEBUG)
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
#endif

            // Read value —
            T value = target.ReadValue<T>();

            // Write value —
            destination.WriteValue(value);
        }
        #endregion

        #region RelayValueAsObject
        /// <returns></returns>
        /// <inheritdoc cref="RelayValue"/>
        /// <inheritdoc cref="InputAction.ReadValueAsObject"/>
        public static void RelayValueAsObject(this InputAction target, InputAction destination)
        {
#if (DEBUG)
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
#endif

            // Action Type Mismatch —
            if (target.type != destination.type)
                throw new Exception($"The given {nameof(target)}'s {nameof(InputAction.type)} ('{target.type}') does not match the {nameof(destination)}'s {nameof(InputAction.type)} ('{destination.type}').");


            // Control Type Mismatch —
            InputControlType targetControlType = target.GetControlTypeAsEnum();
            InputControlType destinationControlType = destination.GetControlTypeAsEnum();

            if (targetControlType != destinationControlType)
                throw new Exception($"The given {nameof(target)} {nameof(InputAction)}'s {nameof(InputControlType)} does not match the {nameof(destination)} {nameof(InputAction)}'s {nameof(InputControlType)}.");


            // Read —
            object value = target.ReadValueAsObject();

            // Write —
            destination.WriteValueAsObject(value);
        }
        #endregion

        // Writing -

        #region WriteValue
        /// <inheritdoc cref="WriteValue{T}(InputAction, T, bool)"/>
        public static void WriteValue<T>(this InputAction target, T input) where T : struct
        {
#if (DEBUG)
            if (target == null) throw new ArgumentNullException(nameof(target));
#endif

            target.WriteValue(input, false);
        }

        /// <summary>
        /// Writes the provided <paramref name="input"/> into the <see cref="InputAction"/>'s <see cref="InputControl"/>s and its associated <see cref="InputDevice"/>s.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void WriteValue<T>(this InputAction target, T input, bool enableWarnings) where T : struct
        {
#if (DEBUG)
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (enableWarnings && target.controls.Count == 0) { Debug.LogWarning($"No active {nameof(InputControl)}s exist for the {nameof(InputAction)} ('{target.name}'). Usually this means that no {nameof(InputDevice)} that can trigger this action is registered."); }
#endif

            // Write value into all 'to' InputControls, effectively relaying the input —
            foreach (var item in target.controls)
            {
                using (StateEvent.From(item.device, out InputEventPtr eventPtr))
                {
                    item.WriteValueIntoEvent(input, eventPtr);
                    InputSystem.QueueEvent(eventPtr);
                }
            }
        }
        #endregion

        #region WriteValueAsObject
        /// <inheritdoc cref="WriteValueAsObject(InputAction, object, bool)"/>
        public static void WriteValueAsObject(this InputAction target, object input)
        {
#if (DEBUG)
            if (target == null) throw new ArgumentNullException(nameof(target));
#endif

            target.WriteValueAsObject(input, false);
        }

        /// <summary>
        /// Same as <see cref="WriteValue{T}(InputAction, T)"/> but writes the value without having to know the value type of the <see cref="InputAction"/>.
        /// </summary>
        /// <returns></returns>
        /// <inheritdoc cref="WriteValue"/>
        /// <inheritdoc cref="InputAction.ReadValueAsObject"/>
        public static void WriteValueAsObject(this InputAction target, object input, bool enableWarnings)
        {
#if (DEBUG)
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (enableWarnings && target.controls.Count == 0) { Debug.LogWarning($"No active {nameof(InputControl)}s exist for the {nameof(InputAction)} ('{target.name}'). Usually this means that no {nameof(InputDevice)} that can trigger this action is registered."); }
#endif

            // Write value into all 'to' InputControls, effectively relaying the input —
            foreach (var item in target.controls)
            {
                using (StateEvent.From(item.device, out InputEventPtr eventPtr))
                {
                    item.WriteValueFromObjectIntoEvent(eventPtr, input);
                    InputSystem.QueueEvent(eventPtr);
                }
            }
        }
        #endregion

        // Utility -

        #region GetAcceptableDevicesFrom
        /// <inheritdoc cref="GetAcceptableDevicesFrom_Internal"/>
        public static string[] GetAcceptableDevicesFrom(this InputAction target)
        {
#if (DEBUG)
            if (target == null) throw new ArgumentNullException(nameof(target));
#endif

            return target.GetAcceptableDevicesFrom_Internal(false, false);
        }

        /// <inheritdoc cref="GetAcceptableDevicesFrom_Internal"/>
        public static string[] GetAcceptableDevicesFrom(this InputAction target, bool ignoreDuplicates, bool baseLayoutsOnly)
        {
#if (DEBUG)
            if (target == null) throw new ArgumentNullException(nameof(target));
#endif

            return target.GetAcceptableDevicesFrom_Internal(ignoreDuplicates, baseLayoutsOnly);
        }

        /// <summary>
        /// Gets all acceptable <see cref="InputDevice"/> layouts that can be used to trigger the given <see cref="InputAction"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private static string[] GetAcceptableDevicesFrom_Internal(this InputAction target, bool ignoreDuplicates, bool baseLayoutsOnly)
        {
#if (DEBUG)
            if (target == null) throw new ArgumentNullException(nameof(target));
#endif

            List<string> outDevices = new();


            foreach (var binding in target.bindings)
            {
                // Try get device layout
                if (binding.TryGetDevice(out string layout))
                {
                    // Get Base Layout?
                    if (baseLayoutsOnly) layout = InputSystem.GetNameOfBaseLayout(layout);

                    // Ignore Duplicates Layouts?
                    if (ignoreDuplicates)
                    {
                        if (!outDevices.Contains(layout)) outDevices.Add(layout);
                    }
                    else
                    {
                        outDevices.Add(layout);
                    }
                }
            }


            return outDevices.ToArray();
        }
        #endregion

        // # ================================ # //
        // # ----- Collection Extensions ---- # //
        // # ================================ # //

        #region Enable
        /// <summary>
        /// Enables all of the provided <see cref="InputAction"/>s.
        /// </summary>
        /// <inheritdoc cref="Toggle"/>
        public static void Enable(this IReadOnlyList<InputAction> actions)
        {
#if (DEBUG)
            if (actions == null) throw new ArgumentNullException(nameof(actions));
#endif

            actions.Toggle(true);
        }
        #endregion

        #region Disable
        /// <summary>
        /// Disables all of the provided <see cref="InputAction"/>s.
        /// </summary>
        /// <inheritdoc cref="Toggle"/>
        public static void Disable(this IReadOnlyList<InputAction> actions)
        {
#if (DEBUG)
            if (actions == null) throw new ArgumentNullException(nameof(actions));
#endif

            actions.Toggle(true);
        }
        #endregion

        #region Toggle
        /// <summary>
        /// Toggles all of the provided <see cref="InputAction"/>s to the given enabled state.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Toggle(this IReadOnlyList<InputAction> actions, bool onOrOff)
        {
#if (DEBUG)
            if (actions == null) throw new ArgumentNullException(nameof(actions));
#endif

            InputAction action;
            int length = actions.Count;
            for (int i = 0; i < length; i++)
            {
                action = actions[i];

                if (action == null) continue;

                if (onOrOff) { action.Enable(); } else { action.Disable(); }
            }
        }
        #endregion
    }
}
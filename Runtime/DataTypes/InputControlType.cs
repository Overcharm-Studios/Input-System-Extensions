using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Overcharm.InputSystemExtensions
{
    /// <summary>
    /// Defines the various possible '<see cref="InputAction.expectedControlType"/>s' — but as an enum instead of a string.
    /// </summary>
    public enum InputControlType
    {
        /// <summary>
        /// The default <see cref="InputControlType"/> for when the <see cref="InputControlType"/> cannot be determined.
        /// </summary>
        [Tooltip("The Expected Control Type could not be determined.")]
        Unknown = int.MinValue,
        Any = 0,

        Analog = 100,
        Axis = 200,
        Bone = 300,
        Button = 400,

        Digital = 500,
        Double = 600,
        Dpad = 700,
        Eyes = 800,

        Integer = 900,
        Quaternion = 1000,

        Touch = 1100,
        Stick = 1200,

        Vector2 = 1300,
        Vector3 = 1400,
    }
}
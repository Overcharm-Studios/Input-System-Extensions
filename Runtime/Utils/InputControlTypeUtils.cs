using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Overcharm.InputSystemExtensions
{
    /// <summary>
    /// A set fo utilities for working with <see cref="InputControlType"/>s.
    /// </summary>
    public static class InputControlTypeUtils
    {
        // Utility -

        #region ConvertToInputType
        /// <summary>
        /// Converts the provided <see cref="string"/> to its corresponding <see cref="InputControlType"/>.
        /// </summary>
        public static InputControlType ConvertStringToInputType(in string value)
        {
            return value switch
            {
                null => InputControlType.Unknown,
                "Any" => InputControlType.Any,

                "Analog" => InputControlType.Analog,
                "Axis" => InputControlType.Axis,
                "Bone" => InputControlType.Bone,
                "Button" => InputControlType.Button,

                "Digital" => InputControlType.Digital,
                "Double" => InputControlType.Double,
                "Dpad" => InputControlType.Dpad,
                "Eyes" => InputControlType.Eyes,

                "Integer" => InputControlType.Integer,
                "Quaternion" => InputControlType.Quaternion,

                "Stick" => InputControlType.Stick,
                "Touch" => InputControlType.Touch,

                "Vector2" => InputControlType.Vector2,
                "Vector3" => InputControlType.Vector3,

                _ => InputControlType.Unknown,
            };
        }
        #endregion

        // Extension -

        #region ExpectedControlTypeAsEnum
        /// <summary>
        /// Returns the 'expected control type' of the <see cref="InputAction"/> as a '<see cref="InputControlType"/>' enum.
        /// </summary>
        public static InputControlType GetControlTypeAsEnum(this InputAction action)
        {
            return ConvertStringToInputType(action.expectedControlType);
        }
        #endregion

        #region ConvertInputTypeToString
        /// <summary>
        /// Converts an <see cref="InputControlType"/> to its corresponding string.
        /// </summary>
        public static string ConvertToString(this InputControlType value)
        {
            return value switch
            {
                InputControlType.Unknown => null,
                InputControlType.Any => "Any",

                InputControlType.Analog => "Analog",
                InputControlType.Axis => "Axis",
                InputControlType.Bone => "Bone",
                InputControlType.Button => "Button",

                InputControlType.Digital => "Digital",
                InputControlType.Double => "Double",
                InputControlType.Dpad => "Dpad",
                InputControlType.Eyes => "Eyes",

                InputControlType.Integer => "Integer",
                InputControlType.Quaternion => "Quaternion",

                InputControlType.Stick => "Stick",
                InputControlType.Touch => "Touch",

                InputControlType.Vector2 => "Vector2",
                InputControlType.Vector3 => "Vector3",

                _ => null,
            };
        }
        #endregion
    }
}
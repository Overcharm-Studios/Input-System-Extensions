using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

namespace Overcharm.InputSystemExtensions.Editor
{
    [CustomEditor(typeof(InputDeviceSimulator), true)]
    public sealed class InputDeviceSimulator_Inspector : UnityEditor.Editor
    {
        public InputDeviceSimulator Self => target as InputDeviceSimulator;

        public override void OnInspectorGUI()
        {
            string message = "";
            message += $"An {nameof(InputDeviceSimulator)} generates a simulated {nameof(InputDevice)} of the type '{Self.TargetDeviceType}'. ";
            message += $"Inputs will be relayed to the simulated device according to the simulator's final implementation. ";
            message += $"Any and all inputs written into the '{Self.TargetDeviceType}' will be detected by the applicable {nameof(InputAction)}s that read from the particular simulated device.";

            EditorGUILayout.HelpBox(message, MessageType.Info);

            base.OnInspectorGUI();
        }
    }
}
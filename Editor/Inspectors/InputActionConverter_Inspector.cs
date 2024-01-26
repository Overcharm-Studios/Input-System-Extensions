using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

/*
namespace Overcharm.InputSystemExtensions.Editor
{
    [CustomEditor(typeof(InputActionConverter), true)]
    public sealed class InputActionConverter_Inspector : UnityEditor.Editor
    {
        public InputActionConverter Self => target as InputActionConverter;

        public override void OnInspectorGUI()
        {
            string message = "";
            message += $"An {nameof(InputActionConverter)} converts, modifies, and/or relays inputs from an 'Origin' {nameof(InputAction)} to a 'Destination' {nameof(InputAction)}. ";

            EditorGUILayout.HelpBox(message, MessageType.Info);
            EditorGUILayout.HelpBox(Self.ImplementationMessage, MessageType.Info);

            base.OnInspectorGUI();
        }
    }
}
*/
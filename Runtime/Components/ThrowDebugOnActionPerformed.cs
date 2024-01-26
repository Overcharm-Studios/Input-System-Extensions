using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using System;

namespace Overcharm.InputSystemExtensions
{
    public sealed class ThrowDebugOnActionPerformed : MonoBehaviour
    {
        // # ================================ # //
        // # ------ Fields + Properties ----- # //
        // # ================================ # //

        [SerializeField] private InputActionReference _inputAction;

        // # ================================ # //
        // # -------- Public Methods -------- # //
        // # ================================ # //

        private void OnEnable()
        {
            _inputAction.action.performed += Action_performed;
        }

        private void OnDisable()
        {
            _inputAction.action.performed -= Action_performed;
        }


        private void Action_performed(InputAction.CallbackContext obj)
        {
            Debug.Log($"{nameof(InputAction)} ('{obj.action.name}') was just performed. Its value was {obj.ReadValueAsObject()?.ToString()}");
        }
    }
}


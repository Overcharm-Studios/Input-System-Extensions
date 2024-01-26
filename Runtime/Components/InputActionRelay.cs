using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using System;

namespace Overcharm.InputSystemExtensions
{
    /// <summary>
    /// A simple <see cref="InputActionConverter"/> that relays the inputs from the 'origin' <see cref="InputAction"/> to the 'destination' <see cref="InputAction"/>.
    /// </summary>
    public sealed class InputActionRelay : InputActionConverter
    {
        // # ================================ # //
        // # -------- Public Methods -------- # //
        // # ================================ # //

        public override string ImplementationMessage => $"Inputs will be relayed 1-to-1 without any modifications made to them.";

        public override void OnInputPerformed(InputAction.CallbackContext context)
        {
            /// This ain't fast GC-alloc-free code... 
            /// But it gets the job done and is generally reliable.
            /// If you are looking for something more efficient,
            /// it is recommended that you implement your own converter.

            try 
            { 
                InputOrigin.RelayValueAsObject(InputDestination);
            }
            catch (Exception exception) 
            { 
                Debug.LogWarning(exception.Message);
            }
        }

        
    }
}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Controls;
using System;
using Overcharm.Engine.UnityExtended;
using Overcharm.Engine.Utils;
using XRController = UnityEngine.InputSystem.XR.XRController;
using Overcharm.InputSystemExtensions;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using Overcharm.Engine;
using Overcharm.Engine.LowLevel;
using Overcharm.Engine.Methamatics;

namespace Overcharm.InputSystemExtensions
{
    public struct MouseInputs 
    {
        public MouseInputs(Mouse mouse) 
        {
            _mouse = null;
            _positionCurrent = default;
            _positionPrevious = default;

            SetMouse(mouse);
        }

        private Mouse _mouse;
        private Vector2 _positionPrevious;
        private Vector2 _positionCurrent;

        public Mouse Mouse => _mouse;
        public Vector2 PositionCurrent => _positionCurrent;
        public Vector2 PositionPrevious => _positionPrevious;
        public Vector2 PositionDelta => PositionCurrent - PositionPrevious;

        public void SetMouse(Mouse mouse) 
        {
            _mouse = mouse;

            if (_mouse == null) return;

            Vector2 mousePos = _mouse.position.ReadValue();
            _positionPrevious = mousePos;
            _positionCurrent = mousePos;
        }

        public void Update()
        {
            if (_mouse == null) return;

            Vector2 mousePos = _mouse.position.ReadValue();
            _positionPrevious = _positionCurrent;
            _positionCurrent = _mouse.position.ReadValue();
        }
    }


    // Note to self - XRController may not be all the layout info needed by an Oculous Quest Controller.
    // Note to Self - XCController in the editor may have a different meaning than in code, and therefore may not cover all cases when instantiated as an input device (it may only contain position / rotation data). 

    // Okay, so our suspicions were correct — 'XR Controller' does not contain all the InputControls we need.
    // It seems we're going to need to procedurally create an InputDevice layout that accomodates the needs of all of the given input actions...
    // Wow.
    // That's gonna be tough.
    // Well, no time like the present!
    // GET ON IT.

    /// <summary>
    /// A component that creates and maintains an artificial <see cref="XRController"/>. 
    /// </summary>
    public class XRControllerSimulator : InputDeviceSimulator<XRSimulatedController>
    {
        public enum XRHandType 
        { 
            LeftHand = -1,
            RightHand = 1,
        }

        // # ================================ # //
        // # ------ Fields + Properties ----- # //
        // # ================================ # //

        [SerializeField] private XRHandType _handType = XRHandType.RightHand;

        [Header(" - Tracking - ")]
        [SerializeField] private InputActionReference _trackingPositionControl;
        [SerializeField] private InputActionReference _trackingRotationControl;

        [Header(" - Joysticks - ")]
        [SerializeField] private InputActionReference _joystickControl;

        [Header(" - Triggers and Grips - ")]
        [SerializeField] private InputActionReference _triggerControl;
        [SerializeField] private InputActionReference _triggerValueControl;
        [SerializeField] private InputActionReference _gripControl;
        [SerializeField] private InputActionReference _gripValueControl;

        [Header(" - Main Buttons - ")]
        [SerializeField] private InputActionReference _buttonLowerControl;
        [SerializeField] private InputActionReference _buttonUpperControl;

        [Space]
        [Header(" - Simulation - ")]

        [SerializeField] private Vector2 _mouseAreaCenter = new(0, 0);
        [SerializeField] private Vector2 _mouseAreaExtents = new(10, 5);
        private Vector3 _simXRPosition;
        private Vector3 _simXRRotation;

        // Unserialized

        private MouseInputs _mouseInputs;
        private Vector2 _mousePositionPrevious;

        // # ================================ # //
        // # ---------- Properties ---------- # //
        // # ================================ # //

        public XRHandType HandType => _handType;

        public InputAction InputTrackingPosition => _trackingPositionControl;
        public InputAction InputTrackingRotation => _trackingRotationControl;

        public InputAction InputJoystick => _joystickControl;

        public InputAction InputTrigger => _triggerControl;
        public InputAction InputTriggerValue => _triggerValueControl;
        public InputAction InputGrip => _gripControl;
        public InputAction InputGripValue => _gripValueControl;

        public InputAction InputButtonLower => _buttonLowerControl;
        public InputAction InputButtonUpper => _buttonUpperControl;

        // # ================================ # //
        // # --------- Unity Methods -------- # //
        // # ================================ # //

        new protected void Awake()
        {
            base.Awake();

            _mouseInputs = new MouseInputs(Mouse.current);
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) EasyDraw.DrawShpere(_simXRPosition, Quaternion.Euler(_simXRRotation), Vector3.one * 1f, Color.white);
        }

        // # ================================ # //
        // # -------- Implementations ------- # //
        // # ================================ # //

        public override string DeviceName => $"{base.DeviceName} - {HandType}";

        protected override void OnDeviceSetup()
        {
            // Setup Action List —
            TryRegisterInputAction(InputTrackingPosition);
            TryRegisterInputAction(InputTrackingRotation);
            TryRegisterInputAction(InputJoystick);
            TryRegisterInputAction(InputTrigger);
            TryRegisterInputAction(InputGrip);

            // Set Device Usage —
            if (_handType == XRHandType.LeftHand) InputSystem.SetDeviceUsage(Device, CommonUsages.LeftHand);
            if (_handType == XRHandType.RightHand) InputSystem.SetDeviceUsage(Device, CommonUsages.RightHand);
        }

        protected override void OnInputSystemUpdate()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            if (keyboard != null) ApplyKeyboardInputs(keyboard);
            if (mouse != null) ApplyMouseInputs(mouse);


            // Drawing —


            EasyDraw.DrawShpere(_simXRPosition, Quaternion.Euler(_simXRRotation), Vector3.one * 1f, Color.white);
        }

        protected override void OnDeviceDestroy()
        {

        }

        // # ================================ # //
        // # -------- Private Methods ------- # //
        // # ================================ # //

        // Input Methods

        #region ApplyKeyboardInputs
        private void ApplyKeyboardInputs(Keyboard keyboard)
        {
            if (keyboard == null) return;
            if (InputJoystick == null) return;


            Vector2Int inputs = default;


            // WASD Keys —
            if (keyboard.wKey.isPressed) inputs.y += 1;
            if (keyboard.sKey.isPressed) inputs.y -= 1;

            if (keyboard.dKey.isPressed) inputs.x += 1;
            if (keyboard.aKey.isPressed) inputs.x -= 1;

            // Arrow Keys —
            if (keyboard.upArrowKey.isPressed) inputs.y += 1;
            if (keyboard.downArrowKey.isPressed) inputs.y -= 1;

            if (keyboard.rightArrowKey.isPressed) inputs.x += 1;
            if (keyboard.leftArrowKey.isPressed) inputs.x -= 1;

  
            // Write Input —
            InputJoystick.WriteValue(((Vector2)inputs).normalized);
        }
        #endregion

        #region ApplyMouseInputs
        private void ApplyMouseInputs(Mouse mouse)
        {
            if (mouse == null) return;


            // Trigger and Grip Inputs —
            TryPressButton(InputTrigger, mouse.leftButton.isPressed);
            TrySetFloatValue(InputTriggerValue, mouse.leftButton.isPressed ? 1 : 0);

            TryPressButton(InputGrip, mouse.rightButton.isPressed);
            TrySetFloatValue(InputGripValue, mouse.rightButton.isPressed ? 1 : 0);


            // Button Inputs —
            TryPressButton(InputButtonUpper, mouse.forwardButton.isPressed);
            TryPressButton(InputButtonLower, mouse.backButton.isPressed);


            // Position Tracking —
            if (InputTrackingPosition != null)
            {
                /// Apparently, Mouse.delta.ReadValue() only returns proper values
                /// when the method is called within an InputAction callback...
                /// This makes it basically impossible to get the Mouse.delta
                /// values outside of using InputActions... Go figure... -o-

                
                Vector2 mousePos = mouse.position.ReadValue();


                // Convert Screen Mouse Pos to -1 to 1 centered value —
                Vector2 viewportPos01 = new
                (
                    Meth.RemapValue(mousePos.x, 0, Screen.width,  -1, 1), 
                    Meth.RemapValue(mousePos.y, 0, Screen.height, -1, 1)
                );


                // Calculate Position in region —
                Vector2 finalHorPosition = (Vector2)transform.position + _mouseAreaCenter + (_mouseAreaExtents * viewportPos01);
                _simXRPosition.x = finalHorPosition.x;
                _simXRPosition.z = finalHorPosition.y;

                _simXRPosition.y += mouse.scroll.y.ReadValue(); // Control cursor hover height


                Vector3 input = default; // Final Position
                input.x = _simXRPosition.x;
                input.y = _simXRPosition.y;
                input.z = _simXRPosition.z;

                InputTrackingPosition.WriteValue(input);
            }


            // Rotation Tracking —
            if (InputTrackingRotation != null)
            {
                Quaternion input = default;
                input = Quaternion.Euler(_simXRRotation);

                InputTrackingRotation.WriteValue(input);
            }
        }
        #endregion

        // # ================================ # //
        // # -------- Public Methods -------- # //
        // # ================================ # //
    }
}


*/
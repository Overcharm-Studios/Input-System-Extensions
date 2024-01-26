using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using System;

namespace Overcharm.InputSystemExtensions
{
    /// <summary>
    /// These utils are in a currently indev state. 
    /// </summary>
    public static class InputLayoutBuilderUtils
    {
        #region BuildLayoutFrom
        /// <inheritdoc cref="BuildLayoutFrom_Internal"/>
        public static InputControlLayout BuildLayoutFrom(IReadOnlyList<InputAction> actions)
        {
            return BuildLayoutFrom_Internal(actions);
        }

        /// <inheritdoc cref="BuildLayoutFrom_Internal"/>
        public static InputControlLayout BuildLayoutFrom(IReadOnlyList<InputAction> actions, string name)
        {
            return BuildLayoutFrom_Internal(actions, name);
        }

        /// <inheritdoc cref="BuildLayoutFrom_Internal"/>
        public static InputControlLayout BuildLayoutFrom(IReadOnlyList<InputAction> actions, string name, string format)
        {
            return BuildLayoutFrom_Internal(actions, name, format);
        }

        /// <summary>
        /// Builds an <see cref="InputControlLayout"/> that supports all of the given <paramref name="actions"/> and can trigger all of their <see cref="InputBinding"/>s.
        /// </summary>
        /// <param name="actions"></param>
        private static InputControlLayout BuildLayoutFrom_Internal(IReadOnlyList<InputAction> actions, string name = null, string format = null)
        {
#if (DEBUG)
            if (actions == null) throw new ArgumentNullException(nameof(actions));
#endif



            if (name == null) name = $"Procedural Device: {Time.realtimeSinceStartup}";


            // Main Build
            var builder = new InputControlLayout.Builder();
            builder.WithName(name);
            builder.WithDisplayName("Procedural Device");
            builder.AddControlsFromAction(actions);
            builder.WithSizeInBytes((actions.Count + 1) * 16);


            // Format?
            if (format != null) builder.WithFormat(format);


            return builder.Build();
        }
        #endregion

        #region AddControlsFromAction
        /// <summary>
        /// Using the given <paramref name="builder"/> — generates all of the controls for the provided <paramref name="actions"/> and writes them into the <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="actions"></param>
        public static void AddControlsFromAction(this InputControlLayout.Builder builder, IReadOnlyList<InputAction> actions)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (actions == null) throw new ArgumentNullException(nameof(actions));

            foreach (var item in actions)
            {
                if (item == null) continue;

                builder.AddControlsFromAction(item);
            }
        }

        /// <summary>
        /// Using the given <paramref name="builder"/> — generates all of the controls for the provided <paramref name="action"/> and writes them into the <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        public static void AddControlsFromAction(this InputControlLayout.Builder builder, InputAction action)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (action == null) throw new ArgumentNullException(nameof(action));

            // TO DO:
            // - Procedurally calculate the necessary '.SizeInBits' and '.Byte/Bit Offsets' for a given binding / control.
            // - Consider creating a helper utility that takes an InputBinding and calculates associated metadata from it (SizeInBits, Offsets, etc).

            //InputControlLayout.Builder.ControlBuilder mainControlBuilder;
            //InputControlLayout.Builder.ControlBuilder subControlBuilder;

            string control;
            var bindings = action.bindings;
            foreach (var binding in bindings)
            {
                /// Skip 'Part-of-Composite' bindings as these are
                /// handled whenever composites-bindings are detected
                /// rather than linearly during binding iteration.
                if (binding.isPartOfComposite) continue;

                Debug.Log("Regular Controls: " + binding.effectivePath);

                if (binding.TryGetControl(out control))
                {
                    var mainControlBuilder = builder.AddControl(control);
                    Debug.Log("MADE IT: " + control);

                    // Specify Usages —
                    #region Specify Usages (Only supports 1 usage at this time)
                    if (binding.TryGetUsage(out string usages))
                    {
                        mainControlBuilder.WithUsages(new string[] { usages });
                    }
                    #endregion


                    // Specify Layout —
                    #region Specify Layout
                    mainControlBuilder.WithLayout(action.expectedControlType);
                    #endregion


                    // Specify Size —
                    #region Specify Size
                    if (binding.isComposite)
                    {
                        if (bindings.TryGetCompositePartsOf(binding, out var compositeParts))
                        {
                            /// This code uses a 'bit offset' that is four times the normal for memory layout safety. 
                            /// Normally one bit is fine for 'Button' type controls — I think...
                            mainControlBuilder.WithSizeInBits((uint)compositeParts.Length * 4);
                        }
                    }
                    #endregion


                    // Specify Bit and Byte Offsets —
                    #region Specify Offsets
                    /// Current bit offsets are not properly calculated.
                    /// For the time being, we are fudging them with insane 
                    /// offset values to heopfully accommodate any and all
                    /// memory layouts we encounter.
                    mainControlBuilder.WithByteOffset(8);
                    //mainControlBuilder.WithBitOffset(2);
                    #endregion
                }


                /// Handle all of the Composite Controls 
                /// of the current binding, here — if any.
                #region Add Composite Controls
                if (binding.isComposite)
                {
                    Debug.Log("THIS IS A COMPOSITE!");

                    if (bindings.TryGetCompositePartsOf(binding, out var compositeParts))
                    {
                        uint bitOffset = 0;

                        foreach (var compositeBinding in compositeParts)
                        {
                            bitOffset++;

                            if (compositeBinding.TryGetControl(out control))
                            {
                                Debug.Log("Added: " + control);

                                // Create Composite Control —
                                var compositeBuilder = builder.AddControl(control);

                                // Specify Layout —
                                compositeBuilder.WithLayout(action.expectedControlType);

                                // Assign Bye and Bit offsets —
                                compositeBuilder.WithByteOffset(unchecked((uint)-1)).WithBitOffset(bitOffset);
                            }
                        }
                    }
                }
                #endregion
            }



            #region OLD CODE
            /*
            foreach (var binding in action.bindings)
            {
            // Composite Control —
            if (binding.isComposite)
            {
                if (binding.TryGetControl(out var mainControl, out var subControl))
                {
                    mainControlBuilder = builder.AddControl(mainControl);


                    // Has Usages —
                    if (binding.TryGetUsages(out string[] usages))
                    {
                        mainControlBuilder.WithUsages(usages);
                    }


                    // Specify Layout —
                    mainControlBuilder.WithLayout(action.expectedControlType);


                    // Shift Bytes
                    mainControlBuilder.WithByteOffset(0);
                    mainControlBuilder.WithBitOffset(0);

                }
            }

            // Composite Part —
            else if (binding.isPartOfComposite)
            {
                if (binding.TryGetControl(out var mainControl, out var subControl))
                {
                    mainControlBuilder = builder.AddControl(mainControl);


                    // Has Usages —
                    if (binding.TryGetUsages(out string[] usages))
                    {
                        mainControlBuilder.WithUsages(usages);
                    }


                    // Specify Layout —
                    //mainControlBuilder.WithLayout(action.expectedControlType);


                    // Shift Bytes
                    mainControlBuilder.WithByteOffset(unchecked((uint)-1));
                    mainControlBuilder.WithBitOffset(0);



                            
                }
            }

            // Non-Composite Control —
            else
            {
                // Has Binding —
                if (binding.TryGetControl(out var mainControl, out var subControl))
                {
                    mainControlBuilder = builder.AddControl(mainControl);


                    // Has Usages —
                    if (binding.TryGetUsages(out string[] usages))
                    {
                        mainControlBuilder.WithUsages(usages);
                    }


                    // Specify Layout —
                    mainControlBuilder.WithLayout(action.expectedControlType);


                    // Shift Bytes
                    mainControlBuilder.WithByteOffset(0);
                    mainControlBuilder.WithBitOffset(0);

                }
            }
            */
            #endregion


            #region For-Reference Help
            /**
            var builder = new InputControlLayout.Builder()
                .WithName("MyDevice")
                .WithFormat("MDEV"); 
             
            // Button Control
            builder.AddControl("button1")
                .WithLayout("Button")
                .WithByteOffset(0)
                .WithBitOffset(0);

            // Button Control
            builder.AddControl("button2")
                .WithLayout("Button")
                .WithByteOffset(0)
                .WithBitOffset(1);


            // Note how the subcontrols have a negative byte offset — that's interesting!
            // The composite control 'dpad' needs to know the exact size (in bits) when it comes to the contained composite controls.
            // Weird how the methods are expecting uints... 


            // Composite:
            builder.AddControl("dpad")
                .WithLayout("Dpad")
                .WithByteOffset(0)
                .WithBitOffset(2)
                .WithSizeInBits(4);

            builder.AddControl("dpad/up")
                .WithByteOffset(unchecked((uint)-1))
                .WithBitOffset(2);

            builder.AddControl("dpad/down")
                .WithByteOffset(unchecked((uint)-1))
                .WithBitOffset(3);

            builder.AddControl("dpad/left")
                .WithByteOffset(unchecked((uint)-1))
                .WithBitOffset(4);

            builder.AddControl("dpad/right")
                .WithByteOffset(unchecked((uint)-1))
                .WithBitOffset(5);
            // Composite:



            // Stick Control
            builder.AddControl("stick")
                .WithLayout("Stick")
                .WithByteOffset(4)
                .WithFormat("VEC2");

            // Axis Control
            builder.AddControl("trigger")
                .WithLayout("Axis")
                .WithByteOffset(12)
                .WithFormat("BYTE");

            var layout = builder.Build();
            */
            #endregion
        }
        #endregion
    }
}
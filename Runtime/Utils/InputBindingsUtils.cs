using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Overcharm.InputSystemExtensions
{
    /// <summary>
    /// A set of utilities for working with <see cref="InputBinding"/>s.
    /// </summary>
    public static class InputBindingUtils
    {
        // # ================================ # //
        // # -------- Private Methods ------- # //
        // # ================================ # //

        #region TryGetDevice
        /// <remarks>
        /// If the <paramref name="binding"/> uses an overide path the overide path is used instead of the normal path.
        /// </remarks>
        /// <inheritdoc cref="TryGetDeviceFromPath(string, out string)"/>
        public static bool TryGetDevice(this InputBinding binding, out string layout)
        {
            return TryGetDeviceFromPath(binding.effectivePath, out layout);
            /*
            if (binding.overridePath != null)
            {
                return TryGetDeviceFromPath(binding.overridePath, out layout);
            }
            else
            {
                return TryGetDeviceFromPath(binding.path, out layout);
            }*/
        }
        #endregion

        #region TryGetControl
        /// <remarks>
        /// If the <paramref name="binding"/> uses an overide path the overide path is used instead of the normal path.
        /// </remarks>
        /// <inheritdoc cref="TryGetControlFromPath(string, out string, out string)"/>
        public static bool TryGetControl(this InputBinding binding, out string control)
        {
            return TryGetControlFromPath(binding.effectivePath, out control);
        }
        #endregion

        #region TryGetUsage
        /// <remarks>
        /// If the <paramref name="binding"/> uses an overide path the overide path is used instead of the normal path.
        /// </remarks>
        /// <inheritdoc cref="TryGetUsageFromPath(string, out string)"/>
        public static bool TryGetUsage(this InputBinding binding, out string usage)
        {
            return TryGetUsageFromPath(binding.effectivePath, out usage);
        }
        #endregion

        #region TryGetCompositePartsOf
        /// <summary>
        /// Out of the given set of <paramref name="bindings"/> — finds all <paramref name="compositeParts"/> associated with the provided <paramref name="binding"/>.
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="binding"></param>
        /// <param name="compositeParts"></param>
        /// <returns>
        /// True if any <paramref name="compositeParts"/> were found. 
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetCompositePartsOf(this IReadOnlyList<InputBinding> bindings, InputBinding binding, out InputBinding[] compositeParts)
        {
#if (DEBUG)
            if (bindings == null) throw new ArgumentNullException(nameof(bindings));
#endif

            List<InputBinding> parts = new();


            bool matchFound = false;
            for (int i = 0; i < bindings.Count; i++)
            {
                // Find Match —
                if (bindings[i] == binding)
                {
                    matchFound = true;
                    continue;
                }

                // Add Subsequent Part-Of-Composite Bindings —
                if (matchFound)
                {
                    if (!bindings[i].isPartOfComposite) break; /// Break on chain end.

                    parts.Add(bindings[i]);
                }
            }

            compositeParts = parts.ToArray();
            return (parts.Count > 0);
        }
        #endregion

        // # ================================ # //
        // # -------- Utility Methods ------- # //
        // # ================================ # //

        #region TryGetDeviceFromPath
        /// <summary>
        /// Searches through the given <paramref name="binding"/> and gets the device <paramref name="layout"/> specified within it.
        /// </summary>
        /// <returns>
        /// True if a <paramref name="layout"/> was specified in the <paramref name="binding"/>. Otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetDeviceFromPath(string binding, out string layout)
        {
#if (DEBUG)
            if (binding == null) throw new ArgumentNullException(nameof(binding));
#endif

            int start = binding.IndexOf("<");
            int end = binding.IndexOf(">");

            if (start == -1 || end == -1) { layout = null; return false; }

            layout = binding.Substring(start + 1, (end - start) - 1);
            return true;
        }
        #endregion

        #region TryGetControlFromPath
        /// <summary>
        /// Searches through the given <paramref name="binding"/> and gets the <paramref name="control"/> (and any <paramref name="subControl"/>) specified within it.
        /// </summary>
        /// <returns>
        /// True if at least the <paramref name="control"/> was specified in the <paramref name="binding"/>. Otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetControlFromPath(string binding, out string control)
        {
#if (DEBUG)
            if (binding == null) throw new ArgumentNullException(nameof(binding));
#endif

            int start = binding.IndexOf("/");

            if (start == -1) { control = null; return false; }

            control = binding.Substring(start + 1, (binding.Length - start) - 1);
            return true;
        }

        /// <summary>
        /// Searches through the given <paramref name="binding"/> and gets the <paramref name="controlSections"/> specified within it.
        /// <para/>
        /// A '<paramref name="controlSections"/>' contains the entire control (including the composite binding (if any) in the path).
        /// </summary>
        /// <returns>
        /// True if a <paramref name="controlSections"/> was specified in the <paramref name="binding"/>. Otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetControlFromPath(string binding, out string[] controlSections)
        {
#if (DEBUG)
            if (binding == null) throw new ArgumentNullException(nameof(binding));
#endif

            List<string> outControls = new();

            binding += "/"; // Append Terminating Forward Slash.
            List<int> indexes = IndexOfAll(binding, "/");

            for (int i = 0; i < indexes.Count - 1; i++)
            {
                int start = indexes[i];
                int end = indexes[i + 1];

                outControls.Add(binding.Substring(start + 1, (end - start) - 1));
            }

            controlSections = outControls.ToArray();
            return outControls.Count != 0;
        }
        #endregion

        #region TryGetUsageFromPath
        /// <summary>
        /// Searches through the given <paramref name="binding"/> and gets the <paramref name="usage"/> specified within it.
        /// </summary>
        /// <returns>
        /// True if a <paramref name="usage"/> was specified in the <paramref name="binding"/>. Otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetUsageFromPath(string binding, out string usage)
        {
#if (DEBUG)
            if (binding == null) throw new ArgumentNullException(nameof(binding));
#endif

            int start = binding.IndexOf("{");
            int end = binding.IndexOf("}");

            if (start == -1 || end == -1) { usage = null; return false; }

            usage = binding.Substring(start + 1, (end - start) - 1);
            return true;

            /*
            //var indexes = IndexOfAll(binding, "/");

            List<string> strings = new();

            var enumerator = binding.GetEnumerator();

            int start = binding.IndexOf("{");
            int end = binding.IndexOf("}");

            while (enumerator.MoveNext())
            {


                if (start != -1 && end != -1)
                {
                    strings.Add(binding.Substring(start + 1, (end - start) - 1));
                }
            }



            throw new NotImplementedException();
            return true;
            */
        }
        #endregion




        // Utils (TO DO: Move Util into engine)
        #region IndexOfAll
        /// <summary>
        /// Returns a list containing the indexes of all occurances of the given <paramref name="value"/> in the string.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<int> IndexOfAll(this string input, string value)
        {
#if (DEBUG)
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (value == null) throw new ArgumentNullException(nameof(value));
#endif

            List<int> indexes = new();

            int i = 0;
            while (i <= input.Length + 1)
            {
                i = input.IndexOf(value, i);

                if (i == -1) break;

                indexes.Add(i);

                i++;
            }

            return indexes;
        }
        #endregion

        // TO DO:
        // Add 'TryGetUsageFromPath' for getting the device's implied usages from the binding path.





        // # ================================ # //
        // # ------ Deprecated Methods ------ # //
        // # ================================ # //

        #region TryGetControlFromPath
        /*
        /// <summary>
        /// Searches through the given <paramref name="binding"/> and gets the <paramref name="mainControl"/> (and any <paramref name="subControl"/>) specified within it.
        /// </summary>
        /// <returns>
        /// True if at least the <paramref name="mainControl"/> was specified in the <paramref name="binding"/>. Otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetControlFromPath(string binding, out string mainControl, out string subControl)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));


            // Try get control —
            if (TryGetControlFromPath(binding, out string[] fullControl))
            {
                Debug.Log(fullControl.Length);

                // Regular Control —
                if (fullControl.Length == 1)
                {
                    mainControl = fullControl[0];
                    subControl = null;
                    return true;
                }

                // Composite Control Part —
                if (fullControl.Length == 2)
                {
                    mainControl = fullControl[0];
                    subControl = fullControl[1];
                    return true;
                }
            }

            mainControl = null;
            subControl = null;
            return false;
        }*/

        /*
        /// <summary>
        /// Searches through the given <paramref name="binding"/> and gets the <paramref name="fullControl"/> specified within it.
        /// <para/>
        /// A '<paramref name="fullControl"/>' contains the entire control (including the composite binding (if any) in the path).
        /// </summary>
        /// <returns>
        /// True if a <paramref name="fullControl"/> was specified in the <paramref name="binding"/>. Otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetControlFromPath(string binding, out string[] fullControl)
        {
            if (binding == null) throw new ArgumentNullException(nameof(binding));

            List<string> outControls = new();

            binding += "/"; // Append Terminating Forward Slash.
            List<int> indexes = IndexOfAll(binding, "/");

            Debug.Log("Binding: " + binding + "     Indexes: " + indexes.Count);

            for (int i = 0; i < indexes.Count - 1; i++)
            {
                int start = indexes[i];
                int end = indexes[i + 1];

                outControls.Add(binding.Substring(start + 1, (end - start) - 1));
            }

            fullControl = outControls.ToArray();
            return outControls.Count != 0;
        }*/
        #endregion
    }
}
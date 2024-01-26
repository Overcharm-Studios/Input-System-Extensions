using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Overcharm.InputSystemExtensions
{
    /// <summary>
    /// A simple <see cref="MonoBehaviour"/> that creates an instance of a given <see cref="InputActionAsset"/> and enables it.
    /// </summary>
    public sealed class InputAssetInstancer : MonoBehaviour
    {
        // # ================================ # //
        // # ------------ Fields ------------ # //
        // # ================================ # //

        #region inputActionAsset
        /// <summary>
        /// The <see cref="InputActionAsset"/> that was defined in the Unity Editor.
        /// </summary>
        /// <remarks>
        /// This field is only used for in-editor authoring. The runtime updates this field when appropriate.
        /// </remarks>
        [SerializeField] private InputActionAsset inputActionAsset;
        #endregion

        // Unserialized

        private InputActionAsset _inputAssetInstance;

        // # ================================ # //
        // # ---------- Properties ---------- # //
        // # ================================ # //

        #region AssetTemplate
        public InputActionAsset AssetTemplate => inputActionAsset;
        #endregion

        #region AssetInstance
        public InputActionAsset AssetInstance => _inputAssetInstance;
        #endregion

        // # ================================ # //
        // # --------- Unity Methods -------- # //
        // # ================================ # //

        #region Awake
        private void Awake()
        {
            if (AssetTemplate == null) return;

            _inputAssetInstance = ScriptableObject.Instantiate(AssetTemplate);
        }
        #endregion

        #region OnEnable
        private void OnEnable()
        {
            if (AssetInstance == null) return;

            _inputAssetInstance.Enable();
        }
        #endregion

        #region OnDisable
        private void OnDisable()
        {
            if (AssetInstance == null) return;

            _inputAssetInstance.Disable();
        }
        #endregion

        #region OnDestroy
        private void OnDestroy()
        {
            if (AssetInstance == null) return;

            Destroy(AssetInstance);
        }
        #endregion
    }
}


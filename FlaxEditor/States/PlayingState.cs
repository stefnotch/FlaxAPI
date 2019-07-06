// Copyright (c) 2012-2019 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;
using FlaxEditor.Content.Settings;
using FlaxEditor.SceneGraph;
using FlaxEditor.Utilities;
using FlaxEngine;
using FlaxEngine.Utilities;

namespace FlaxEditor.States
{
    /// <summary>
    /// In this state editor is simulating game.
    /// </summary>
    /// <seealso cref="FlaxEditor.States.EditorState" />
    public sealed class PlayingState : EditorState
    {
        private readonly DuplicateScenes _duplicateScenes = new DuplicateScenes();
        private readonly List<Guid> _selectedObjects = new List<Guid>();

        /// <summary>
        /// Gets a value indicating whether any scene was dirty before entering the play mode.
        /// </summary>
        public bool WasDirty => _duplicateScenes.WasDirty;

        /// <inheritdoc />
        public override bool CanEditScene => true;

        /// <inheritdoc />
        public override bool CanEnterPlayMode => true;

        /// <summary>
        /// Occurs when play mode is starting (before scene duplicating).
        /// </summary>
        public event Action SceneDuplicating;

        /// <summary>
        /// Occurs when play mode is starting (after scene duplicating).
        /// </summary>
        public event Action SceneDuplicated;

        /// <summary>
        /// Occurs when play mode is ending (before scene restoring).
        /// </summary>
        public event Action SceneRestoring;

        /// <summary>
        /// Occurs when play mode is ending (after scene restoring).
        /// </summary>
        public event Action SceneRestored;

        /// <summary>
        /// Gets or sets a value indicating whether game logic is paused.
        /// </summary>
        public bool IsPaused
        {
            get => false; // Time.GamePaused;
            set
            {
                if (!IsActive)
                    throw new InvalidOperationException();

                //Time.GamePaused = value;
            }
        }

        internal PlayingState(Editor editor)
        : base(editor)
        {
            SetupEditorEnvOptions();
        }

        private void CacheSelection()
        {
            _selectedObjects.Clear();
            for (int i = 0; i < Editor.SceneEditing.Selection.Count; i++)
            {
                _selectedObjects.Add(Editor.SceneEditing.Selection[i].ID);
            }
        }

        private void RestoreSelection()
        {
            var count = Editor.SceneEditing.Selection.Count;
            Editor.SceneEditing.Selection.Clear();
            for (int i = 0; i < _selectedObjects.Count; i++)
            {
                var node = SceneGraphFactory.FindNode(_selectedObjects[i]);
                if (node != null)
                    Editor.SceneEditing.Selection.Add(node);
            }
            if (Editor.SceneEditing.Selection.Count != count)
                Editor.SceneEditing.OnSelectionChanged();
        }

        /// <inheritdoc />
        public override string Status => "Play Mode!";

        /// <inheritdoc />
        public override void OnEnter()
        {
            CacheSelection();

            // Remove references to the scene objects
            Editor.Scene.ClearRefsToSceneObjects(true);

            // Apply game settings (user may modify them before the gameplay)
            GameSettings.Apply();

            // Duplicate editor scene for simulation
            SceneDuplicating?.Invoke();
            _duplicateScenes.GatherSceneData();
            Editor.Internal_SetPlayMode(true);
            IsPaused = false;
            _duplicateScenes.CreateScenes();
            SceneDuplicated?.Invoke();
            RestoreSelection();

            // Fire event
            Editor.OnPlayBegin();
        }

        private void SetupEditorEnvOptions()
        {
            Time.TimeScale = 1.0f;
            Time.UpdateFPS = 60;
            Time.PhysicsFPS = 30;
            Time.DrawFPS = 60;
            Physics.AutoSimulation = true;
            Screen.CursorVisible = true;
            Screen.CursorLock = CursorLockMode.None;
        }

        /// <inheritdoc />
        public override void OnExit(State nextState)
        {
            IsPaused = true;

            // Remove references to the scene objects
            Editor.Scene.ClearRefsToSceneObjects(true);

            // Restore editor scene
            SceneRestoring?.Invoke();
            _duplicateScenes.DeletedScenes();
            Editor.Internal_SetPlayMode(false);
            _duplicateScenes.RestoreSceneData();
            SceneRestored?.Invoke();

            // Restore game settings and state for editor environment
            SetupEditorEnvOptions();
            var win = Editor.Windows.GameWin?.Root;
            if (win != null)
                win.Cursor = CursorType.Default;
            IsPaused = true;
            RestoreSelection();

            // Fire event
            Editor.OnPlayEnd();
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// A Visual Element which handles and displays a Tile Palette Clipboard.
    /// A Tile Palette Clipboard shows the Active Palette for Grid Painting and allows
    /// users to use the Active Brush to assign and pick items for painting.
    /// </summary>
    [UxmlElement]
    public partial class TilePaletteClipboardElement : IMGUIContainer
    {
        /// <summary>
        /// Factory for TilePaletteClipboardElement.
        /// </summary>
        [Obsolete("TilePaletteClipboardElementFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
        public class TilePaletteClipboardElementFactory : UxmlFactory<TilePaletteClipboardElement, TilePaletteClipboardElementUxmlTraits> {}
        /// <summary>
        /// UxmlTraits for TilePaletteClipboardElement.
        /// </summary>
        [Obsolete("TilePaletteClipboardElementUxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
        public class TilePaletteClipboardElementUxmlTraits : UxmlTraits {}

        private new static readonly string ussClassName = "unity-tilepalette-clipboard-element";
        private static readonly string k_Name = L10n.Tr("Tile Palette Clipboard Element");

        private GridPaintPaletteClipboard m_TilePaletteClipboard;
        private EditorWindow m_Window;

        /// <summary>
        /// Callback when the active Brush does a Pick on the Clipboard.
        /// </summary>
        public event Action onBrushPicked;

        /// <summary>
        /// Whether the clipboard is unlocked for editing.
        /// </summary>
        public bool clipboardUnlocked
        {
            get => m_TilePaletteClipboard.unlocked;
            set => m_TilePaletteClipboard.unlocked = value;
        }

        /// <summary>
        /// The last active grid position on the clipboard.
        /// </summary>
        public Vector3Int clipboardMouseGridPosition => new Vector3Int(m_TilePaletteClipboard.mouseGridPosition.x, m_TilePaletteClipboard.mouseGridPosition.y, m_TilePaletteClipboard.zPosition);

        /// <summary>
        /// Callback when the clipboard unlock status has changed
        /// </summary>
        public event Action<bool> clipboardUnlockedChanged;

        internal GridPaintPaletteClipboard clipboardView => m_TilePaletteClipboard;

        /// <summary>
        /// Initializes and returns an instance of TilePaletteClipboardElement.
        /// </summary>
        public TilePaletteClipboardElement()
        {
            AddToClassList(ussClassName);

            name = k_Name;
            TilePaletteOverlayUtility.SetStyleSheet(this);

            onGUIHandler = OnClipboardGUI;

            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            if (m_TilePaletteClipboard == null)
            {
                m_TilePaletteClipboard = ScriptableObject.CreateInstance<GridPaintPaletteClipboard>();
                m_TilePaletteClipboard.hideFlags = HideFlags.HideAndDontSave;
                m_TilePaletteClipboard.unlockedChanged += UnlockChanged;
                m_TilePaletteClipboard.unlocked = false;
                m_TilePaletteClipboard.attachedVisualElement = this;
            }

            GridPaintingState.beforePaletteChanged += BeforePaletteChanged;
            GridPaintingState.paletteChanged += PaletteChanged;
        }

        private void UnlockChanged(bool unlocked)
        {
            clipboardUnlockedChanged?.Invoke(unlocked);
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            m_TilePaletteClipboard.unlockedChanged -= UnlockChanged;
            GridPaintingState.beforePaletteChanged -= BeforePaletteChanged;
            GridPaintingState.paletteChanged -= PaletteChanged;

            Cleanup();
        }

        /// <summary>
        /// Handles cleanup for the Tile Palette Clipboard.
        /// </summary>
        private void Cleanup()
        {
            UnityEngine.Object.DestroyImmediate(m_TilePaletteClipboard);
            m_TilePaletteClipboard = null;
        }

        private void BeforePaletteChanged()
        {
            m_TilePaletteClipboard.OnBeforePaletteSelectionChanged();
        }

        private void PaletteChanged(GameObject palette)
        {
            m_TilePaletteClipboard.OnAfterPaletteSelectionChanged();
            if (m_Window != null)
                m_Window.Repaint();
        }

        private void OnClipboardGUI()
        {
            var clipboardRect = GUILayoutUtility.GetRect(layout.width, layout.height);
            if (onBrushPicked != null && m_TilePaletteClipboard != null)
                m_TilePaletteClipboard.onBrushPicked += onBrushPicked;
            m_TilePaletteClipboard.OnClipboardGUI(clipboardRect);
            if (onBrushPicked != null && m_TilePaletteClipboard != null)
                m_TilePaletteClipboard.onBrushPicked -= onBrushPicked;
        }
    }
}

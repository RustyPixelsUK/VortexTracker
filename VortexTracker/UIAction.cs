using LibVT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexTracker
{
    public class UIAction
    {
        private bool _enabled = true;
        private bool _checked = false;
        private string _category = null;
        private UIActionType _actionType = UIActionType.None;
        private string _text = string.Empty;
        private string _hint = null;
        private int _imageIndex = -1;
        private Keys _shortCut = Keys.None;
        private readonly List<Component> _components = new();
        private EventHandler<EventArgs> _execute;
        private EventHandler<EventArgs> _update;

        public IReadOnlyList<Component> Components => _components;

        internal void InternalUpdate() => UpdateComponents();   // keep original logic
        internal void InternalUpdate(object sender, EventArgs e) => Update(sender, e);
        internal void InternalExecute(object sender, EventArgs e) => Execute(sender, e);

        public UIAction(UIActionType actionType, string category, string text, string hint, int imageIndex, Keys shortCut)
        {
            _actionType = actionType;
            _category = category;
            _text = text;
            _hint = hint;
            _imageIndex = imageIndex;
            _shortCut = shortCut;
        }

        public UIAction(UIActionType actionType, string category, string text, string hint, int imageIndex)
            : this(actionType, category, text, hint, imageIndex, Keys.None) { }

        public void AddComponents(params Component[] components)
        {
            foreach (var component in components)
                AddComponent(component);
        }

        public void AddComponent(Component component)
        {
            _components.Add(component);
            UpdateComponent(component);
        }

        public void AddEvents(EventHandler<EventArgs> execute, EventHandler<EventArgs> update)
        {
            _execute = execute;
            _update = update;
        }

        private void UpdateComponents()
        {
            foreach (var component in _components)
                UpdateComponent(component);
        }

        private ToolStrip GetTopLevelToolStripOwner(ToolStripItem toolStripItem)
        {
            ToolStripItem currentItem = toolStripItem;

            while (currentItem.OwnerItem != null)
                currentItem = currentItem.OwnerItem;

            return currentItem.Owner;
        }

        private void UpdateComponent(Component component)
        {
            string buttonText = _text;
            string menuText = _text;

            if (!string.IsNullOrEmpty(_text))
            {
                if (_text.Contains('|'))
                {
                    string[] parts = _text.Split('|', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    buttonText = parts[0];
                    menuText = parts[1];
                }
            }

            if (component is ButtonBase buttonBase)
            {
                if (buttonBase.ImageIndex != _imageIndex)
                    buttonBase.ImageIndex = _imageIndex;

                if (buttonBase.Enabled != _enabled)
                    buttonBase.Enabled = _enabled;

                if (buttonBase.Text != buttonText)
                    buttonBase.Text = buttonText;
            }
            else if (component is ToolStripButton toolStripButton)
            {
                if (toolStripButton.ImageIndex != _imageIndex)
                    toolStripButton.ImageIndex = _imageIndex;

                if (toolStripButton.Enabled != _enabled)
                    toolStripButton.Enabled = _enabled;

                if (toolStripButton.Checked != _checked)
                    toolStripButton.Checked = _checked;

                if (toolStripButton.Text != buttonText)
                    toolStripButton.Text = buttonText;

                if (toolStripButton.ToolTipText != _hint)
                    toolStripButton.ToolTipText = _hint;
            }
            else if (component is ToolStripItem toolStripItem)
            {
                if (toolStripItem.ImageIndex != _imageIndex)
                {
                    toolStripItem.ImageIndex = _imageIndex;
                    toolStripItem.Image = (_imageIndex != -1 ? GetTopLevelToolStripOwner(toolStripItem).ImageList?.Images[_imageIndex] : null);
                }

                if (toolStripItem.Enabled != _enabled)
                    toolStripItem.Enabled = _enabled;

                if (toolStripItem.Text != menuText)
                    toolStripItem.Text = menuText;

                if (toolStripItem.ToolTipText != _hint)
                    toolStripItem.ToolTipText = _hint;
            }
        }

        public void Execute(object sender, EventArgs e)
        {
            if (!_enabled)
                return;

            if (_execute != null)
                _execute(sender, e);
        }

        public void Update(object sender, EventArgs e)
        {
            if (!_enabled)
                return;

            if (_update != null)
                _update(sender, e);
        }

        public UIActionType ActionType => _actionType;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                UpdateComponents();
            }
        }

        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                UpdateComponents();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                UpdateComponents();
            }
        }

        public int ImageIndex
        {
            get => _imageIndex;
            set
            {
                _imageIndex = value;
                UpdateComponents();
            }
        }

        public string Hint
        {
            get => _hint;
            set
            {
                _hint = value;
                UpdateComponents();
            }
        }

        public Keys ShortCut
        {
            get => _shortCut;
            set
            {
                _shortCut = value;
                UpdateComponents();
            }
        }
    }
}

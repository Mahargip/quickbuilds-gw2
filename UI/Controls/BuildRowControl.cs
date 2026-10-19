using System;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using QuickBuilds.Models;
using QuickBuilds.Services;

namespace QuickBuilds.UI.Controls
{
    public class BuildRowControl : Panel
    {
        private readonly BuildTemplateEntry _build;
        private readonly Label _titleLabel;
        private readonly Label _categoryLabel;
        private readonly StandardButton _copyButton;
        private readonly StandardButton _editButton;
        private readonly StandardButton _deleteButton;

        public BuildTemplateEntry Build => _build;

        public event EventHandler<BuildTemplateEntry>? Selected;
        public event EventHandler<BuildTemplateEntry>? EditRequested;
        public event EventHandler<BuildTemplateEntry>? DeleteRequested;

        public BuildRowControl(BuildTemplateEntry build, int width)
        {
            _build = build;
            Width = width;
            Height = 44;
            BackgroundColor = new Color(22, 24, 30, 200);

            // Title
            string titleText = string.IsNullOrEmpty(build.EliteSpec)
                ? build.Title
                : $"{build.Title} ({build.EliteSpec})";

            _titleLabel = new Label
            {
                Text = titleText,
                Font = GameService.Content.DefaultFont14,
                TextColor = build.GetProfessionColor(),
                Location = new Point(12, 6),
                Width = Math.Max(100, width - 210),
                Height = 18,
                Parent = this
            };

            // Category tag & Chat code preview
            string codeSnippet = build.ChatCode.Length > 16 ? build.ChatCode.Substring(0, 16) + "..." : build.ChatCode;
            _categoryLabel = new Label
            {
                Text = $"[{build.Category}]  {codeSnippet}",
                Font = GameService.Content.DefaultFont12,
                TextColor = Color.LightGray * 0.7f,
                Location = new Point(12, 24),
                Width = Math.Max(100, width - 210),
                Height = 16,
                Parent = this
            };

            // Action Buttons on Right
            int btnY = 9;

            // Copy Button (No emojis, clean text)
            _copyButton = new StandardButton
            {
                Text = "Copy",
                Location = new Point(width - 180, btnY),
                Size = new Point(65, 26),
                BasicTooltipText = "Copy chat code to clipboard to paste in-game",
                Parent = this
            };
            _copyButton.Click += OnCopyClicked;

            // Edit Button
            _editButton = new StandardButton
            {
                Text = "Edit",
                Location = new Point(width - 110, btnY),
                Size = new Point(50, 26),
                Parent = this
            };
            _editButton.Click += (s, e) => EditRequested?.Invoke(this, _build);

            // Delete Button
            _deleteButton = new StandardButton
            {
                Text = "Del",
                Location = new Point(width - 55, btnY),
                Size = new Point(45, 26),
                Parent = this
            };
            _deleteButton.Click += (s, e) => DeleteRequested?.Invoke(this, _build);

            // Row click selection
            Click += (s, e) => Selected?.Invoke(this, _build);
            _titleLabel.Click += (s, e) => Selected?.Invoke(this, _build);
            _categoryLabel.Click += (s, e) => Selected?.Invoke(this, _build);
        }

        private async void OnCopyClicked(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_build.ChatCode)) return;

            bool success = ClipboardService.SetText(_build.ChatCode);
            if (success)
            {
                _copyButton.Text = "Copied!";
                _copyButton.Enabled = false;

                await Task.Delay(2000);

                if (_copyButton.Parent != null)
                {
                    _copyButton.Text = "Copy";
                    _copyButton.Enabled = true;
                }
            }
        }

        protected override void OnMouseEntered(MouseEventArgs e)
        {
            BackgroundColor = new Color(42, 46, 58, 230);
            base.OnMouseEntered(e);
        }

        protected override void OnMouseLeft(MouseEventArgs e)
        {
            BackgroundColor = new Color(22, 24, 30, 200);
            base.OnMouseLeft(e);
        }
    }
}

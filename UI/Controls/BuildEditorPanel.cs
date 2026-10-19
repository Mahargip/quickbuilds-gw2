using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using QuickBuilds.Models;

namespace QuickBuilds.UI.Controls
{
    public class BuildEditorPanel : Panel
    {
        private readonly Label _headerLabel;
        private readonly Label _titlePrompt;
        private readonly TextBox _titleBox;

        private readonly Label _professionPrompt;
        private readonly Dropdown _professionDropdown;

        private readonly Label _specPrompt;
        private readonly TextBox _specBox;

        private readonly Label _categoryPrompt;
        private readonly Dropdown _categoryDropdown;

        private readonly Label _codePrompt;
        private readonly TextBox _codeBox;

        private readonly Label _notesPrompt;
        private readonly TextBox _notesBox;

        private readonly StandardButton _saveButton;
        private readonly StandardButton _cancelButton;

        private BuildTemplateEntry? _editingBuild;

        public event EventHandler<BuildTemplateEntry>? BuildSaved;
        public event EventHandler? Cancelled;

        private static readonly string[] Professions = new[]
        {
            "Necromancer", "Guardian", "Warrior", "Engineer",
            "Ranger", "Thief", "Elementalist", "Mesmer", "Revenant"
        };

        private static readonly string[] Categories = new[]
        {
            "Open World", "Raid", "Strike", "Fractal", "WvW", "PvP", "Other"
        };

        public BuildEditorPanel(int width, int height)
        {
            Width = width;
            Height = height;
            BackgroundColor = new Color(18, 20, 26, 245);
            Visible = false;

            int leftCol = 20;
            int rightCol = 220;

            // Header
            _headerLabel = new Label
            {
                Text = "Add New Build",
                Font = GameService.Content.DefaultFont18,
                TextColor = Color.Gold,
                Location = new Point(leftCol, 15),
                Width = 400,
                Height = 24,
                Parent = this
            };

            // 1. Title
            _titlePrompt = new Label
            {
                Text = "Build Title:",
                Font = GameService.Content.DefaultFont14,
                Location = new Point(leftCol, 52),
                Width = 100,
                Parent = this
            };
            _titleBox = new TextBox
            {
                PlaceholderText = "e.g. Condi Scourge - Raid DPS",
                Location = new Point(leftCol, 74),
                Width = 460,
                Parent = this
            };

            // 2. Profession & Elite Spec
            _professionPrompt = new Label
            {
                Text = "Profession:",
                Font = GameService.Content.DefaultFont14,
                Location = new Point(leftCol, 115),
                Width = 120,
                Parent = this
            };
            _professionDropdown = new Dropdown
            {
                Location = new Point(leftCol, 137),
                Width = 180,
                Parent = this
            };
            foreach (var prof in Professions)
            {
                _professionDropdown.Items.Add(prof);
            }
            _professionDropdown.SelectedItem = "Necromancer";

            _specPrompt = new Label
            {
                Text = "Elite Spec (optional):",
                Font = GameService.Content.DefaultFont14,
                Location = new Point(rightCol, 115),
                Width = 160,
                Parent = this
            };
            _specBox = new TextBox
            {
                PlaceholderText = "e.g. Scourge, Reaper",
                Location = new Point(rightCol, 137),
                Width = 260,
                Parent = this
            };

            // 3. Category (Game Mode)
            _categoryPrompt = new Label
            {
                Text = "Category / Mode:",
                Font = GameService.Content.DefaultFont14,
                Location = new Point(leftCol, 178),
                Width = 150,
                Parent = this
            };
            _categoryDropdown = new Dropdown
            {
                Location = new Point(leftCol, 200),
                Width = 180,
                Parent = this
            };
            foreach (var cat in Categories)
            {
                _categoryDropdown.Items.Add(cat);
            }
            _categoryDropdown.SelectedItem = "Open World";

            // 4. Chat Code
            _codePrompt = new Label
            {
                Text = "Chat Code ([&DQY...]):",
                Font = GameService.Content.DefaultFont14,
                Location = new Point(leftCol, 240),
                Width = 250,
                Parent = this
            };
            _codeBox = new TextBox
            {
                PlaceholderText = "Paste chat code here (e.g. [&DQk1KTIlPCp+...])",
                Location = new Point(leftCol, 262),
                Width = 460,
                Parent = this
            };

            // 5. Notes & Gear
            _notesPrompt = new Label
            {
                Text = "Gear Notes & Recommendations (optional):",
                Font = GameService.Content.DefaultFont14,
                Location = new Point(leftCol, 305),
                Width = 350,
                Parent = this
            };
            _notesBox = new TextBox
            {
                PlaceholderText = "Recommended stats, weapons, runes, relics, or rotation tips...",
                Location = new Point(leftCol, 327),
                Width = 460,
                Height = 85,
                Parent = this
            };

            // Action Buttons
            _saveButton = new StandardButton
            {
                Text = "Save Build",
                Location = new Point(leftCol, 430),
                Size = new Point(130, 32),
                Parent = this
            };
            _saveButton.Click += OnSaveClicked;

            _cancelButton = new StandardButton
            {
                Text = "Cancel",
                Location = new Point(165, 430),
                Size = new Point(100, 32),
                Parent = this
            };
            _cancelButton.Click += (s, e) =>
            {
                Visible = false;
                Cancelled?.Invoke(this, EventArgs.Empty);
            };
        }

        public void OpenForNew(string? defaultProfession = null)
        {
            _editingBuild = null;
            _headerLabel.Text = "Add New Build";

            _titleBox.Text = string.Empty;
            _specBox.Text = string.Empty;
            _codeBox.Text = string.Empty;
            _notesBox.Text = string.Empty;

            if (!string.IsNullOrEmpty(defaultProfession) && Array.Exists(Professions, p => p.Equals(defaultProfession, StringComparison.OrdinalIgnoreCase)))
            {
                _professionDropdown.SelectedItem = defaultProfession;
            }
            else
            {
                _professionDropdown.SelectedItem = "Necromancer";
            }

            _categoryDropdown.SelectedItem = "Open World";
            Visible = true;
        }

        public void OpenForEdit(BuildTemplateEntry build)
        {
            _editingBuild = build;
            _headerLabel.Text = "Edit Build Template";

            _titleBox.Text = build.Title;
            _specBox.Text = build.EliteSpec;
            _codeBox.Text = build.ChatCode;
            _notesBox.Text = build.Notes;

            if (Array.Exists(Professions, p => p.Equals(build.Profession, StringComparison.OrdinalIgnoreCase)))
            {
                _professionDropdown.SelectedItem = build.Profession;
            }

            if (Array.Exists(Categories, c => c.Equals(build.Category, StringComparison.OrdinalIgnoreCase)))
            {
                _categoryDropdown.SelectedItem = build.Category;
            }

            Visible = true;
        }

        private void OnSaveClicked(object sender, MouseEventArgs e)
        {
            string title = _titleBox.Text.Trim();
            string code = _codeBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                return;
            }

            var build = _editingBuild ?? new BuildTemplateEntry();
            build.Title = title;
            build.Profession = _professionDropdown.SelectedItem ?? "Necromancer";
            build.EliteSpec = _specBox.Text.Trim();
            build.Category = _categoryDropdown.SelectedItem ?? "Open World";
            build.ChatCode = code;
            build.Notes = _notesBox.Text.Trim();

            Visible = false;
            BuildSaved?.Invoke(this, build);
        }
    }
}

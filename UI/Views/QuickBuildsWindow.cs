using System;
using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using QuickBuilds.Models;
using QuickBuilds.Services;
using QuickBuilds.UI.Controls;

namespace QuickBuilds.UI.Views
{
    public class QuickBuildsWindow : StandardWindow
    {
        private static readonly Logger Logger = Logger.GetLogger<QuickBuildsWindow>();

        private readonly BuildStorageService _storageService;
        private readonly SpecializationDataService _specializationDataService;

        // Sidebar - Profession Filter
        private readonly Panel _sidebarPanel;
        private readonly Dictionary<string, StandardButton> _professionButtons = new Dictionary<string, StandardButton>();
        private string _selectedProfession = "All";

        // Top Filter Bar
        private readonly TextBox _searchBox;
        private readonly Dropdown _categoryFilterDropdown;
        private readonly StandardButton _newBuildButton;

        // Center - Builds Flow Panel
        private readonly FlowPanel _buildsFlow;
        private readonly Label _emptyStateLabel;

        // Bottom - Detail / Preview Panel
        private readonly Panel _previewPanel;
        private readonly Label _previewTitle;
        private readonly Label _previewCategory;
        private readonly StandardButton _tabTreeButton;
        private readonly StandardButton _tabNotesButton;
        private readonly StandardButton _previewCopyButton;

        // Preview sub-containers
        private readonly BuildTraitTreePanel _traitTreePanel;
        private readonly Panel _notesContainer;
        private readonly Label _previewChatCode;
        private readonly Label _previewNotes;

        // Modal Editor Panel
        private readonly BuildEditorPanel _editorPanel;

        private BuildTemplateEntry? _selectedBuild;
        private bool _isTreeTabActive = true;

        private static readonly string[] ProfessionList = new[]
        {
            "All",
            "Necromancer",
            "Guardian",
            "Warrior",
            "Engineer",
            "Ranger",
            "Thief",
            "Elementalist",
            "Mesmer",
            "Revenant"
        };

        private static readonly string[] CategoryFilters = new[]
        {
            "All",
            "Open World",
            "Raid",
            "Strike",
            "Fractal",
            "WvW",
            "PvP",
            "Other"
        };

        public QuickBuildsWindow(
            AsyncTexture2D background,
            BuildStorageService storageService,
            SpecializationDataService specializationDataService)
            : base(background, new Rectangle(25, 26, 820, 680), new Rectangle(35, 45, 775, 620))
        {
            _storageService = storageService;
            _specializationDataService = specializationDataService;

            Title = "QuickBuilds";
            Subtitle = "Lightweight Build Manager";
            Id = "QuickBuilds_MainWindow_v2";
            SavesPosition = true;
            Parent = GameService.Graphics.SpriteScreen;

            // 1. Left Sidebar (Professions)
            _sidebarPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Point(140, 615),
                Parent = this
            };

            var sidebarTitle = new Label
            {
                Text = "Professions",
                Font = GameService.Content.DefaultFont14,
                TextColor = Color.Gold,
                Location = new Point(5, 5),
                Width = 130,
                Parent = _sidebarPanel
            };

            int btnY = 30;
            foreach (var prof in ProfessionList)
            {
                var profBtn = new StandardButton
                {
                    Text = prof,
                    Location = new Point(0, btnY),
                    Size = new Point(135, 28),
                    Parent = _sidebarPanel
                };

                string p = prof;
                profBtn.Click += (s, e) => SelectProfession(p);
                _professionButtons[prof] = profBtn;
                btnY += 32;
            }

            // 2. Top Bar (Search, Category Filter, New Build)
            int contentX = 150;
            int contentWidth = 615;

            _searchBox = new TextBox
            {
                PlaceholderText = "Search builds...",
                Location = new Point(contentX, 5),
                Size = new Point(230, 30),
                Parent = this
            };
            _searchBox.TextChanged += (s, e) => RefreshBuildsList();

            _categoryFilterDropdown = new Dropdown
            {
                Location = new Point(contentX + 240, 5),
                Size = new Point(160, 30),
                Parent = this
            };
            foreach (var cat in CategoryFilters)
            {
                _categoryFilterDropdown.Items.Add(cat);
            }
            _categoryFilterDropdown.SelectedItem = "All";
            _categoryFilterDropdown.ValueChanged += (s, e) => RefreshBuildsList();

            // Editor Overlay Panel (initialized first so callbacks can reference it)
            _editorPanel = new BuildEditorPanel(contentWidth, 615)
            {
                Location = new Point(contentX, 0),
                Parent = this,
                Visible = false
            };
            _editorPanel.BuildSaved += (s, savedBuild) =>
            {
                if (_storageService.AllBuilds.Any(b => b.Id == savedBuild.Id))
                {
                    _storageService.Update(savedBuild);
                }
                else
                {
                    _storageService.Add(savedBuild);
                }
                HideEditor();
                RefreshBuildsList();
                SelectBuild(savedBuild);
            };
            _editorPanel.Cancelled += (s, e) =>
            {
                HideEditor();
            };

            _newBuildButton = new StandardButton
            {
                Text = "+ New Build",
                Location = new Point(contentX + 410, 5),
                Size = new Point(120, 30),
                BasicTooltipText = "Create a new build template",
                Parent = this
            };
            _newBuildButton.Click += (s, e) =>
            {
                string defaultProf = _selectedProfession.Equals("All", StringComparison.OrdinalIgnoreCase)
                    ? "Necromancer"
                    : _selectedProfession;
                ShowEditor();
                _editorPanel.OpenForNew(defaultProf);
            };

            // 3. Center - Builds Flow Panel
            _buildsFlow = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                ControlPadding = new Vector2(0, 4),
                CanScroll = true,
                Location = new Point(contentX, 42),
                Size = new Point(contentWidth, 235),
                Parent = this
            };

            _emptyStateLabel = new Label
            {
                Text = "No builds found. Click '+ New Build' to add one!",
                Font = GameService.Content.DefaultFont14,
                TextColor = Color.LightGray * 0.7f,
                Location = new Point(20, 30),
                Width = 400,
                Visible = false,
                Parent = _buildsFlow
            };

            // 4. Bottom - Preview / Trait Tree Panel
            _previewPanel = new Panel
            {
                Location = new Point(contentX, 282),
                Size = new Point(contentWidth, 332),
                BackgroundColor = new Color(18, 20, 26, 210),
                Parent = this
            };

            _previewTitle = new Label
            {
                Text = "Select a build to preview",
                Font = GameService.Content.DefaultFont16,
                TextColor = Color.Gold,
                Location = new Point(12, 6),
                Width = 330,
                Height = 20,
                Parent = _previewPanel
            };

            _previewCategory = new Label
            {
                Text = string.Empty,
                Font = GameService.Content.DefaultFont12,
                TextColor = Color.LightGray * 0.8f,
                Location = new Point(12, 27),
                Width = 330,
                Height = 16,
                Parent = _previewPanel
            };

            // View toggle tabs
            _tabTreeButton = new StandardButton
            {
                Text = "Tree",
                Location = new Point(355, 8),
                Size = new Point(75, 26),
                Visible = false,
                Parent = _previewPanel
            };
            _tabTreeButton.Click += (s, e) => SwitchTab(true);

            _tabNotesButton = new StandardButton
            {
                Text = "Notes",
                Location = new Point(435, 8),
                Size = new Point(75, 26),
                Visible = false,
                Parent = _previewPanel
            };
            _tabNotesButton.Click += (s, e) => SwitchTab(false);

            _previewCopyButton = new StandardButton
            {
                Text = "Copy",
                Location = new Point(515, 8),
                Size = new Point(88, 26),
                Visible = false,
                Parent = _previewPanel
            };
            _previewCopyButton.Click += async (s, e) =>
            {
                if (_selectedBuild != null && !string.IsNullOrWhiteSpace(_selectedBuild.ChatCode))
                {
                    ClipboardService.SetText(_selectedBuild.ChatCode);
                    _previewCopyButton.Text = "Copied!";
                    _previewCopyButton.Enabled = false;
                    await System.Threading.Tasks.Task.Delay(2000);
                    if (_previewCopyButton.Parent != null)
                    {
                        _previewCopyButton.Text = "Copy";
                        _previewCopyButton.Enabled = true;
                    }
                }
            };

            // Trait Tree View Container
            _traitTreePanel = new BuildTraitTreePanel(_specializationDataService, 605, 275)
            {
                Location = new Point(5, 48),
                Parent = _previewPanel
            };

            // Notes View Container
            _notesContainer = new Panel
            {
                Location = new Point(5, 48),
                Size = new Point(605, 275),
                BackgroundColor = new Color(14, 16, 20, 220),
                Visible = false,
                Parent = _previewPanel
            };

            _previewChatCode = new Label
            {
                Text = string.Empty,
                Font = GameService.Content.DefaultFont12,
                TextColor = Color.LightSkyBlue,
                Location = new Point(15, 12),
                Width = 575,
                Height = 20,
                Parent = _notesContainer
            };

            _previewNotes = new Label
            {
                Text = string.Empty,
                Font = GameService.Content.DefaultFont12,
                TextColor = Color.LightGray,
                Location = new Point(15, 38),
                Width = 575,
                Height = 220,
                WrapText = true,
                Parent = _notesContainer
            };

            // Event Subscriptions
            _storageService.BuildsChanged += (s, e) => RefreshBuildsList();

            // Initial view
            SelectProfession("All");
        }

        private void SwitchTab(bool showTree)
        {
            _isTreeTabActive = showTree;
            _traitTreePanel.Visible = showTree;
            _notesContainer.Visible = !showTree;
            _tabTreeButton.Enabled = !showTree;
            _tabNotesButton.Enabled = showTree;
        }

        public void SelectProfession(string profession)
        {
            HideEditor();
            _selectedProfession = profession;

            // Highlight selected button
            foreach (var kvp in _professionButtons)
            {
                bool isSelected = kvp.Key.Equals(profession, StringComparison.OrdinalIgnoreCase);
                kvp.Value.Enabled = !isSelected;
            }

            RefreshBuildsList();
        }

        public void AutoSelectActiveProfession()
        {
            try
            {
                var player = GameService.Gw2Mumble.PlayerCharacter;
                if (player != null && !string.IsNullOrEmpty(player.Profession.ToString()))
                {
                    string prof = player.Profession.ToString();
                    if (_professionButtons.ContainsKey(prof))
                    {
                        SelectProfession(prof);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not auto-select active profession.");
            }
        }

        public void RefreshBuildsList()
        {
            _buildsFlow.ClearChildren();

            string query = _searchBox.Text;
            string category = _categoryFilterDropdown.SelectedItem ?? "All";

            var filtered = _storageService.Filter(_selectedProfession, category, query);

            if (filtered.Count == 0)
            {
                _emptyStateLabel.Visible = true;
                _emptyStateLabel.Parent = _buildsFlow;
                return;
            }

            _emptyStateLabel.Visible = false;

            foreach (var build in filtered)
            {
                var row = new BuildRowControl(build, 595)
                {
                    Parent = _buildsFlow
                };

                row.Selected += (s, b) => SelectBuild(b);
                row.EditRequested += (s, b) =>
                {
                    ShowEditor();
                    _editorPanel.OpenForEdit(b);
                };
                row.DeleteRequested += (s, b) =>
                {
                    _storageService.Delete(b.Id);
                    if (_selectedBuild?.Id == b.Id)
                    {
                        ClearPreview();
                    }
                };
            }

            // If selected build is still in the list, re-preview it
            if (_selectedBuild != null && filtered.Any(b => b.Id == _selectedBuild.Id))
            {
                SelectBuild(_selectedBuild);
            }
            else if (filtered.Count > 0)
            {
                SelectBuild(filtered[0]);
            }
            else
            {
                ClearPreview();
            }
        }

        private void SelectBuild(BuildTemplateEntry build)
        {
            _selectedBuild = build;

            string specText = string.IsNullOrEmpty(build.EliteSpec) ? string.Empty : $" ({build.EliteSpec})";
            _previewTitle.Text = $"{build.Title}{specText}";
            _previewTitle.TextColor = build.GetProfessionColor();

            _previewCategory.Text = $"Mode: {build.Category}  |  Profession: {build.Profession}";
            _previewChatCode.Text = build.ChatCode;
            _previewNotes.Text = string.IsNullOrWhiteSpace(build.Notes) ? "No notes added for this build." : build.Notes;

            _traitTreePanel.SetBuild(build);

            _previewCopyButton.Visible = true;
            _tabTreeButton.Visible = true;
            _tabNotesButton.Visible = true;

            SwitchTab(_isTreeTabActive);
        }

        private void ClearPreview()
        {
            _selectedBuild = null;
            _previewTitle.Text = "Select a build to preview";
            _previewTitle.TextColor = Color.Gold;
            _previewCategory.Text = string.Empty;
            _previewChatCode.Text = string.Empty;
            _previewNotes.Text = string.Empty;
            _previewCopyButton.Visible = false;
            _tabTreeButton.Visible = false;
            _tabNotesButton.Visible = false;
            _traitTreePanel.SetBuild(null);
        }

        private void ShowEditor()
        {
            _searchBox.Visible = false;
            _categoryFilterDropdown.Visible = false;
            _newBuildButton.Visible = false;
            _buildsFlow.Visible = false;
            _previewPanel.Visible = false;
            _editorPanel.Visible = true;
        }

        private void HideEditor()
        {
            _editorPanel.Visible = false;
            _searchBox.Visible = true;
            _categoryFilterDropdown.Visible = true;
            _newBuildButton.Visible = true;
            _buildsFlow.Visible = true;
            _previewPanel.Visible = true;
        }
    }
}

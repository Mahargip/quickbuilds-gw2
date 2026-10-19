using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using QuickBuilds.Services;
using QuickBuilds.UI.Views;

namespace QuickBuilds
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class QuickBuildsModule : Blish_HUD.Modules.Module
    {
        private static readonly Logger Logger = Logger.GetLogger<QuickBuildsModule>();

        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;

        public static QuickBuildsModule Instance { get; private set; } = null!;

        // Services
        private BuildStorageService _storageService = null!;
        private SpecializationDataService _specializationDataService = null!;

        // UI
        private CornerIcon? _cornerIcon;
        private ContextMenuStrip? _contextMenu;
        private QuickBuildsWindow? _mainWindow;
        private AsyncTexture2D? _windowBackground;
        private AsyncTexture2D? _cornerIconTexture;
        private AsyncTexture2D? _cornerIconHoverTexture;

        // Settings
        private SettingEntry<KeyBinding>? _hotkeySetting;
        private SettingEntry<bool>? _autoSelectActiveProfessionSetting;

        [ImportingConstructor]
        public QuickBuildsModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
        {
            Instance = this;
        }

        protected override void DefineSettings(SettingCollection settings)
        {
            _hotkeySetting = settings.DefineSetting(
                "ToggleWindowHotkey",
                new KeyBinding(Keys.None),
                () => "Toggle Window Hotkey",
                () => "Key combination to open or close the QuickBuilds window."
            );

            _autoSelectActiveProfessionSetting = settings.DefineSetting(
                "AutoSelectProfession",
                true,
                () => "Auto-Select Active Profession",
                () => "Automatically switch to your current character's profession tab when opening the window."
            );
        }

        protected override async Task LoadAsync()
        {
            try
            {
                Logger.Info("Initializing QuickBuilds module...");

                // 1. Initialize Storage & Specialization Data
                string storageDir = DirectoriesManager.GetFullDirectoryPath("quick-builds");
                _storageService = new BuildStorageService();
                _storageService.Initialize(storageDir);

                _specializationDataService = new SpecializationDataService();
                _specializationDataService.LoadData(ContentsManager, storageDir);

                // 2. Load Textures
                _windowBackground = AsyncTexture2D.FromAssetId(155997);

                try
                {
                    _cornerIconTexture = ContentsManager.GetTexture("icon.png");
                    _cornerIconHoverTexture = ContentsManager.GetTexture("icon_hover.png");
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Could not load custom icon, using default texture.");
                }

                await Task.CompletedTask;

                // 3. Create Main Window
                _mainWindow = new QuickBuildsWindow(_windowBackground, _storageService, _specializationDataService);

                // 4. Create Corner Icon
                _cornerIcon = new CornerIcon
                {
                    Icon = _cornerIconTexture ?? ContentService.Textures.Pixel,
                    HoverIcon = _cornerIconHoverTexture ?? _cornerIconTexture,
                    BasicTooltipText = "QuickBuilds",
                    Priority = 84729205,
                    Parent = GameService.Graphics.SpriteScreen
                };
                _cornerIcon.Click += (s, e) => ToggleMainWindow();

                // 5. Context Menu (Right Click)
                _contextMenu = new ContextMenuStrip();
                _contextMenu.AddMenuItem("Open QuickBuilds").Click += (s, e) => ToggleMainWindow();
                _contextMenu.AddMenuItem("Current Profession").Click += (s, e) =>
                {
                    if (_mainWindow != null)
                    {
                        _mainWindow.Show();
                        _mainWindow.AutoSelectActiveProfession();
                    }
                };
                _cornerIcon.Menu = _contextMenu;

                // 6. Setup Hotkey
                if (_hotkeySetting?.Value != null)
                {
                    _hotkeySetting.Value.Activated += OnHotkeyActivated;
                }

                Logger.Info("QuickBuilds module loaded successfully!");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load QuickBuilds module.");
            }
        }

        private void ToggleMainWindow()
        {
            if (_mainWindow == null) return;

            if (!_mainWindow.Visible && (_autoSelectActiveProfessionSetting?.Value ?? true))
            {
                _mainWindow.AutoSelectActiveProfession();
            }

            _mainWindow.ToggleWindow();
        }

        private void OnHotkeyActivated(object sender, EventArgs e)
        {
            ToggleMainWindow();
        }

        protected override void Update(GameTime gameTime)
        {
        }

        protected override void Unload()
        {
            if (_hotkeySetting?.Value != null)
            {
                _hotkeySetting.Value.Activated -= OnHotkeyActivated;
            }

            _mainWindow?.Dispose();
            _mainWindow = null;

            _cornerIcon?.Dispose();
            _cornerIcon = null;

            _contextMenu?.Dispose();
            _contextMenu = null;

            _cornerIconTexture?.Dispose();
            _cornerIconTexture = null;

            _cornerIconHoverTexture?.Dispose();
            _cornerIconHoverTexture = null;

            Instance = null!;
            Logger.Info("QuickBuilds unloaded.");
        }
    }
}

using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using QuickBuilds.Models;
using QuickBuilds.Services;

namespace QuickBuilds.UI.Controls
{
    public class BuildTraitTreePanel : Panel
    {
        private readonly SpecializationDataService _dataService;
        private readonly Label _emptyStateLabel;

        public BuildTraitTreePanel(SpecializationDataService dataService, int width, int height = 228)
        {
            _dataService = dataService;

            Width = width;
            Height = height;
            BackgroundColor = new Color(14, 16, 20, 220);

            _emptyStateLabel = new Label
            {
                Text = "Select a build to view its specialization tree.",
                Font = GameService.Content.DefaultFont14,
                TextColor = Color.LightGray * 0.7f,
                Location = new Point(20, (height - 20) / 2),
                Width = width - 40,
                HorizontalAlignment = HorizontalAlignment.Center,
                Parent = this
            };
        }

        public void SetBuild(BuildTemplateEntry? build)
        {
            ClearChildren();

            if (build == null || string.IsNullOrWhiteSpace(build.ChatCode))
            {
                _emptyStateLabel.Text = "Select a build with a valid chat code to preview traits.";
                _emptyStateLabel.Visible = true;
                _emptyStateLabel.Parent = this;
                return;
            }

            var decoded = BuildChatCodeDecoder.Decode(build.ChatCode);
            if (decoded == null || !decoded.IsValid)
            {
                _emptyStateLabel.Text = "Could not decode build template code. Check code format.";
                _emptyStateLabel.Visible = true;
                _emptyStateLabel.Parent = this;
                return;
            }

            _emptyStateLabel.Visible = false;

            int rowY = 0;
            int rowHeight = (Height - 4) / 3;

            for (int i = 0; i < decoded.Specializations.Count; i++)
            {
                var specChoice = decoded.Specializations[i];
                var specDto = _dataService.GetSpecialization(specChoice.SpecializationId);

                if (specDto != null)
                {
                    var row = new SpecializationRowControl(
                        specDto,
                        specChoice.TraitChoice1,
                        specChoice.TraitChoice2,
                        specChoice.TraitChoice3,
                        _dataService,
                        Width,
                        rowHeight)
                    {
                        Location = new Point(0, rowY),
                        Parent = this
                    };
                }
                else
                {
                    // Fallback placeholder if spec ID is unknown
                    new Label
                    {
                        Text = $"Specialization Line {i + 1} (ID: {specChoice.SpecializationId})",
                        Font = GameService.Content.DefaultFont14,
                        TextColor = Color.Gold,
                        Location = new Point(20, rowY + 20),
                        Width = Width - 40,
                        Parent = this
                    };
                }

                rowY += rowHeight + 2;
            }
        }
    }
}

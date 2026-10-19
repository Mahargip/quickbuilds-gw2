using System;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuickBuilds.Models;
using QuickBuilds.Services;

namespace QuickBuilds.UI.Controls
{
    public class SpecializationRowControl : Panel
    {
        private readonly SpecializationDto _spec;
        private readonly int _choice1;
        private readonly int _choice2;
        private readonly int _choice3;

        private readonly AsyncTexture2D? _specBackgroundTexture;
        private readonly AsyncTexture2D? _specEmblemTexture;

        public SpecializationRowControl(
            SpecializationDto spec,
            int choice1,
            int choice2,
            int choice3,
            SpecializationDataService dataService,
            int width,
            int height = 74)
        {
            _spec = spec;
            _choice1 = choice1;
            _choice2 = choice2;
            _choice3 = choice3;

            Width = width;
            Height = height;
            BackgroundColor = new Color(18, 20, 26, 230);

            if (!string.IsNullOrEmpty(spec.Background))
            {
                _specBackgroundTexture = GameService.Content.GetRenderServiceTexture(spec.Background);
            }

            if (!string.IsNullOrEmpty(spec.Icon))
            {
                _specEmblemTexture = GameService.Content.GetRenderServiceTexture(spec.Icon);
            }

            // 1. Specialization Emblem Image on Left
            var specIcon = new Image
            {
                Texture = _specEmblemTexture,
                Location = new Point(10, (height - 54) / 2),
                Size = new Point(54, 54),
                BasicTooltipText = $"{spec.Name} {(spec.Elite ? "[Elite Specialization]" : "[Core Specialization]")}",
                Parent = this
            };

            Color accentColor = spec.Elite ? Color.Gold : Color.LightGreen;

            // 2. Trait Nodes
            // Minor Trait 1
            if (spec.MinorTraits.Count > 0)
            {
                var minor1 = dataService.GetTrait(spec.MinorTraits[0]);
                new TraitNodeControl(
                    minor1?.Icon ?? string.Empty,
                    minor1?.Name ?? "Minor Trait",
                    minor1?.Description ?? string.Empty,
                    isSelected: true,
                    isMinor: true,
                    accentColor,
                    30)
                {
                    Location = new Point(80, (height - 30) / 2),
                    Parent = this
                };
            }

            // Adept Major Traits (Choices 1, 2, 3)
            int majorBaseY = (height - 62) / 2;
            int adeptY = majorBaseY;
            for (int i = 0; i < 3; i++)
            {
                int majorIndex = i;
                int traitId = spec.MajorTraits.Count > majorIndex ? spec.MajorTraits[majorIndex] : 0;
                var trait = dataService.GetTrait(traitId);
                bool isSelected = (_choice1 == i + 1);

                new TraitNodeControl(
                    trait?.Icon ?? string.Empty,
                    trait?.Name ?? $"Adept Trait {i + 1}",
                    trait?.Description ?? string.Empty,
                    isSelected,
                    isMinor: false,
                    accentColor,
                    20)
                {
                    Location = new Point(135, adeptY),
                    Parent = this
                };
                adeptY += 21;
            }

            // Minor Trait 2
            if (spec.MinorTraits.Count > 1)
            {
                var minor2 = dataService.GetTrait(spec.MinorTraits[1]);
                new TraitNodeControl(
                    minor2?.Icon ?? string.Empty,
                    minor2?.Name ?? "Minor Trait",
                    minor2?.Description ?? string.Empty,
                    isSelected: true,
                    isMinor: true,
                    accentColor,
                    30)
                {
                    Location = new Point(185, (height - 30) / 2),
                    Parent = this
                };
            }

            // Master Major Traits (Choices 1, 2, 3)
            int masterY = majorBaseY;
            for (int i = 0; i < 3; i++)
            {
                int majorIndex = 3 + i;
                int traitId = spec.MajorTraits.Count > majorIndex ? spec.MajorTraits[majorIndex] : 0;
                var trait = dataService.GetTrait(traitId);
                bool isSelected = (_choice2 == i + 1);

                new TraitNodeControl(
                    trait?.Icon ?? string.Empty,
                    trait?.Name ?? $"Master Trait {i + 1}",
                    trait?.Description ?? string.Empty,
                    isSelected,
                    isMinor: false,
                    accentColor,
                    20)
                {
                    Location = new Point(240, masterY),
                    Parent = this
                };
                masterY += 21;
            }

            // Minor Trait 3
            if (spec.MinorTraits.Count > 2)
            {
                var minor3 = dataService.GetTrait(spec.MinorTraits[2]);
                new TraitNodeControl(
                    minor3?.Icon ?? string.Empty,
                    minor3?.Name ?? "Minor Trait",
                    minor3?.Description ?? string.Empty,
                    isSelected: true,
                    isMinor: true,
                    accentColor,
                    30)
                {
                    Location = new Point(290, (height - 30) / 2),
                    Parent = this
                };
            }

            // Grandmaster Major Traits (Choices 1, 2, 3)
            int gmY = majorBaseY;
            for (int i = 0; i < 3; i++)
            {
                int majorIndex = 6 + i;
                int traitId = spec.MajorTraits.Count > majorIndex ? spec.MajorTraits[majorIndex] : 0;
                var trait = dataService.GetTrait(traitId);
                bool isSelected = (_choice3 == i + 1);

                new TraitNodeControl(
                    trait?.Icon ?? string.Empty,
                    trait?.Name ?? $"Grandmaster Trait {i + 1}",
                    trait?.Description ?? string.Empty,
                    isSelected,
                    isMinor: false,
                    accentColor,
                    20)
                {
                    Location = new Point(345, gmY),
                    Parent = this
                };
                gmY += 21;
            }

            // 3. Right Title & Summary Labels
            string eliteSuffix = spec.Elite ? " [Elite]" : string.Empty;
            new Label
            {
                Text = $"{spec.Name}{eliteSuffix}",
                Font = GameService.Content.DefaultFont16,
                TextColor = accentColor,
                Location = new Point(390, 13),
                Width = width - 400,
                Height = 22,
                Parent = this
            };

            string choiceText = $"Traits: {FormatChoice(_choice1)} - {FormatChoice(_choice2)} - {FormatChoice(_choice3)}";
            new Label
            {
                Text = choiceText,
                Font = GameService.Content.DefaultFont12,
                TextColor = Color.LightSkyBlue,
                Location = new Point(390, 39),
                Width = width - 400,
                Height = 18,
                Parent = this
            };
        }

        private static string FormatChoice(int choice)
        {
            switch (choice)
            {
                case 1: return "Top";
                case 2: return "Mid";
                case 3: return "Bot";
                default: return "None";
            }
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            base.PaintBeforeChildren(spriteBatch, bounds);

            // 1. Draw Background Art if available
            if (_specBackgroundTexture != null)
            {
                spriteBatch.Draw(_specBackgroundTexture, bounds, Color.White * 0.75f);
            }

            // Dark gradient/contrast overlay so traits remain crisp
            spriteBatch.Draw(ContentService.Textures.Pixel, bounds, new Color(12, 14, 18, 170));

            // 2. Draw connecting lines between active trait centers
            Color lineColor = (_spec.Elite ? Color.LightGoldenrodYellow : Color.LightCyan) * 0.80f;

            Vector2 minor1Center = new Vector2(bounds.X + 95, bounds.Y + (Height / 2f));
            Vector2 major1Center = new Vector2(bounds.X + 145, bounds.Y + GetMajorCenterY(_choice1));
            Vector2 minor2Center = new Vector2(bounds.X + 200, bounds.Y + (Height / 2f));
            Vector2 major2Center = new Vector2(bounds.X + 250, bounds.Y + GetMajorCenterY(_choice2));
            Vector2 minor3Center = new Vector2(bounds.X + 305, bounds.Y + (Height / 2f));
            Vector2 major3Center = new Vector2(bounds.X + 355, bounds.Y + GetMajorCenterY(_choice3));

            DrawLine(spriteBatch, minor1Center, major1Center, lineColor);
            DrawLine(spriteBatch, major1Center, minor2Center, lineColor);
            DrawLine(spriteBatch, minor2Center, major2Center, lineColor);
            DrawLine(spriteBatch, major2Center, minor3Center, lineColor);
            DrawLine(spriteBatch, minor3Center, major3Center, lineColor);

            // Bottom border
            spriteBatch.Draw(ContentService.Textures.Pixel, new Rectangle(bounds.X, bounds.Bottom - 1, bounds.Width, 1), Color.Black * 0.6f);
        }

        private float GetMajorCenterY(int choice)
        {
            int majorBaseY = (Height - 62) / 2;
            switch (choice)
            {
                case 1: return majorBaseY + 10;
                case 2: return majorBaseY + 21 + 10;
                case 3: return majorBaseY + 42 + 10;
                default: return Height / 2f;
            }
        }

        private static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness = 2)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            float length = edge.Length();

            spriteBatch.Draw(
                ContentService.Textures.Pixel,
                new Rectangle((int)start.X, (int)start.Y, (int)length, thickness),
                null,
                color,
                angle,
                Vector2.Zero,
                SpriteEffects.None,
                0f
            );
        }
    }
}

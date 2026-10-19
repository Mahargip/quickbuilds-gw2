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

        private readonly int _emblemX = 15;
        private readonly int _emblemSize = 64;

        private readonly int _minor1X = 110;
        private readonly int _major1X = 195;
        private readonly int _minor2X = 280;
        private readonly int _major2X = 365;
        private readonly int _minor3X = 450;
        private readonly int _major3X = 535;

        private readonly int _minorSize = 32;
        private readonly int _majorSize = 26;
        private readonly int _majorBaseY;

        public SpecializationRowControl(
            SpecializationDto spec,
            int choice1,
            int choice2,
            int choice3,
            SpecializationDataService dataService,
            int width,
            int height = 90)
        {
            _spec = spec;
            _choice1 = choice1;
            _choice2 = choice2;
            _choice3 = choice3;

            Width = width;
            Height = height;
            BackgroundColor = new Color(14, 16, 22, 230);

            Color accentColor = spec.Elite ? Color.Gold : new Color(80, 220, 140);

            // 1. Full-width Specialization Background Artwork (ZIndex = 0)
            if (!string.IsNullOrEmpty(spec.Background))
            {
                new Image
                {
                    Texture = GameService.Content.GetRenderServiceTexture(spec.Background),
                    Location = new Point(0, 0),
                    Size = new Point(width, height),
                    Opacity = 0.55f,
                    ZIndex = 0,
                    Parent = this
                };
            }

            // 2. Trait Line Canvas (ZIndex = 2) - draws lines on top of background and behind icons
            new TraitLineCanvas(this, width, height)
            {
                Parent = this
            };

            // 3. Specialization Emblem Hexagon on Left (ZIndex = 6)
            if (!string.IsNullOrEmpty(spec.Icon))
            {
                new Image
                {
                    Texture = GameService.Content.GetRenderServiceTexture(spec.Icon),
                    Location = new Point(_emblemX, (height - _emblemSize) / 2),
                    Size = new Point(_emblemSize, _emblemSize),
                    ZIndex = 6,
                    BasicTooltipText = $"{spec.Name} {(spec.Elite ? "[Elite Specialization]" : "[Core Specialization]")}",
                    Parent = this
                };
            }

            // Calculate vertical positioning for the 3 stacked major traits
            // Total height for 3 traits with 3px gap = 26*3 + 3*2 = 84px
            _majorBaseY = Math.Max(3, (height - ((3 * _majorSize) + 6)) / 2);

            // 4. Minor Trait 1 (Adept Minor)
            if (spec.MinorTraits.Count > 0)
            {
                var minor1 = dataService.GetTrait(spec.MinorTraits[0]);
                string icon1 = minor1?.Icon ?? string.Empty;
                if (!string.IsNullOrEmpty(icon1))
                {
                    new Image
                    {
                        Texture = GameService.Content.GetRenderServiceTexture(icon1),
                        Location = new Point(_minor1X, (height - _minorSize) / 2),
                        Size = new Point(_minorSize, _minorSize),
                        ZIndex = 6,
                        BasicTooltipText = $"{minor1?.Name} [Minor Trait]\n\n{minor1?.Description}",
                        Parent = this
                    };
                }
            }

            // 5. Adept Major Traits (Choices 1, 2, 3)
            for (int i = 0; i < 3; i++)
            {
                int traitId = spec.MajorTraits.Count > i ? spec.MajorTraits[i] : 0;
                var trait = dataService.GetTrait(traitId);
                string icon = trait?.Icon ?? string.Empty;
                bool isSelected = (_choice1 == i + 1);
                int yPos = _majorBaseY + (i * (_majorSize + 3));

                if (!string.IsNullOrEmpty(icon))
                {
                    if (isSelected)
                    {
                        new Panel
                        {
                            Location = new Point(_major1X - 2, yPos - 2),
                            Size = new Point(_majorSize + 4, _majorSize + 4),
                            BackgroundColor = accentColor,
                            ZIndex = 4,
                            Parent = this
                        };
                    }

                    new Image
                    {
                        Texture = GameService.Content.GetRenderServiceTexture(icon),
                        Location = new Point(_major1X, yPos),
                        Size = new Point(_majorSize, _majorSize),
                        Opacity = isSelected ? 1.0f : 0.28f,
                        ZIndex = 6,
                        BasicTooltipText = $"{trait?.Name} [Major Trait - Tier 1]\n\n{trait?.Description}",
                        Parent = this
                    };
                }
            }

            // 6. Minor Trait 2 (Master Minor)
            if (spec.MinorTraits.Count > 1)
            {
                var minor2 = dataService.GetTrait(spec.MinorTraits[1]);
                string icon2 = minor2?.Icon ?? string.Empty;
                if (!string.IsNullOrEmpty(icon2))
                {
                    new Image
                    {
                        Texture = GameService.Content.GetRenderServiceTexture(icon2),
                        Location = new Point(_minor2X, (height - _minorSize) / 2),
                        Size = new Point(_minorSize, _minorSize),
                        ZIndex = 6,
                        BasicTooltipText = $"{minor2?.Name} [Minor Trait]\n\n{minor2?.Description}",
                        Parent = this
                    };
                }
            }

            // 7. Master Major Traits (Choices 1, 2, 3)
            for (int i = 0; i < 3; i++)
            {
                int traitId = spec.MajorTraits.Count > (3 + i) ? spec.MajorTraits[3 + i] : 0;
                var trait = dataService.GetTrait(traitId);
                string icon = trait?.Icon ?? string.Empty;
                bool isSelected = (_choice2 == i + 1);
                int yPos = _majorBaseY + (i * (_majorSize + 3));

                if (!string.IsNullOrEmpty(icon))
                {
                    if (isSelected)
                    {
                        new Panel
                        {
                            Location = new Point(_major2X - 2, yPos - 2),
                            Size = new Point(_majorSize + 4, _majorSize + 4),
                            BackgroundColor = accentColor,
                            ZIndex = 4,
                            Parent = this
                        };
                    }

                    new Image
                    {
                        Texture = GameService.Content.GetRenderServiceTexture(icon),
                        Location = new Point(_major2X, yPos),
                        Size = new Point(_majorSize, _majorSize),
                        Opacity = isSelected ? 1.0f : 0.28f,
                        ZIndex = 6,
                        BasicTooltipText = $"{trait?.Name} [Major Trait - Tier 2]\n\n{trait?.Description}",
                        Parent = this
                    };
                }
            }

            // 8. Minor Trait 3 (Grandmaster Minor)
            if (spec.MinorTraits.Count > 2)
            {
                var minor3 = dataService.GetTrait(spec.MinorTraits[2]);
                string icon3 = minor3?.Icon ?? string.Empty;
                if (!string.IsNullOrEmpty(icon3))
                {
                    new Image
                    {
                        Texture = GameService.Content.GetRenderServiceTexture(icon3),
                        Location = new Point(_minor3X, (height - _minorSize) / 2),
                        Size = new Point(_minorSize, _minorSize),
                        ZIndex = 6,
                        BasicTooltipText = $"{minor3?.Name} [Minor Trait]\n\n{minor3?.Description}",
                        Parent = this
                    };
                }
            }

            // 9. Grandmaster Major Traits (Choices 1, 2, 3)
            for (int i = 0; i < 3; i++)
            {
                int traitId = spec.MajorTraits.Count > (6 + i) ? spec.MajorTraits[6 + i] : 0;
                var trait = dataService.GetTrait(traitId);
                string icon = trait?.Icon ?? string.Empty;
                bool isSelected = (_choice3 == i + 1);
                int yPos = _majorBaseY + (i * (_majorSize + 3));

                if (!string.IsNullOrEmpty(icon))
                {
                    if (isSelected)
                    {
                        new Panel
                        {
                            Location = new Point(_major3X - 2, yPos - 2),
                            Size = new Point(_majorSize + 4, _majorSize + 4),
                            BackgroundColor = accentColor,
                            ZIndex = 4,
                            Parent = this
                        };
                    }

                    new Image
                    {
                        Texture = GameService.Content.GetRenderServiceTexture(icon),
                        Location = new Point(_major3X, yPos),
                        Size = new Point(_majorSize, _majorSize),
                        Opacity = isSelected ? 1.0f : 0.28f,
                        ZIndex = 6,
                        BasicTooltipText = $"{trait?.Name} [Major Trait - Tier 3]\n\n{trait?.Description}",
                        Parent = this
                    };
                }
            }
        }

        private float GetMajorCenterY(int choice, float centerY)
        {
            switch (choice)
            {
                case 1: return _majorBaseY + (_majorSize / 2f);
                case 2: return _majorBaseY + (_majorSize + 3) + (_majorSize / 2f);
                case 3: return _majorBaseY + (2 * (_majorSize + 3)) + (_majorSize / 2f);
                default: return centerY;
            }
        }

        internal void PaintLines(SpriteBatch spriteBatch, Rectangle bounds)
        {
            float centerY = Height / 2f;

            // Connector lines: crisp bright line for active path
            Color lineColor = _spec.Elite
                ? new Color(255, 235, 150) * 0.90f
                : new Color(225, 245, 255) * 0.85f;

            Vector2 emblemRight = new Vector2(_emblemX + _emblemSize, centerY);
            Vector2 minor1Center = new Vector2(_minor1X + (_minorSize / 2f), centerY);
            Vector2 major1Center = new Vector2(_major1X + (_majorSize / 2f), GetMajorCenterY(_choice1, centerY));
            Vector2 minor2Center = new Vector2(_minor2X + (_minorSize / 2f), centerY);
            Vector2 major2Center = new Vector2(_major2X + (_majorSize / 2f), GetMajorCenterY(_choice2, centerY));
            Vector2 minor3Center = new Vector2(_minor3X + (_minorSize / 2f), centerY);
            Vector2 major3Center = new Vector2(_major3X + (_majorSize / 2f), GetMajorCenterY(_choice3, centerY));

            // Complete chain from Emblem -> Minor 1 -> Major 1 -> Minor 2 -> Major 2 -> Minor 3 -> Major 3
            DrawLine(spriteBatch, emblemRight, minor1Center, lineColor);
            DrawLine(spriteBatch, minor1Center, major1Center, lineColor);
            DrawLine(spriteBatch, major1Center, minor2Center, lineColor);
            DrawLine(spriteBatch, minor2Center, major2Center, lineColor);
            DrawLine(spriteBatch, major2Center, minor3Center, lineColor);
            DrawLine(spriteBatch, minor3Center, major3Center, lineColor);

            // Elite specialization decorative golden border framing
            if (_spec.Elite)
            {
                Color gold = Color.Gold * 0.85f;
                // Top and bottom borders
                spriteBatch.Draw(ContentService.Textures.Pixel, new Rectangle(0, 0, Width, 2), gold);
                spriteBatch.Draw(ContentService.Textures.Pixel, new Rectangle(0, Height - 2, Width, 2), gold);

                // Left corner brackets
                spriteBatch.Draw(ContentService.Textures.Pixel, new Rectangle(0, 0, 3, Height), gold);
                spriteBatch.Draw(ContentService.Textures.Pixel, new Rectangle(0, 0, 16, 3), gold);
                spriteBatch.Draw(ContentService.Textures.Pixel, new Rectangle(0, Height - 3, 16, 3), gold);
            }
            else
            {
                // Subtle separator border between core rows
                spriteBatch.Draw(ContentService.Textures.Pixel, new Rectangle(0, Height - 1, Width, 1), Color.Black * 0.7f);
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
                new Vector2(0, 0.5f),
                SpriteEffects.None,
                0f
            );
        }

        private class TraitLineCanvas : Control
        {
            private readonly SpecializationRowControl _row;

            public TraitLineCanvas(SpecializationRowControl row, int width, int height)
            {
                _row = row;
                Size = new Point(width, height);
                Location = new Point(0, 0);
                ZIndex = 2;
            }

            protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
            {
                _row.PaintLines(spriteBatch, bounds);
            }
        }
    }
}

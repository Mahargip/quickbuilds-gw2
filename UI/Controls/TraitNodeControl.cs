using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuickBuilds.UI.Controls
{
    public class TraitNodeControl : Control
    {
        private readonly AsyncTexture2D? _texture;
        private readonly bool _isSelected;
        private readonly bool _isMinor;
        private readonly Color _accentColor;

        public TraitNodeControl(string iconUrl, string name, string description, bool isSelected, bool isMinor, Color accentColor, int size)
        {
            _isSelected = isSelected;
            _isMinor = isMinor;
            _accentColor = accentColor;
            Size = new Point(size, size);

            if (!string.IsNullOrEmpty(iconUrl))
            {
                _texture = GameService.Content.GetRenderServiceTexture(iconUrl);
            }

            string typeText = isMinor ? "[Minor Trait]" : "[Major Trait]";
            BasicTooltipText = $"{typeText} {name}\n\n{description}";
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
        {
            if (_texture != null && _texture.HasTexture)
            {
                Color drawColor = _isSelected ? Color.White : Color.White * 0.30f;
                spriteBatch.Draw(_texture, bounds, drawColor);
            }
            else
            {
                // Fallback placeholder while icon downloads
                Color placeholderColor = _isSelected ? _accentColor * 0.5f : Color.DarkSlateGray * 0.35f;
                spriteBatch.Draw(ContentService.Textures.Pixel, bounds, placeholderColor);
            }

            // If selected major trait, draw a clean highlight border around it
            if (_isSelected && !_isMinor)
            {
                int t = 2;
                // Top
                spriteBatch.Draw(ContentService.Textures.Pixel, new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 2, t), _accentColor);
                // Bottom
                spriteBatch.Draw(ContentService.Textures.Pixel, new Rectangle(bounds.X - 1, bounds.Bottom - 1, bounds.Width + 2, t), _accentColor);
                // Left
                spriteBatch.Draw(ContentService.Textures.Pixel, new Rectangle(bounds.X - 1, bounds.Y - 1, t, bounds.Height + 2), _accentColor);
                // Right
                spriteBatch.Draw(ContentService.Textures.Pixel, new Rectangle(bounds.Right - 1, bounds.Y - 1, t, bounds.Height + 2), _accentColor);
            }
        }
    }
}

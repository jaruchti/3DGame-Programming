using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3DGameProject
{
    class TitleScreen
    {
        private Texture2D background;
        private SpriteBatch spriteBatch;
        private Rectangle viewportRect;

        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            spriteBatch = new SpriteBatch(device);
            background = content.Load<Texture2D>("Textures/background");

            viewportRect = new Rectangle(0, 0, 
                device.Viewport.Width, 
                device.Viewport.Height);
        }

        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, viewportRect, Color.White);

            spriteBatch.End();
        }
    }
}

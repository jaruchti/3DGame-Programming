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
    abstract class GameTextScreen
    {
        protected SpriteBatch spriteBatch;
        protected SpriteFont largeFont;
        protected SpriteFont mediumFont;
        protected Vector2 textSize;

        public virtual void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            spriteBatch = new SpriteBatch(device);

            largeFont = content.Load<SpriteFont>("Fonts/LargeFont");
            mediumFont = content.Load<SpriteFont>("Fonts/MediumFont");
        }
    }
}

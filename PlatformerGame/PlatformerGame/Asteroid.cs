using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformerGame
{
    class Asteroid
    {
        Sprite sprite = new Sprite();
        KeyboardState state;

        const int movementSpeed = 3;

        public Asteroid()
        {
            sprite.position.X += 200;
            sprite.position.Y += 150;
        }

        public void Load(ContentManager content)
        {
            sprite.Load(content, "rock_large");
        }

        public void Update(float deltaTime)
        {
            state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.D))
            {
                sprite.position.X += movementSpeed;
            }
            if (state.IsKeyDown(Keys.A))
            {
                sprite.position.X -= movementSpeed;
            }
            if (state.IsKeyDown(Keys.W))
            {
                sprite.position.Y -= movementSpeed;
            }
            if (state.IsKeyDown(Keys.S))
            {
                sprite.position.Y += movementSpeed;
            }
            sprite.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}

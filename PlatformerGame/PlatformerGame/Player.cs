using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PlatformerGame
{
    class Player
    {
        Sprite sprite = new Sprite();
        KeyboardState state;
        Game1 game = null;
        bool isFalling = true;
        bool isJumping = false;

        Vector2 velocity = Vector2.Zero;
        Vector2 position = Vector2.Zero;

        public Vector2 Position
        {
            get { return sprite.position; }
        }

        const int movementSpeed = 3;

        public Player(Game1 game)
        {
            this.game = game;
            isFalling = true;
            isJumping = false;
            velocity = Vector2.Zero;
            position = Vector2.Zero;
        }

        public void Load(ContentManager content)
        {
            sprite.Load(content, "hero");
        }

        public void Update(float deltaTime)
        {
            state = Keyboard.GetState();
           
                if (state.IsKeyDown(Keys.Right))
            {
                sprite.position.X += movementSpeed;
            }
            if (state.IsKeyDown(Keys.Left))
            {
                sprite.position.X -= movementSpeed;
            }
            if (state.IsKeyDown(Keys.Up))
            {
                sprite.position.Y -= movementSpeed;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                sprite.position.Y += movementSpeed;
            }

            UpdateInput(deltaTime);
            sprite.Update(deltaTime);
        }

        private void UpdateInput(float deltaTime)
        {
            bool wasMovingLeft = velocity.X < 0;
            bool wasMovingRight = velocity.X > 0;
            bool falling = isFalling;

            Vector2 acceleration = new Vector2(0, Game1.gravity);

            if (Keyboard.GetState().IsKeyDown(Keys.Left) == true)
            {
                acceleration.X -= Game1.acceleration;
            }
            else if (wasMovingLeft == true)
            {
                acceleration.X += Game1.friction;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right) == true)
            {
                acceleration.X += Game1.acceleration;
            }
            else if (wasMovingRight == true)
            {
                acceleration.X -= Game1.friction;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up) == true && this.isJumping == false && falling == false)
            {
                acceleration.Y -= Game1.jumpImpulse;
                this.isJumping = true;
            }

            velocity += acceleration * deltaTime;
            //integrate the forces to calculate the new position and velocity
            
            velocity.X = MathHelper.Clamp(velocity.X, -Game1.maxVelocity.X, Game1.maxVelocity.X);
            velocity.Y = MathHelper.Clamp(velocity.Y, -Game1.maxVelocity.Y, Game1.maxVelocity.Y);
            //clamp the velocity so the player doesn't go too fast

            sprite.position += velocity * deltaTime;

            if ((wasMovingLeft && (velocity.X > 0)) ||
                (wasMovingRight && (velocity.X < 0)))
            {
                velocity.X = 0;
                //clamping velocity so player doesn't jiggle
            }

            //Collision detection below
            int tx = game.PixelToTile(sprite.position.X);
            int ty = game.PixelToTile(sprite.position.Y);
            bool nx = (sprite.position.X) % Game1.tile != 0; //nx = true if player overlaps right
            bool ny = (sprite.position.Y) % Game1.tile != 0; //ny = true if player overlaps below
            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;

            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    //clamp the position to avoid falling through platform
                    sprite.position.Y = game.TileToPixel(ty);
                    this.velocity.Y = 0; //stop falling
                    this.isFalling = false;
                    this.isJumping = false; //No longer jumping or falling
                    ny = false; //no longer overlaps below cells
                }
            }
            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    //clamp position to avoid jumping through above platforms
                    sprite.position.Y = game.TileToPixel(ty + 1);
                    this.velocity.Y = 0; //stops upward velocity
                    cell = celldown;
                    cellright = celldiag; //player's no longer in cell so we clamped them to the cell below
                    ny = false; //no longer overlaps below cells
                }
            }

            if (this.velocity.X > 0)
            {
                if ((cellright && !cell) || (celldiag && !celldown && ny))
                {
                    sprite.position.X = game.TileToPixel(tx); //clamped to x position to stop moving into the platform we just hit
                    this.velocity.X = 0;
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) ||(celldown && !celldiag && ny))
                {
                    sprite.position.X = game.TileToPixel(tx + 1); //clamped to x position to stop moving into the platform we just hit
                    this.velocity.X = 0;
                }
            }
            this.isFalling = !(celldown || (nx && celldiag));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace PlatformerGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static int tile = 64; //1 tile = 1 meter
        public static float meter = tile; //Very exaggerated gravity (6x)
        public static float gravity = meter * 9.8f * 6.0f; //Max vertical speed (10tiles/s horizontal, 15tiles/s vertical)
        public static Vector2 maxVelocity = new Vector2(meter * 10, meter * 15); //Horizontal acceleration (takes .5s to reach max velocity)
        public static float acceleration = maxVelocity.X * 2; //Horizontal friction (takes 1/6 second to stop from max velocity)
        public static float friction = maxVelocity.X * 6; //(a large) instantaneous jump impulse
        public static float jumpImpulse = meter * 1500;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player = null;
        Asteroid asteroid = new Asteroid();

        Camera2D camera = null;
        TiledMap map = null;
        TiledTileLayer collisionLayer;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public int ScreenWidth
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Width;
            }
        }
        public int ScreenHeight
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Height;
            }
        }

               protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Player(this);

            base.Initialize();
        }

               protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice,
                                                                ScreenWidth, ScreenHeight);
            camera = new Camera2D(viewportAdapter);
            camera.Position = new Vector2(0, ScreenHeight);

            map = Content.Load<TiledMap>("Level1");
            foreach (TiledTileLayer layer in map.TileLayers)
            {
                if (layer.Name == "Collisions")
                    collisionLayer = layer;
            }

            player.Load(Content);
            asteroid.Load(Content);
        }

       protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

       protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //camera.Move(new Vector2(50, 0) * deltaTime);
            player.Update(deltaTime);
            asteroid.Update(deltaTime);

            KeyboardState state;
            state = Keyboard.GetState();

            camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);
           /*if (state.IsKeyDown(Keys.W))
            {
                camera.Move(new Vector2(0, -300) * deltaTime);
            }
            if (state.IsKeyDown(Keys.A))
            {
                camera.Move(new Vector2(-300, 0) * deltaTime);
            }
            if (state.IsKeyDown(Keys.S))
            {
                camera.Move(new Vector2(0, 300) * deltaTime);
            }
            if (state.IsKeyDown(Keys.D))
            {
                camera.Move(new Vector2(300, 0) * deltaTime);
            }*/

            base.Update(gameTime);
        }

       protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            // TODO: Add your drawing code here

            var transformMatrix = camera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: transformMatrix);
            map.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin();
            player.Draw(spriteBatch);
            asteroid.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public int PixelToTile(float pixelCoord)
        {
            return (int)Math.Floor(pixelCoord / tile);
        }
        public int TileToPixel (int tileCoord)
        {
            return tile * tileCoord;
        }
        public int CellAtPixelCoord(Vector2 pixelCoords)
        {
            if (pixelCoords.X < 0 ||
                pixelCoords.X > map.WidthInPixels || pixelCoords.Y < 0)
                return 1;
            //Let the player drop to the bottom of the screen (meaning death)
            if (pixelCoords.Y > map.HeightInPixels)
                return 0;
            return CellAtTileCoord(
                PixelToTile(pixelCoords.X), PixelToTile(pixelCoords.Y));
        }
        public int CellAtTileCoord(int tx, int ty)
        {
            if (tx < 0 || tx >= map.Width || ty < 0)
                return 1;
            //Let the player drop to the bottom of the screen (meanning death)
            if (ty >= map.Height)
                return 0;

            TiledTile tile = collisionLayer.GetTile(tx, ty);
            return tile.Id; 
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI411.SimpleEngine;

namespace Lab05
{
    public class Lab05 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix world, view , projection;
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        float angle = 0, angle2 = 0, distance = 10;
        MouseState previousMouseState;
        KeyboardState previousKeyState;

        Skybox skybox;

        public Lab05()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }
        
        protected override void Initialize()
        {

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            string[] skyboxTextures = {
                "skybox/SunsetPNG2", "skybox/SunsetPNG1",
                "skybox/SunsetPNG4", "skybox/SunsetPNG3",
                "skybox/SunsetPNG6", "skybox/SunsetPNG5",
            };
            skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
        }
        
        protected override void UnloadContent()
        {
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState currentMouseState = Mouse.GetState();
            KeyboardState currentKeyState = Keyboard.GetState();

            //camera control
            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Pressed)
            {
                angle += (previousMouseState.X - currentMouseState.X) / 100f;
                angle2 += (previousMouseState.Y - currentMouseState.Y) / 100f;
            }

            else if (currentKeyState.IsKeyDown(Keys.Left) && previousKeyState.IsKeyDown(Keys.Left))
            {
                angle += 0.02f;
            }
            else if (currentKeyState.IsKeyDown(Keys.Right) && previousKeyState.IsKeyDown(Keys.Right))
            {
                angle -= 0.02f;
            }
            else if (currentKeyState.IsKeyDown(Keys.Up) && previousKeyState.IsKeyDown(Keys.Up))
            {
                angle2 += 0.02f;
            }
            else if (currentKeyState.IsKeyDown(Keys.Down) && previousKeyState.IsKeyDown(Keys.Down))
            {
                angle2 -= 0.02f;
            }

            world = Matrix.Identity;
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90), 800f / 600f, 0.1f, 100f);
            //camera position calculation
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                  Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));

            //update view matrix
            view = Matrix.CreateLookAt(
                cameraPosition,
                Vector3.Zero,
                Vector3.Transform(Vector3.Up,
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));

            previousMouseState = Mouse.GetState();
            previousKeyState = Keyboard.GetState();

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            skybox.Draw(view, projection, cameraPosition);

            base.Draw(gameTime);
        }
    }
}

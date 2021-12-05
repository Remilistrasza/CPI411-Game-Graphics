using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab02
{
    public class Lab02 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Effect effect; //shader fuke
        Texture2D texture;
        VertexPositionTexture[] vertices =
        {
            new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0.5f, 0)),
            new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(-1, 0, 0), new Vector2(0, 1))
        };

        Matrix world, view, projection;
        float angle = 0;
        float distance = 2;
        Vector3 cameraPosition;

        public Lab02()
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

            effect = Content.Load<Effect>("SimpleTexture");
            texture = Content.Load<Texture2D>("logo_mg");

            world = Matrix.Identity;
            view = Matrix.CreateLookAt(
                new Vector3(0, 0, 1.5f),
                new Vector3(),
                new Vector3(0, 1, 0)
                );
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f, 100
                );
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //camera control
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                angle += 0.02f;
            else if(Keyboard.GetState().IsKeyDown(Keys.Right))
                angle -= 0.02f;
            else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                distance -= 0.02f;
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                distance += 0.02f;

            //camera position calculation
            cameraPosition = distance * 
                new Vector3(
                    (float)System.Math.Sin(angle),
                    0,
                    (float)System.Math.Cos(angle)
                    );

            //update view matrix
            view = Matrix.CreateLookAt(
                cameraPosition,
                new Vector3(),
                new Vector3(0, 1, 0)
                );
            effect.Parameters["View"].SetValue(view);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.AlphaBlend; //alpha blend

            effect.Parameters["MyTexture"].SetValue(texture);
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
            }

            base.Draw(gameTime);
        }
    }
}

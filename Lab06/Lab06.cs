using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI411.SimpleEngine;

namespace Lab06
{
    public class Lab06 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix world, view, projection;
        Model model;
        Texture2D texture;
        Effect effect;
        Skybox skybox;

        Vector3 cameraPosition = new Vector3(0, 0, 5);
        float angle = 0, angle2 = 0, distance = 2;
        float reflectivity = 0.5f;

        MouseState previousMouseState;
        KeyboardState previousKeyState;

        public Lab06()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }
        
        protected override void Initialize()
        {
            IsMouseVisible = true;
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("Helicopter");
            texture = Content.Load<Texture2D>("HelicopterTexture");
            string[] skyboxTextures = {
                "skybox/SunsetPNG2", "skybox/SunsetPNG1",
                "skybox/SunsetPNG4", "skybox/SunsetPNG3",
                "skybox/SunsetPNG6", "skybox/SunsetPNG5",
            };
            skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);
            effect = Content.Load<Effect>("Reflection");
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
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            //draw skybox
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;
            skybox.Draw(view, projection, cameraPosition);

            //draw 3d model
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;
            DrawModelWithEffect();

            base.Draw(gameTime);
        }

        private void DrawModelWithEffect()
        {
            effect.CurrentTechnique = effect.Techniques[0];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        Matrix worldInverseTranspose = Matrix.Transpose(
                            Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);
                        effect.Parameters["DecalMap"].SetValue(texture);
                        effect.Parameters["EnvironmentMap"].SetValue(skybox.skyboxTexture);
                        effect.Parameters["Reflectivity"].SetValue(reflectivity);

                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, 
                            part.VertexOffset,
                            part.StartIndex, 
                            part.PrimitiveCount);
                    }
                }
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab04
{
    public class Lab04 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix world, view, projection;
        float angle = 0, angle2 = 0;
        float distance = 20, camX = 0, camY = 0;

        Model model;
        Effect effect;
        SpriteFont font;

        //Shader variabes
        Vector4 ambientColor = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.1f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 lightPosition = new Vector3(20, 20, 20);
        float diffuseIntensity = 1.0f;

        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        float specularIntensity = 1.0f;
        float shininess = 20f;
        Vector3 cameraPosition;

        //User control
        MouseState previousMouseState;

        public Lab04()
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

            model = Content.Load<Model>("Torus");
            effect = Content.Load<Effect>("SimpleShading");
            font = Content.Load<SpriteFont>("Font");

            effect.CurrentTechnique = effect.Techniques[1];

            world = Matrix.Identity;
            view = Matrix.CreateLookAt(
                new Vector3(0, 0, distance),
                new Vector3(),
                new Vector3(0, 1, 0)
                );
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f, 100
                );
        }
        
        protected override void UnloadContent()
        {
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState currentMouseState = Mouse.GetState();

            //camera control
            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Pressed)
            {
                angle += (previousMouseState.X - currentMouseState.X) / 100f;
                angle2 += (previousMouseState.Y - currentMouseState.Y) / 100f;
            }
            else if (currentMouseState.MiddleButton == ButtonState.Pressed &&
                previousMouseState.MiddleButton == ButtonState.Pressed)
            {
                camX += (previousMouseState.X - currentMouseState.X) / 100f;
                camY += (previousMouseState.Y - currentMouseState.Y) / 100f;
            }
            else if (currentMouseState.RightButton == ButtonState.Pressed &&
                previousMouseState.RightButton == ButtonState.Pressed)
            {
                distance -= (previousMouseState.Y - currentMouseState.Y) / 100f;
            }

            //shader switch
            if (Keyboard.GetState().IsKeyDown(Keys.D0) || Keyboard.GetState().IsKeyDown(Keys.NumPad0))
            {
                effect.CurrentTechnique = effect.Techniques[0];
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D1) || Keyboard.GetState().IsKeyDown(Keys.NumPad1))
            {
                effect.CurrentTechnique = effect.Techniques[1];
            }

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

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(
                            Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["AmbientColor"].SetValue(ambientColor);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);

                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);

                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
                        effect.Parameters["Shininess"].SetValue(shininess);

                        effect.Parameters["LightPosition"].SetValue(lightPosition);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);

                        pass.Apply(); //send data to GPU
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

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Light Position:" + lightPosition, Vector2.UnitX + Vector2.UnitY * 12, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
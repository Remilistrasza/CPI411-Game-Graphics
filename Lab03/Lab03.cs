using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab03
{
    public class Lab03 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix world, view, projection;
        float angle = 0, angle2 = 0;
        float distance = 10, camX = 0, camY = 0;

        Model model;
        Effect effect;

        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.1f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 lightPosition = new Vector3(1, 1, 1);
        float diffuseIntensity = 1.0f;

        MouseState previousMouseState;

        public Lab03()
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

            model = Content.Load<Model>("bunny");
            effect = Content.Load<Effect>("Diffuse");

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

            //camera position calculation
            Vector3 cameraPosition = Vector3.Transform(
                new Vector3(camX, camY, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            
            //update view matrix
            view = Matrix.CreateLookAt(
                cameraPosition,
                new Vector3(),
                new Vector3(0, 1, 0)
                );

            previousMouseState = Mouse.GetState();
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.BlendState = BlendState.Opaque;
            //GraphicsDevice.DepthStencilState = new DepthStencilState();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            /*
            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            model.Draw(world, view, projection);
            */
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
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(
                            Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["AmbientColor"].SetValue(ambient);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["DiffuseLightDirection"].SetValue(lightPosition);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);

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

            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab08
{
    public class Lab08 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //SpriteFont font;
        Effect effect1, effect2;
        bool effect = true;

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(
            new Vector3(0, 0, 20),
            new Vector3(0, 0, 0),
            Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45),
            800f / 600f,
            0.1f,
            100f);

        Vector3 cameraPosition, cameraTarget, lightPosition;
        Matrix lightView, lightProjection;

        float angle = 0;
        float angle2 = 0;
        float angleL = 0;
        float angleL2 = 0;
        float distance = 20;

        MouseState preMouse;
        Model model;
        Texture2D texture;

        public Lab08()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //font = Content.Load<SpriteFont>("");
            model = Content.Load<Model>("Plane");
            effect1 = Content.Load<Effect>("ProjectiveTexture");
            effect2 = Content.Load<Effect>("ShadowMap");
            texture = Content.Load<Texture2D>("nvlobby_new_negz");
        }
        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.F2)) effect = !effect;
                if (Keyboard.GetState().IsKeyDown(Keys.S)) { angle = angle2 = angleL = angleL2 = 0; distance = 30; cameraTarget = Vector3.Zero; }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                angle -= (Mouse.GetState().X - preMouse.X) / 100f;
                angle2 += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                distance += (Mouse.GetState().X - preMouse.X) / 100f;
            }

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 10f;
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 10f;
            }

            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget));
            lightPosition = Vector3.Transform(
                new Vector3(0, 0, 10),
                Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));
            lightView = Matrix.CreateLookAt(
                lightPosition, 
                Vector3.Zero,
                Vector3.Transform(
                    Vector3.UnitY, 
                    Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL)));
            lightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 1f, 50f);

            view = Matrix.CreateLookAt(
                cameraPosition,
                cameraTarget,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));

            preMouse = Mouse.GetState();

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (effect) drawEffect1();
            else drawEffect2();

            base.Draw(gameTime);
        }

        private void drawEffect1()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            effect1.CurrentTechnique = effect1.Techniques[0];

            foreach (EffectPass pass in effect1.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {

                        effect1.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect1.Parameters["View"].SetValue(view);
                        effect1.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect1.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect1.Parameters["LightViewMatrix"].SetValue(lightView);
                        effect1.Parameters["LightProjectionMatrix"].SetValue(lightProjection);
                        effect1.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect1.Parameters["LightPosition"].SetValue(lightPosition);
                        effect1.Parameters["ProjectiveTexture"].SetValue(texture);

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
        private void drawEffect2()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            effect1.CurrentTechnique = effect1.Techniques[0];

            foreach (EffectPass pass in effect2.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {

                        effect2.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect2.Parameters["View"].SetValue(view);
                        effect2.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect2.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect2.Parameters["LightViewMatrix"].SetValue(lightView);
                        effect2.Parameters["LightProjectionMatrix"].SetValue(lightProjection);
                        effect2.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect2.Parameters["LightPosition"].SetValue(lightPosition);

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

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
    class Skybox
    {
        Texture2D[] skyboxTextures;
        Model skyboxModel;
        Effect effect;

        public void LoadContent(ContentManager content, string assetName)
        {
            Effect effect = content.Load<Effect>("effects");
            skyboxModel = content.Load<Model>(assetName);

            skyboxTextures = new Texture2D[skyboxModel.Meshes.Count];
            int i = 0;
            foreach (ModelMesh mesh in skyboxModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    skyboxTextures[i++] = currentEffect.Texture;

            foreach (ModelMesh mesh in skyboxModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();
        }

        public void Draw(ref GraphicsDevice device, Camera gameCamera, Player player)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            device.DepthStencilState = dss;

            Matrix[] skyboxTransforms = new Matrix[skyboxModel.Bones.Count];
            skyboxModel.CopyAbsoluteBoneTransformsTo(skyboxTransforms);
            int i = 0;
            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(player.Position);
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(gameCamera.ViewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(gameCamera.ProjectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(skyboxTextures[i++]);
                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            device.DepthStencilState = dss;
        }
    }
}

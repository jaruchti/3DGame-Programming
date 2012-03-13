/*
 * 3D Game Programming Project
 * Dr. Liu
 * Zach Bates, Lauren Buss, Corey Darr, Jason Ruchti, Jared Tittle
 */

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
    /// <summary>
    /// Class which allows the client to display a skybox.
    /// </summary>
    /// <remarks>
    /// This is a singleton in the game.  The code is slightly modified from the Reimer's example, and
    /// is put in a seperate class to abstract the details from the main game.
    /// </remarks>
    class Skybox
    {
        Texture2D[] skyboxTextures; // textures to display around the skybox
        Model skyboxModel;
        Effect effect;

        /// <summary>
        /// Load the content required to display the skybox.
        /// </summary>
        /// <param name="content">Content pipeline for textures/effect</param>
        public void LoadContent(ContentManager content)
        {
            effect = content.Load<Effect>("Effects/effects");
            skyboxModel = content.Load<Model>("Skybox/skybox");

            skyboxTextures = new Texture2D[skyboxModel.Meshes.Count];

            // apply the textures to the model
            int i = 0;
            foreach (ModelMesh mesh in skyboxModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    skyboxTextures[i++] = currentEffect.Texture;

            // add the effect to the model
            foreach (ModelMesh mesh in skyboxModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();
        }


        /// <summary>
        /// Draw the skybox to the screen.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="gameCamera">Used to get the view and projection matrices</param>
        /// <param name="player">The player's position governs where the skybox is drawn</param>
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

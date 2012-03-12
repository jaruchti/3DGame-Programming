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
    class Map
    {
        private Texture2D scenaryTexture;
        private Texture2D roadTexture;
        private Texture2D cementTexture;
        private Effect effect;
        private int[,] floorPlan;
        private VertexBuffer cityVertexBuffer;
        private int[] buildingHeights = new int[] { 0, 2, 2, 6, 5, 4 };
        private BoundingBox[] buildingBoundingBoxes;
                    
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            effect = content.Load<Effect>("Effects/effects");
            scenaryTexture = content.Load<Texture2D>("Textures/texturemap");
            roadTexture = content.Load<Texture2D>("Textures/Road");
            cementTexture = content.Load<Texture2D>("Textures/RoadCement");

            LoadFloorPlan();
            SetUpVertices(ref device);
            SetUpBoundingBoxes();
        }

        private void LoadFloorPlan()
        {
            floorPlan = new int[,]
            {
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,0,1,1,1,0,1,0,1,1,1,0,1,1,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,0,1},
                {1,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,1},
                {1,1,1,1,0,1,1,1,0,1,0,1,1,1,0,1,1,1,1},
                {0,0,0,1,0,1,0,0,0,0,0,0,0,1,0,1,0,0,0},
                {1,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1},
                {1,1,1,1,0,0,0,1,0,0,0,1,0,0,0,1,1,1,1},
                {1,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1},
                {0,0,0,1,0,1,0,0,0,0,0,0,0,1,0,1,0,0,0},
                {1,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,0,1,1,1,0,1,0,1,1,1,0,1,1,0,1},
                {1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1},
                {1,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,1},
                {1,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,1},
                {1,0,1,1,1,1,1,1,0,1,0,1,1,1,1,1,1,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            };

            Random random = new Random();
            int differentBuildings = buildingHeights.Length - 1;
            for (int x = 0; x < floorPlan.GetLength(0); x++)
                for (int y = 0; y < floorPlan.GetLength(1); y++)
                    if (floorPlan[x, y] == 1)
                        floorPlan[x, y] = random.Next(differentBuildings) + 1;
        }

        private void SetUpVertices(ref GraphicsDevice device)
        {
            int cityWidth = floorPlan.GetLength(0);
            int cityLength = floorPlan.GetLength(1);
            int differentBuildings = buildingHeights.Length - 1;
            float imagesInTexture = 1 + differentBuildings * 2;

            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();
            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    int currentbuilding = floorPlan[x, z];

                    //floor or ceiling
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2(currentbuilding * 2 / imagesInTexture, 1)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 1)));

                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 1)));

                    if (currentbuilding != 0)
                    {
                        //front wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));

                        //back wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));

                        //left wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));

                        //right wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                    }
                }
            }

            cityVertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);
            cityVertexBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
        }

        private void SetUpBoundingBoxes()
        {
            int cityWidth = floorPlan.GetLength(0);
            int cityLength = floorPlan.GetLength(1);

            List<BoundingBox> bbList = new List<BoundingBox>();

            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    int buildingType = floorPlan[x, z];
                    if (buildingType != 0)
                    {
                        int buildingHeight = buildingHeights[buildingType];
                        Vector3[] buildingPoints = new Vector3[2];
                        buildingPoints[0] = new Vector3(x, 0, -z);
                        buildingPoints[1] = new Vector3(x + 1, buildingHeight, -z - 1);
                        BoundingBox buildingBox = BoundingBox.CreateFromPoints(buildingPoints);
                        bbList.Add(buildingBox);
                   }
                }
            }

            buildingBoundingBoxes = bbList.ToArray();
        }

        public GameConstants.CollisionType CheckCollision(BoundingSphere sphere)
        {
            for (int i = 0; i < buildingBoundingBoxes.Length; i++)
                if (buildingBoundingBoxes[i].Contains(sphere) != ContainmentType.Disjoint)
                    return GameConstants.CollisionType.Building;

            return GameConstants.CollisionType.None;
        }
        
        public void Draw(ref GraphicsDevice device, Camera gameCamera)
        {
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(gameCamera.ViewMatrix);
            effect.Parameters["xProjection"].SetValue(gameCamera.ProjectionMatrix);
            effect.Parameters["xTexture"].SetValue(scenaryTexture);
            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xLightDirection"].SetValue(GameConstants.LightDirection);
            effect.Parameters["xAmbient"].SetValue(0.5f);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.SetVertexBuffer(cityVertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, cityVertexBuffer.VertexCount / 3);
            }
        }

        public void DrawModel()
        {
        }
    }
}

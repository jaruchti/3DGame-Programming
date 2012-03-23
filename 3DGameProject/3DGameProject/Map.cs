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
    /// Class which defines the map which the game will be played within.
    /// </summary>
    /// <remarks>
    /// This is a singleton in the game.
    /// This class is modified from the Riemer's tuturial and placed in a seperate 
    /// file to aid in abstraction.
    /// </remarks>
    public class Map
    {
        private Texture2D scenaryTexture;               // building textures
        private Effect effect;

        private int[,] floorPlan;                       // arrangement for the buildings
        private Fuel[] fuelBarrels;                     // fuel for the player to pick up
        private Bonus[] bonuses;                        // bonuses for the player to pick up
        private VertexBuffer cityVertexBuffer;          // holds the vertices to draw the city
        private int[] buildingHeights = new int[] { 0, 2, 2, 6, 5, 4 }; 
        private BoundingBox[] buildingBoundingBoxes;    // bounding boxes for the building in the map

        /// <summary>
        /// Property to allow the client to get read-only access to the map's floor plan.
        /// </summary>
        public int[,] FloorPlan { get { return floorPlan; } }

        /// <summary>
        /// Property to allow the client to get read-only access to the fuel objects in the map.
        /// </summary>
        public Fuel[] FuelBarrels { get { return fuelBarrels; } }

        /// <summary>
        /// Property to allow the client to get read-only access to the bonus objects in the map.
        /// </summary>
        public Bonus[] Bonuses { get { return bonuses; } }
      
        /// <summary>
        /// Load the content necessary to create the map
        /// </summary>
        /// <param name="device">Graphics card</param>
        /// <param name="content">Content pipeline</param>
        public void LoadContent(ref GraphicsDevice device, ContentManager content)
        {
            effect = content.Load<Effect>("Effects/effects");
            scenaryTexture = content.Load<Texture2D>("Textures/texturemap");

            LoadFloorPlan();
            LoadFuel(content);
            LoadBonuses(content);
            SetUpVertices(ref device);
            SetUpBoundingBoxes();
        }

        /// <summary>
        /// Create the floor plan for the game.
        /// </summary>
        /// <remarks>
        /// Each position in the floorPlan maps directly to an x, -z position in the 
        /// game.  If the floor plan contains a zero, this specifies there is not
        /// a building in this location.  Otherwise, the floor plan contains the height
        /// of the building at that point.
        /// </remarks>
        private void LoadFloorPlan()
        {
            // Initial floor plan, specify which positions will have buildings
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
                {0,0,0,1,0,1,0,1,1,0,1,1,0,1,0,1,0,0,0},
                {0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0},
                {0,0,0,1,0,1,0,1,1,1,1,1,0,1,0,1,0,0,0},
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

            // Now, for those locations with buildings, determine the building height
            Random random = new Random();
            int differentBuildings = buildingHeights.Length - 1;
            for (int x = 0; x < floorPlan.GetLength(0); x++)
            {
                for (int y = 0; y < floorPlan.GetLength(1); y++)
                {
                    if (floorPlan[x, y] == 1)
                        floorPlan[x, y] = random.Next(differentBuildings) + 1;
                }
            }    
        }

        /// <summary>
        /// Load the fuel barrel models and place them in the proper position
        /// </summary>
        /// <param name="content">Content pipeline (for models)</param>
        private void LoadFuel(ContentManager content)
        {
            fuelBarrels = new Fuel[4];

            // load models
            for (int i = 0; i < fuelBarrels.Length; i++)
            {
                fuelBarrels[i] = new Fuel();
                fuelBarrels[i].LoadContent(content);
            }

            // put the fuel objects in the proper position in the map
            fuelBarrels[0].UpdatePositionAndBoundingSphere(new Vector3(2.5f, 0, -1.5f));
            fuelBarrels[1].UpdatePositionAndBoundingSphere(new Vector3(15.5f, 0, -1.5f));
            fuelBarrels[2].UpdatePositionAndBoundingSphere(new Vector3(2.5f, 0, -17.5f));
            fuelBarrels[3].UpdatePositionAndBoundingSphere(new Vector3(15.5f, 0, -17.5f));
        }

        /// <summary>
        /// Load the bonus models and place the bonuses in random locations
        /// </summary>
        /// <param name="content">Content pipeline (for models)</param>
        private void LoadBonuses(ContentManager content)
        {
            bonuses = new Bonus[5];

            // load models
            for (int i = 0; i < bonuses.Length; i++)
            {
                bonuses[i] = new Bonus();
                bonuses[i].LoadContent(content);
            }

            // place the bonuses in random locations
            foreach (Bonus b in bonuses)
                b.PlaceRandomly(this);
        }

        /// <summary>
        /// Setup the cityVertexBuffer to hold the information needed to draw the map 
        /// </summary>
        /// <param name="device">Graphics card</param>
        private void SetUpVertices(ref GraphicsDevice device)
        {
            int cityWidth = floorPlan.GetLength(0);
            int cityLength = floorPlan.GetLength(1);
            int differentBuildings = buildingHeights.Length - 1;    // number of building types
            float imagesInTexture = 1 + differentBuildings * 2;     // images in scenaryTexture

            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();

            // Loop through the floor plan and create the buildings and roads 
            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    int currentbuilding = floorPlan[x, z];

                    //create a road or ceiling
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2(currentbuilding * 2 / imagesInTexture, 1)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 1)));

                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, buildingHeights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 1)));

                    if (currentbuilding != 0) // create a building
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

            // Setup the vertex buffer for the city
            cityVertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);
            cityVertexBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
        }

        /// <summary>
        /// Setup bounding boxes for each of the buildings in the city.
        /// </summary>
        private void SetUpBoundingBoxes()
        {
            int cityWidth = floorPlan.GetLength(0);
            int cityLength = floorPlan.GetLength(1);

            List<BoundingBox> bbList = new List<BoundingBox>();

            // Loop through the floor plan and check for buildings
            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    int buildingType = floorPlan[x, z];

                    if (buildingType != 0)
                    {
                        // there is a building in this position
                        int buildingHeight = buildingHeights[buildingType];

                        Vector3[] buildingPoints = new Vector3[2];
                        buildingPoints[0] = new Vector3(x, 0, -z);  // lower left of the building
                        buildingPoints[1] = new Vector3(x + 1, buildingHeight, -z - 1); // upper right of building
                        BoundingBox buildingBox = BoundingBox.CreateFromPoints(buildingPoints); // create bounding box from lower left and upper right of building
                        bbList.Add(buildingBox);
                   }
                }
            }

            // Convert the bounding box list to an array for greater efficiency
            buildingBoundingBoxes = bbList.ToArray();
        }

        /// <summary>
        /// Check for collisions with any of the parts of the map.
        /// </summary>
        /// <param name="sphere">BoundingSphere for the object we are checking collisions for</param>
        /// <returns>
        /// A collision type enumeration type with the type of the collision
        /// (e.g. Building, Fuel, None).
        /// </returns>
        public GameConstants.CollisionType CheckCollision(BoundingSphere sphere)
        {
            // Check for collisions with the buildings
            for (int i = 0; i < buildingBoundingBoxes.Length; i++)
                if (buildingBoundingBoxes[i].Contains(sphere) != ContainmentType.Disjoint)
                    return GameConstants.CollisionType.Building;

            // Check for collisions with the fuel barrels
            for (int i = 0; i < fuelBarrels.Length; i++)
            {
                if (fuelBarrels[i].BoundingSphere.Contains(sphere) != ContainmentType.Disjoint)
                    return GameConstants.CollisionType.Fuel;
            }

            // Check for collision with the bonuses
            for (int i = 0; i < bonuses.Length; i++)
            {
                if (bonuses[i].BoundingSphere.Contains(sphere) != ContainmentType.Disjoint)
                {
                    bonuses[i].PlaceRandomly(this); // place the barrier in a new random location
                    return GameConstants.CollisionType.Bonus;
                }
            }

            return GameConstants.CollisionType.None;
        }

        /// <summary>
        /// Draw the map on the display.
        /// </summary>
        /// <param name="device">Graphics card (to draw the vertices)</param>
        /// <param name="gameCamera">For view and projection martices</param>
        /// <param name="gameState">For the state of the game</param>
        /// <remarks>The bonuses are not drawn if the game is still in the intro state</remarks>
        public void Draw(ref GraphicsDevice device, Camera gameCamera, GameConstants.GameState gameState)
        {
            DrawCity(ref device, gameCamera);
            DrawFuelBarrels(gameCamera);

            if (gameState != GameConstants.GameState.Intro)
                DrawBonuses(gameCamera);
        }

        /// <summary>
        /// Draw the city
        /// </summary>
        /// <param name="device">Graphics card to use when drawing primitives</param>
        /// <param name="gameCamera">For view and projection matrices</param>
        private void DrawCity(ref GraphicsDevice device, Camera gameCamera)
        {
            // Setup the details of the effect
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(gameCamera.ViewMatrix);
            effect.Parameters["xProjection"].SetValue(gameCamera.ProjectionMatrix);
            effect.Parameters["xTexture"].SetValue(scenaryTexture);
            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xLightDirection"].SetValue(GameConstants.LightDirection);
            effect.Parameters["xAmbient"].SetValue(0.5f); // light side of building opposite LightDirection

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                // Draw the triangles.  Each triangle has three vertices (hence cityVertexBuffer.VertexCount / 3)
                // as last parameter to device.DrawPrimities(...).
                device.SetVertexBuffer(cityVertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, cityVertexBuffer.VertexCount / 3); 
            }
        }

        /// <summary>
        /// Draw the fuel barrels
        /// </summary>
        /// <param name="gameCamera">For view and projection matrices</param>
        private void DrawFuelBarrels(Camera gameCamera)
        {
            foreach (Fuel f in fuelBarrels)
                f.Draw(gameCamera);
        }

        /// <summary>
        /// Draw the bonus models
        /// </summary>
        /// <param name="gameCamera">For view and projection matrices</param>
        private void DrawBonuses(Camera gameCamera)
        {
            foreach (Bonus b in bonuses)
                b.Draw(gameCamera);
        }
    }
}

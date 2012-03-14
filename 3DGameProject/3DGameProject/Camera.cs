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
    /// Class which implements the logic for the camera.
    /// </summary>
    /// <remarks>This is a singleton in the game</remarks>
    public class Camera
    {
        private const float NearClip = 0.2f;
        private const float FarClip = 500.0f;
        private const float ViewAngle = 45.0f;

        /// <summary>Default y-offset from the player's position to the camera</summary>
        private const float DefaultAvatarYOffset = 0.15f;
        /// <summary>Default z-offset from the player's position to the camera</summary>
        private const float DefaultAvataryZOffset = 0.6f;

        private Vector3 avatarHeadOffset;       // offset of the camera from the player's position
        private Vector3 targetOffset;           // offset from the player's position of the camera's target
        private float cameraRotation = 0.0f;    // camera rotation about y axis

        /// <summary>
        /// Offset of the camera from the player's position.
        /// </summary>
        /// <remarks>
        /// Wrap member variable as property to allow for changes 
        /// later while keeping interface the same
        /// </remarks>
        public Vector3 AvatarHeadOffset
        {
            get { return avatarHeadOffset; }
            set { avatarHeadOffset = value; }
        }

        /// <summary>
        /// Offset from the player's position of the camera's target
        /// </summary>
        /// <remarks>
        /// Wrap member variable as property to allow for changes 
        /// later while keeping interface the same
        /// </remarks>
        public Vector3 TargetOffset 
        {
            get { return targetOffset; }
            set { avatarHeadOffset = value; }
        }

        /// <summary>
        /// Defines the position and orientation of the camera
        /// </summary>
        public Matrix ViewMatrix { get; set; }

        /// <summary>
        /// Defines the viewing angle, aspect ratio, near clip and far clip of the camera.
        /// </summary>
        public Matrix ProjectionMatrix { get; set; }

        /// <summary>
        /// Allows the client to create a new camera
        /// </summary>
        public Camera()
        {
            avatarHeadOffset = new Vector3(0.0f, DefaultAvatarYOffset, DefaultAvataryZOffset);
            targetOffset = new Vector3(0, 0, 0);
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Update the camera's view and projection matrices.
        /// </summary>
        /// <param name="avatarYaw">The direction the player is pointed</param>
        /// <param name="position">The position of the player</param>
        /// <param name="floorPlan">The layout of the map</param>
        /// <param name="aspectRatio">The viewport aspect ratio</param>
        public void Update(float avatarYaw, Vector3 position, int[,] floorPlan, float aspectRatio)
        {
            Matrix rotationMatrix;              // matrix with the camera rotation
            Vector3 transformedheadOffset;      // headOffset with camera rotation
            Vector3 transformedReference;       // targetOffset with camera rotation
            Vector3 cameraPosition;             // position of the camera
            Vector3 cameraTarget;               // camera target

            // delay the camera rotation
            cameraRotation = MathHelper.Lerp(cameraRotation, avatarYaw, 0.1f);     
            
            rotationMatrix = Matrix.CreateRotationY(cameraRotation);

            transformedheadOffset = Vector3.Transform(AvatarHeadOffset, rotationMatrix);
            transformedReference = Vector3.Transform(TargetOffset, rotationMatrix);

            cameraPosition = position + transformedheadOffset;
            cameraTarget = position + transformedReference;

            while (floorPlan[(int)(cameraPosition.X), (int)(-cameraPosition.Z)] != 0)
            {
                // The camera position is not within the map, shorten the distance to the
                // player and move the camera's target in front of the car

                avatarHeadOffset.Z -= 1 / 600f;
                avatarHeadOffset.Y += 1 / 1200f;
                targetOffset.Z -= 1 / 600f;

                transformedheadOffset = Vector3.Transform(AvatarHeadOffset, rotationMatrix);
                transformedReference = Vector3.Transform(TargetOffset, rotationMatrix);

                cameraPosition = position + transformedheadOffset;
                cameraTarget = position + transformedReference;
            }

            while (floorPlan[(int)cameraPosition.X, (int)-cameraPosition.Z] == 0 &&
                avatarHeadOffset.Z < DefaultAvataryZOffset)
            {
                // The camera's position used to not be within the map.  However, now it is
                // within the map.  Move the camera's position as close as possible to the default

                avatarHeadOffset.Z += 1 / 600f;
                avatarHeadOffset.Y -= 1 / 1200f;
                targetOffset.Z += 1 / 600f;

                transformedheadOffset = Vector3.Transform(AvatarHeadOffset, rotationMatrix);
                transformedReference = Vector3.Transform(TargetOffset, rotationMatrix);

                cameraPosition = position + transformedheadOffset;
                cameraTarget = position + transformedReference;
            }

            //Calculate the camera's view and projection 
            //matrices based on current values.
            ViewMatrix =
                Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            ProjectionMatrix =
                Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(ViewAngle), aspectRatio, NearClip, FarClip);
        }
    }
}

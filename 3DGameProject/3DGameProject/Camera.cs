using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _3DGameProject
{
    class Camera
    {
        private Vector3 avatarHeadOffset;
        private Vector3 targetOffset;
        private float cameraRotation = 0.0f;
        private int[,] floorPlan;

        public Vector3 AvatarHeadOffset
        {
            get { return avatarHeadOffset; }
            set { avatarHeadOffset = value; }
        }

        public Vector3 TargetOffset 
        {
            get { return targetOffset; }
            set { avatarHeadOffset = value; }
        }

        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }

        public Camera()
        {
            avatarHeadOffset = new Vector3(0.0f, 0.15f, 0.6f);
            targetOffset = new Vector3(0, 0, 0);
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;
        }

        public void LoadFloorPlan(int[,] floorPlan)
        {
            this.floorPlan = floorPlan;
        }

        public void ViewTopDown(float aspectRatio)
        {
            ViewMatrix =
                Matrix.CreateLookAt(new Vector3(19 / 2.0f, 20.0f, -10.0f),
                                    new Vector3(19 / 2.0f, 0.0f, -10.0f),
                                    new Vector3(0, 0, 1));
            ProjectionMatrix =
                Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(GameConstants.ViewAngle), aspectRatio,
                    GameConstants.NearClip, GameConstants.FarClip);
        }

        public void Update(float avatarYaw, Vector3 position, float aspectRatio)
        {
            Matrix rotationMatrix;
            Vector3 transformedheadOffset;
            Vector3 transformedReference;
            Vector3 cameraPosition;
            Vector3 cameraTarget;

            cameraRotation = MathHelper.Lerp(cameraRotation, avatarYaw, 0.1f);
            
            rotationMatrix = Matrix.CreateRotationY(cameraRotation);

            transformedheadOffset = Vector3.Transform(AvatarHeadOffset, rotationMatrix);
            transformedReference = Vector3.Transform(TargetOffset, rotationMatrix);

            cameraPosition = position + transformedheadOffset;
            cameraTarget = position + transformedReference;

            while (floorPlan[(int)(cameraPosition.X), (int)(-cameraPosition.Z)] != 0)
            {
                avatarHeadOffset.Z -= 1 / 600f;
                avatarHeadOffset.Y += 1 / 1200f;
                targetOffset.Z -= 1 / 600f;

                transformedheadOffset = Vector3.Transform(AvatarHeadOffset, rotationMatrix);
                transformedReference = Vector3.Transform(TargetOffset, rotationMatrix);

                cameraPosition = position + transformedheadOffset;
                cameraTarget = position + transformedReference;
            }

            while (floorPlan[(int)cameraPosition.X, (int)-cameraPosition.Z] == 0 &&
                avatarHeadOffset.Z < 0.6f)
            {
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
                    MathHelper.ToRadians(GameConstants.ViewAngle), aspectRatio,
                    GameConstants.NearClip, GameConstants.FarClip);
        }
    }
}

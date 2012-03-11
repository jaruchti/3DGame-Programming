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
        public Vector3 AvatarHeadOffset { get; set; }
        public Vector3 TargetOffset { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }

        public Camera()
        {
            AvatarHeadOffset = new Vector3(0.0f, 0.1f, 0.5f);
            TargetOffset = new Vector3(0, 0, 0);
            ViewMatrix = Matrix.CreateLookAt(new Vector3(20, 13, -5), new Vector3(8, 0, -7), new Vector3(0, 1, 0));
            ProjectionMatrix = Matrix.Identity;
        }

        public void Update(float avatarYaw, Vector3 position, float aspectRatio)
        {
            Matrix rotationMatrix = Matrix.CreateRotationY(avatarYaw);

            Vector3 transformedheadOffset =
                Vector3.Transform(AvatarHeadOffset, rotationMatrix);
            Vector3 transformedReference =
                Vector3.Transform(TargetOffset, rotationMatrix);

            Vector3 cameraPosition = position + transformedheadOffset;
            Vector3 cameraTarget = position + transformedReference;

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

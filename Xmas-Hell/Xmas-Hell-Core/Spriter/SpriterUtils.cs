using System;
using Microsoft.Xna.Framework;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using System.Linq;
using System.IO;
using XmasHell.Extensions;

namespace XmasHell.Spriter
{
    static class SpriterUtils
    {
        public static SpriterObject GetSpriterFileData(string spritePartFileName, MonoGameAnimator animator)
        {
            if (animator.FrameData == null)
                return null;

            int folderId;
            var spriterFile = GetSpriterFile(spritePartFileName, animator, out folderId);
            var spriteData = animator.FrameData.SpriteData;
            var fileSpriteDataFound = spriteData.FindAll(so => so.FolderId == folderId && so.FileId == spriterFile.Id);

            if (fileSpriteDataFound.Count == 0)
                return null;

            return fileSpriteDataFound.First();
        }

        public static Vector2 GetSpriterFilePosition(string spritePartFileName, MonoGameAnimator animator)
        {
            if (animator.FrameData == null)
                return Vector2.Zero;

            int folderId;
            var spriterFile = GetSpriterFile(spritePartFileName, animator, out folderId);
            var spriteData = animator.FrameData.SpriteData;
            var fileSpriteDataFound = spriteData.FindAll(so => so.FolderId == folderId && so.FileId == spriterFile.Id);

            if (fileSpriteDataFound.Count == 0)
                return Vector2.Zero;

            var fileSpriteData = fileSpriteDataFound.First();
            return new Vector2(fileSpriteData.X, -fileSpriteData.Y);
        }

        public static Point GetSpriterFileSize(string spritePartFileName, MonoGameAnimator animator)
        {
            foreach (var folder in animator.Entity.Spriter.Folders)
            {
                var spriterFileId = Array.FindIndex(folder.Files, (file) => Path.GetFileName(file.Name) == spritePartFileName);

                if (spriterFileId != -1)
                {
                    var spriteBodyPart = folder.Files[spriterFileId];

                    return new Point(spriteBodyPart.Width, spriteBodyPart.Height);
                }
            }

            return Point.Zero;
        }

        public static SpriterFile GetSpriterFileStaticData(string spritePartFileName, MonoGameAnimator animator)
        {
            foreach (var folder in animator.Entity.Spriter.Folders)
            {
                var spriterFileId = Array.FindIndex(folder.Files, (file) => Path.GetFileName(file.Name) == spritePartFileName);

                if (spriterFileId != -1)
                {
                    return folder.Files[spriterFileId];
                }
            }

            return null;
        }

        // To make folderId parameter optional
        public static SpriterFile GetSpriterFile(string spritePartFileName, MonoGameAnimator animator)
        {
            int folderId = 0;
            return GetSpriterFile(spritePartFileName, animator, out folderId);
        }

        public static SpriterFile GetSpriterFile(string spritePartFileName, MonoGameAnimator animator, out int folderId)
        {
            if (animator == null)
                throw new ArgumentException("Please make sure that a Spriter animator is linked to the given entity.");

            foreach (var folder in animator.Entity.Spriter.Folders)
            {
                var spriterFileId = Array.FindIndex(folder.Files, (file) => Path.GetFileName(file.Name) == spritePartFileName);

                folderId = folder.Id;

                if (spriterFileId != -1)
                    return folder.Files[spriterFileId];
            }

            throw new ArgumentException("Please make sure that the given Spriter entity has a sprite part named: " + spritePartFileName);
        }

        public static SpriterFile GetSpriterFile(SpriterObject spriterObject, MonoGameAnimator animator)
        {
            if (animator == null)
                throw new ArgumentException("Please make sure that a Spriter animator is linked to the given entity.");

            foreach (var folder in animator.Entity.Spriter.Folders)
            {
                var spriterFileId = Array.FindIndex(folder.Files, (file) => file.Id == spriterObject.FileId);

                if (spriterFileId != -1)
                    return folder.Files[spriterFileId];
            }

            throw new ArgumentException("Please make sure that the given Spriter entity has a sprite part with file ID: " + spriterObject.FileId);
        }

        public static Vector2 GetSpriterWorldPosition(SpriterObject spriterObject, MonoGameAnimator animator)
        {
            var worldPosition = animator.Position;

            if (animator.FrameData != null)
            {
                // Compute translation
                var animationOffset = new Vector2(spriterObject.X, -spriterObject.Y);
                var scale = new Vector2(spriterObject.ScaleX, spriterObject.ScaleY);
                var realPivotPosition = new Vector2(1 - spriterObject.PivotX, 1 - spriterObject.PivotY);

                // TOFIX...
                realPivotPosition = new Vector2(0f, 1f);

                var spriterFile = GetSpriterFile(spriterObject, animator);

                var spriteCenter = new Vector2(
                    spriterFile.Width * realPivotPosition.X,
                    spriterFile.Height * realPivotPosition.Y
                );
                var worldTopLeftCornerPosition = animator.Position - (spriteCenter * scale);

                worldPosition = worldTopLeftCornerPosition + animationOffset;

                // Compute rotation
                var pivot = new Vector2(
                    (spriterFile.Width * realPivotPosition.X) + spriterObject.X,
                    (spriterFile.Height * realPivotPosition.Y) - spriterObject.Y
                );

                var origin = animator.Position + pivot - spriteCenter;

                var rotation = -spriterObject.Angle;
                rotation = MathHelper.WrapAngle(MathHelper.ToRadians(rotation));

                // Take the animation angle into account
                worldPosition = MathExtension.RotatePoint(
                    worldPosition, rotation, origin
                );

                // Take the global angle into account
                worldPosition = MathExtension.RotatePoint(
                    worldPosition, animator.Rotation, animator.Position
                );
            }

            return worldPosition;
        }

        public static Vector2 GetWorldPosition(string spriterPartFilename, MonoGameAnimator animator, Vector2? optionalLocalPosition = null)
        {
            var vertex = optionalLocalPosition ?? Vector2.Zero;
            var worldPosition = animator.Position + vertex;

            if (animator.FrameData != null)
            {
                // Compute translation
                int folderId = 0;
                var spriterPartFile = SpriterUtils.GetSpriterFile(spriterPartFilename, animator, out folderId);

                var spriteData = animator.FrameData.SpriteData.Find((so) => so.FileId == spriterPartFile.Id);
                var animationOffset = new Vector2(spriteData.X, -spriteData.Y);
                var scale = new Vector2(spriteData.ScaleX, spriteData.ScaleY);
                var realPivotPosition = new Vector2(1 - spriteData.PivotX, 1 - spriteData.PivotY);

                vertex *= scale;

                var spriteCenter = new Vector2(
                    spriterPartFile.Width * realPivotPosition.X,
                    spriterPartFile.Height * realPivotPosition.Y
                );
                var worldTopLeftCornerPosition = animator.Position;// - (spriteCenter * scale);

                worldPosition = worldTopLeftCornerPosition + vertex + animationOffset;

                // Compute rotation
                var pivot = new Vector2(
                    (spriterPartFile.Width * realPivotPosition.X) + spriteData.X,
                    (spriterPartFile.Height * realPivotPosition.Y) - spriteData.Y
                );

                var origin = animator.Position + pivot - spriteCenter;

                var rotation = -spriteData.Angle;
                rotation = MathHelper.WrapAngle(MathHelper.ToRadians(rotation));

                // Take the animation angle into account
                worldPosition = MathExtension.RotatePoint(
                    worldPosition, rotation, origin
                );

                // Take the global angle into account
                worldPosition = MathExtension.RotatePoint(
                    worldPosition, animator.Rotation, animator.Position
                );
            }

            return worldPosition;
        }
    }
}

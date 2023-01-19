using T4.Globals;
using UnityEngine;

namespace T4.Utils
{
    public static class RectUtils
    {
        private static Texture2D whiteTexture;
        public static Texture2D WhiteTexture
        {
            get
            {
                if (whiteTexture == null)
                {
                    whiteTexture = new Texture2D(1, 1);
                    whiteTexture.SetPixel(0, 0, Color.white);
                    whiteTexture.Apply();
                }

                return whiteTexture;
            }
        }

        public static void DrawScreenRect(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, WhiteTexture);
            GUI.color = Color.white;
        }

        public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
        {
            // Top
            RectUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // Left
            RectUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // Right
            RectUtils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
            // Bottom
            RectUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        }

        public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
        {
            // Move origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;
            // Calculate corners
            Vector3 topLeft = Vector3.Min(screenPosition1, screenPosition2);
            Vector3 bottomRight = Vector3.Max(screenPosition1, screenPosition2);
            // Create Rect
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }
        public static Bounds ScreenToViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
        {
            Vector3 v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
            Vector3 v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
            Vector3 min = Vector3.Min(v1, v2);
            Vector3 max = Vector3.Max(v1, v2);
            min.z = camera.nearClipPlane;
            max.z = camera.farClipPlane;

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }
        public static Bounds WorldToViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
        {
            Vector3 v1 = Camera.main.WorldToViewportPoint(screenPosition1);
            Vector3 v2 = Camera.main.WorldToViewportPoint(screenPosition2);
            Vector3 min = Vector3.Min(v1, v2);
            Vector3 max = Vector3.Max(v1, v2);
            min.z = camera.nearClipPlane;
            max.z = camera.farClipPlane;

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        public static Bounds RectToBounds(Vector3 po1, Vector3 pos2)
        {
            Vector3 min = Vector3.Min(po1, pos2);
            Vector3 max = Vector3.Max(po1, pos2);
            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        public static Bounds RectToBounds(Rect rect)
        {
            Vector3 min = Vector3.Min(rect.min, rect.max);
            Vector3 max = Vector3.Max(rect.min, rect.max);
            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        private static Vector2 WorldToGUIPoint(Camera camera, Vector3 world)
        {
            Vector2 screenPoint = camera.WorldToScreenPoint(world);
            screenPoint.y = Screen.height - screenPoint.y;
            return screenPoint;
        }

        public static Rect WorldBoundsToScreenRect(Camera camera, Bounds bounds)
        {
            // get the 8 vertices of the bounding box
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            Vector2[] extentPoints = new Vector2[]
             {
                WorldToGUIPoint(camera, new Vector3(center.x-extents.x, center.y-extents.y, center.z-extents.z)),
                WorldToGUIPoint(camera, new Vector3(center.x+extents.x, center.y-extents.y, center.z-extents.z)),
                WorldToGUIPoint(camera, new Vector3(center.x-extents.x, center.y-extents.y, center.z+extents.z)),
                WorldToGUIPoint(camera, new Vector3(center.x+extents.x, center.y-extents.y, center.z+extents.z)),
                WorldToGUIPoint(camera, new Vector3(center.x-extents.x, center.y+extents.y, center.z-extents.z)),
                WorldToGUIPoint(camera, new Vector3(center.x+extents.x, center.y+extents.y, center.z-extents.z)),
                WorldToGUIPoint(camera, new Vector3(center.x-extents.x, center.y+extents.y, center.z+extents.z)),
                WorldToGUIPoint(camera, new Vector3(center.x+extents.x, center.y+extents.y, center.z+extents.z))
             };
            Vector2 min = extentPoints[0];
            Vector2 max = extentPoints[0];
            foreach (Vector2 v in extentPoints)
            {
                min = Vector2.Min(min, v);
                max = Vector2.Max(max, v);
            }
            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }
        public static Vector3[] ScreenCornersToWorldPoints()
        {
            return ScreenCornersToWorld(Camera.main);
        }
        public static Vector3[] ScreenCornersToWorld(Camera cam)
        {
            Vector3[] corners = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                Ray ray = cam.ScreenPointToRay(new Vector2((i % 2) * Screen.width, (i / 2) * Screen.height));
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMaskValues.FlatTerrainLayer))
                {
                    corners[i] = hit.point;
                }
            }
            return corners;
        }
        public static Vector3 MiddleOfScreenPointToWorld()
        {
            return MiddleOfScreenPointToWorld(Camera.main);
        }
        public static Vector3 MiddleOfScreenPointToWorld(Camera cam)
        {
            Ray _ray = cam.ScreenPointToRay(0.5f * new Vector2(Screen.width, Screen.height));
            if (Physics.Raycast(_ray, out RaycastHit _hit, 1000f, LayerMaskValues.FlatTerrainLayer))
            {
                return _hit.point;
            }

            return Vector3.zero;
        }


        public static bool IsRectInsideViewPort(Rect rect)
        {
            return IsRectInsideViewPort(rect, Camera.main);
        }
        public static bool IsRectInsideViewPort(Rect rect, Camera camera)
        {
            Vector2[] vertices =  
            {
                new Vector2(rect.xMin, rect.yMin),
                new Vector2(rect.xMin, rect.yMax),
                new Vector2(rect.xMax, rect.yMax),
                new Vector2(rect.xMax, rect.yMin),
            };
            int verticesInside = 0;
            foreach(Vector2 v in vertices)
            {
                Vector2 i = camera.ScreenToViewportPoint(v);
                if (i.x >= 0 && i.y >= 0 && i.x <= 1 && i.y <= 1) verticesInside++;
            }

            return verticesInside == 4;
        }

        public static bool IsValidSelection(Rect unit, Rect selection)
        {
            return IsValidSelection(unit, selection, Camera.main);
        }
        public static bool IsValidSelection(Rect unit, Rect selection, Camera camera)
        {
            return selection.Overlaps(unit) && IsRectInsideViewPort(unit, camera);
        }
    }
}
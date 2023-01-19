using System.Linq;
using T4.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace T4.Minimap
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class MinimapIndicator : UIBehaviour
    {
        private MeshFilter meshFilter;
        private Mesh mesh;

        [SerializeField]
        private float strokeWidth;

        protected override void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            mesh = CreateIndicatorMesh();

            meshFilter.mesh = mesh;
        }

        private Mesh CreateIndicatorMesh()
        {
            Mesh m = new();

            Vector3[] vertices = new Vector3[] {
                Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero,
                Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero
            };
            int[] triangles = new int[] {
                0, 4, 1, 4, 5, 1,
                0, 2, 6, 6, 4, 0,
                6, 2, 7, 2, 3, 7,
                5, 7, 3, 3, 1, 5
            };
            m.vertices = vertices;
            m.triangles = triangles;
            return m;
        }

        public void ComputeIndicatorMesh()
        {
            Vector3 middle = RectUtils.MiddleOfScreenPointToWorld();
            Vector3[] corners = RectUtils.ScreenCornersToWorldPoints();
            float w = corners[1].x - corners[0].x;
            float h = corners[2].z - corners[0].z;
            float wratio = (w - strokeWidth) / w;
            float hratio = (h - strokeWidth) / h;

            Vector3[] innerCorners = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                corners[i] = corners[i] - middle;
                innerCorners[i].x = corners[i].x * wratio;
                innerCorners[i].z = corners[i].z * hratio;

                corners[i].y = 10f;
                innerCorners[i].y = 10f;
            }

            mesh.vertices = corners.Concat(innerCorners).ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        public void TranslateToCenter()
        {
            Vector3 middle = RectUtils.MiddleOfScreenPointToWorld();
            transform.position = middle;
        }
    }
}

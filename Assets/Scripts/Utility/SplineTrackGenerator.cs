using System.Collections.Generic;
using UnityEngine;

namespace TootRaftRush.Utility
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SplineTrackGenerator : MonoBehaviour
    {
        [Header("Waypoints")]
        [SerializeField] private List<Vector3> controlPoints = new List<Vector3>();
        [SerializeField] private int curveSubdivisions = 16;

        [Header("Track Dimensions")]
        [SerializeField] private float trackWidth = 7.0f;
        [SerializeField] private float channelDepth = 1.8f;
        [SerializeField] private float railRadius = 0.25f;

        [Header("Materials")]
        [SerializeField] private Material waterSlideMaterial;

        [Header("Level Generation Presets")]
        [SerializeField] private int levelPreset = 1; // 1: Sunny Slide, 2: Spiky Canyon, 3: Turbo Rapids
        [SerializeField] private bool autoGenerateOnStart = true;

        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
        }

        private void Start()
        {
            if (autoGenerateOnStart)
            {
                if (controlPoints.Count < 4)
                {
                    SetupPresetPoints(levelPreset);
                }
                GenerateTrackMesh();
            }
        }

        public void SetupPresetPoints(int preset)
        {
            controlPoints.Clear();
            float length = 180f;

            switch (preset)
            {
                case 1: // Sunny Slide — gentle curves, easy downhill
                    controlPoints.Add(new Vector3(0, 25, 0));
                    controlPoints.Add(new Vector3(0, 20, 40));
                    controlPoints.Add(new Vector3(15, 14, 90));
                    controlPoints.Add(new Vector3(-12, 8, 140));
                    controlPoints.Add(new Vector3(0, 2, 190));
                    controlPoints.Add(new Vector3(0, 0, 240));
                    break;

                case 2: // Spiky Canyon — sharper S-turns and canyon drops
                    controlPoints.Add(new Vector3(0, 35, 0));
                    controlPoints.Add(new Vector3(-25, 28, 45));
                    controlPoints.Add(new Vector3(30, 20, 100));
                    controlPoints.Add(new Vector3(-35, 12, 160));
                    controlPoints.Add(new Vector3(18, 5, 220));
                    controlPoints.Add(new Vector3(0, 0, 280));
                    break;

                case 3: // Turbo Rapids — steep drops, rapid zig-zags, high speed
                    controlPoints.Add(new Vector3(0, 50, 0));
                    controlPoints.Add(new Vector3(20, 38, 50));
                    controlPoints.Add(new Vector3(-30, 26, 110));
                    controlPoints.Add(new Vector3(40, 16, 170));
                    controlPoints.Add(new Vector3(-25, 6, 240));
                    controlPoints.Add(new Vector3(0, 0, 320));
                    break;
            }
        }

        public void GenerateTrackMesh()
        {
            if (controlPoints == null || controlPoints.Count < 2) return;

            if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();

            List<Vector3> splinePoints = SampleSpline(controlPoints, curveSubdivisions);
            int numRings = splinePoints.Count;
            if (numRings < 2) return;

            int radialSegments = 12; // U-shape cross section
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();

            float totalLength = 0f;
            List<float> distances = new List<float> { 0f };
            for (int i = 1; i < numRings; i++)
            {
                totalLength += Vector3.Distance(splinePoints[i], splinePoints[i - 1]);
                distances.Add(totalLength);
            }

            for (int i = 0; i < numRings; i++)
            {
                Vector3 current = splinePoints[i];
                Vector3 forward = Vector3.forward;
                if (i < numRings - 1)
                    forward = (splinePoints[i + 1] - current).normalized;
                else if (i > 0)
                    forward = (current - splinePoints[i - 1]).normalized;

                Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
                Vector3 up = Vector3.Cross(forward, right).normalized;

                float v = distances[i] / 10f; // UV tiling

                for (int j = 0; j <= radialSegments; j++)
                {
                    float u = (float)j / radialSegments;
                    float relX = (u - 0.5f) * trackWidth;
                    // Parabolic trough bed profile
                    float normX = (u - 0.5f) * 2f;
                    float relY = channelDepth * (normX * normX);

                    Vector3 vertPos = current + right * relX + up * relY;
                    vertices.Add(transform.InverseTransformPoint(vertPos));
                    uvs.Add(new Vector2(u, v));
                    normals.Add(up);
                }
            }

            int vertsPerRing = radialSegments + 1;
            for (int i = 0; i < numRings - 1; i++)
            {
                for (int j = 0; j < radialSegments; j++)
                {
                    int currentIdx = i * vertsPerRing + j;
                    int nextIdx = currentIdx + vertsPerRing;

                    // Quad triangles
                    triangles.Add(currentIdx);
                    triangles.Add(nextIdx);
                    triangles.Add(currentIdx + 1);

                    triangles.Add(nextIdx);
                    triangles.Add(nextIdx + 1);
                    triangles.Add(currentIdx + 1);
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "WaterSlide_GeneratedMesh";
            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.sharedMesh = mesh;

            if (meshCollider == null) meshCollider = GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = mesh;
            }
        }

        private List<Vector3> SampleSpline(List<Vector3> points, int subdivisions)
        {
            List<Vector3> result = new List<Vector3>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 p0 = i > 0 ? points[i - 1] : points[i];
                Vector3 p1 = points[i];
                Vector3 p2 = points[i + 1];
                Vector3 p3 = (i + 2 < points.Count) ? points[i + 2] : p2;

                for (int s = 0; s < subdivisions; s++)
                {
                    float t = (float)s / subdivisions;
                    result.Add(CatmullRom(p0, p1, p2, p3, t));
                }
            }
            result.Add(points[points.Count - 1]);
            return result;
        }

        private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            return 0.5f * (
                (2f * p1) +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * (t * t) +
                (-p0 + 3f * p1 - 3f * p2 + p3) * (t * t * t)
            );
        }

        private void OnDrawGizmos()
        {
            if (controlPoints == null || controlPoints.Count < 2) return;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < controlPoints.Count; i++)
            {
                Gizmos.DrawWireSphere(controlPoints[i], 1.2f);
                if (i < controlPoints.Count - 1)
                {
                    Gizmos.DrawLine(controlPoints[i], controlPoints[i + 1]);
                }
            }
        }
    }
}

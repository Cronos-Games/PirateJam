using UnityEngine;

/// <summary>
/// Renders a stealth-style vision cone on the ground as a mesh fan.
/// Optionally "cuts" the cone against obstacles using raycasts.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionConeVisualizer : MonoBehaviour
{
    [Header("Shape")]
    [Min(0.1f)] public float distance = 8f;
    [Range(1f, 360f)] public float angleDegrees = 90f;
    [Range(3, 120)] public int segments = 40;

    [Header("Placement")]
    public float groundOffset = 0.02f;     // prevents z-fighting
    public bool followY = false;           // if false, uses transform.position.y as ground level

    [Header("Obstacle cutting (optional)")]
    public bool cutByObstacles = true;
    public LayerMask obstacleMask;
    public float rayStartHeight = 1.6f;    // cast from "eye height"
    public float rayEndHeight = 0.05f;     // project hit point to ground-ish
    public bool ignoreTriggers = true;

    [Header("Update")]
    public bool updateEveryFrame = true;
    
    Mesh _mesh;

    private float groundYWorld = 0;
    
    void Awake()
    {
        _mesh = new Mesh { name = "VisionConeMesh" };
        GetComponent<MeshFilter>().sharedMesh = _mesh;
        
        groundYWorld = transform.position.y + groundOffset;
    }

    void LateUpdate()
    {
        if (updateEveryFrame)
            Rebuild();
    }

    void OnValidate()
    {
        if (segments < 3) segments = 3;
        if (distance < 0.1f) distance = 0.1f;
        if (angleDegrees < 1f) angleDegrees = 1f;
    }

    public void Rebuild()
    {
        if (_mesh == null) return;

        // We build vertices in local space so the mesh naturally follows rotation/position.
        // Ground plane is local XZ.
        int vertCount = segments + 2; // center + (segments+1) arc points
        Vector3[] verts = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] tris = new int[segments * 3];

        // Center at local origin on the ground
        float baseY = followY ? 0f : 0f;
        verts[0] = new Vector3(0f, baseY, 0f);
        uvs[0] = new Vector2(0.5f, 0f);

        float half = angleDegrees * 0.5f;
        float step = angleDegrees / segments;

        // For obstacle cutting, we raycast in world space then convert hit distance to local vertex.
        QueryTriggerInteraction qti = ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide;
        

        for (int i = 0; i <= segments; i++)
        {
            float a = -half + step * i;                       // degrees
            float rad = a * Mathf.Deg2Rad;

            // Direction on local XZ plane
            Vector3 localDir = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)); // forward is +Z

            float d = distance;

            if (cutByObstacles)
            {
                // Ray from eye height in this direction, starting at NPC position in world space.
                Vector3 origin = transform.position + Vector3.up * rayStartHeight;

                // Convert localDir to world direction
                Vector3 worldDir = transform.TransformDirection(localDir).normalized;

                if (Physics.Raycast(origin, worldDir, out RaycastHit hit, distance, obstacleMask, qti))
                {
                    d = hit.distance;
                }
            }

            // Build vertex in local space at distance d
            Vector3 localPoint = localDir * d;

            // Put it on the ground (mesh plane); actual world Y is handled by the object transform.
            // We'll keep local y = 0 and position the GameObject at the ground height externally.
            verts[i + 1] = new Vector3(localPoint.x, 0f, localPoint.z);

            // Basic UVs (radial-ish). Not required unless you want a textured/gradient material.
            uvs[i + 1] = new Vector2((float)i / segments, 1f);
        }

        // Triangles: fan from center
        int t = 0;
        for (int i = 1; i <= segments; i++)
        {
            tris[t++] = 0;
            tris[t++] = i;
            tris[t++] = i + 1;
        }

        _mesh.Clear();
        _mesh.vertices = verts;
        _mesh.triangles = tris;
        _mesh.uv = uvs;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        // Place the visual on the ground: keep rotation from NPC but lock Y slightly above ground
        // (If you attach this script to a child object, just position that child at offset).
        Vector13LockToGround(groundYWorld);
    }

    void Vector13LockToGround(float groundYWorld)
    {
        // Keep XZ following NPC (because this script is on the NPC or a child),
        // but lift slightly to avoid z-fighting.
        Vector3 p = transform.position;
        p.y = groundYWorld;
        transform.position = p;

        // Ensure visual is flat (optional: if NPC can tilt, you might want to zero pitch/roll)
        Vector3 e = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, e.y, 0f);
    }
}

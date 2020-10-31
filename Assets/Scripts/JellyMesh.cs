using UnityEngine;


public class JellyMesh : MonoBehaviour
{
    public float intensity  = 1f;
    public float mass       = 1f;
    public float stiffness  = 1f;
    public float damping    = 0.75f;

    private Mesh originalMesh;
    private Mesh clonedMesh;
    private new MeshRenderer renderer;

    private JellyVertex[] jellyVertices;
    private Vector3[] vertices;


    void Start()
    {
        MeshFilter mesh = GetComponent<MeshFilter>();
        renderer = GetComponent<MeshRenderer>();

        originalMesh = mesh.sharedMesh;
        clonedMesh   = Instantiate(originalMesh);
        mesh.sharedMesh = clonedMesh;

        Vector3[] vertices = originalMesh.vertices;
        jellyVertices = new JellyVertex[vertices.Length];
        for (int i = 0; i < vertices.Length; ++i)
            jellyVertices[i] = new JellyVertex(transform.TransformPoint(vertices[i]));

    }


    void FixedUpdate()
    {
        vertices = originalMesh.vertices;

        for (int i = 0; i < jellyVertices.Length; ++i)
        {
            JellyVertex jellyVertex = jellyVertices[i];
            Vector3 vertex = vertices[i];

            Vector3 target = transform.TransformPoint(vertex);
            float factor = (1 - (renderer.bounds.max.y - target.y) / renderer.bounds.size.y) * intensity;

            jellyVertex.Shake(target, mass, stiffness, damping);
            target = transform.InverseTransformPoint(jellyVertex.position);

            vertices[i] = Vector3.Lerp(vertex, target, factor);

        }

        clonedMesh.vertices = vertices;
    }


    public class JellyVertex
    {
        public Vector3 position;
        private Vector3 velocity;

        public JellyVertex(Vector3 position)
        {
            this.position = position;
            this.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }

        public void Shake(Vector3 target, float mass, float stiffness, float damping)
        {
            Vector3 force = (target - position) * stiffness;
            velocity = (velocity + force / mass) * damping;
            position += velocity;

            Vector3 a = velocity + force + force / mass;
            if (a.magnitude < 0.001f)
                position = target;
        }
    }

}

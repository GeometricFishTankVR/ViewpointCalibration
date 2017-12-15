using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class CubeBehaviour : MonoBehaviour
{
    public float GrowTime = 0.333F;

    public int HitPoints = 8;

    public GameObject CubeDamageFxPrefab;
    public GameObject CubeParticlePrefab;

    public AudioClip DamageSoundEffect;

    internal CubeDropper Dropper;

    internal Vector3 DeltaPosition;
    private Vector3 LastPosition;

    private float Scale;

    private Rigidbody Rigidbody;
    private BoxCollider BoxCollider;
    private MeshRenderer MeshRenderer;
    private Transform Transform;

    private HashSet<GameObject> SameNeighbors;

    private float LinkBreakDistance { get { return BoxCollider.bounds.extents.magnitude * 1.75F; } }

    void Start()
    {
        // Get Components
        Rigidbody = GetComponent<Rigidbody>();
        BoxCollider = GetComponent<BoxCollider>();
        MeshRenderer = GetComponent<MeshRenderer>();
        Transform = GetComponent<Transform>();

        // 
        SameNeighbors = new HashSet<GameObject>();

        // Prevent gravity
        Rigidbody.useGravity = false;

        // Prevent collisions
        BoxCollider.enabled = false;

        // 
        InvokeRepeating("DetectAndBreakLinks", 0F, 1F / 5F); // 5fps heartbeat  

        // 
        Scale = 0;
    }

    void FixedUpdate()
    {
        if (!Rigidbody.useGravity)
        {
            if (Scale < 1.0F)
            {
                Scale += Time.smoothDeltaTime / GrowTime;
                Transform.localScale = new Vector3(Scale, Scale, Scale);
            }
            else
            {
                Scale = 1F;
                Transform.localScale = new Vector3(Scale, Scale, Scale);
                LastPosition = Rigidbody.position;
                DeltaPosition = Vector3.zero;

                Rigidbody.useGravity = true;
                BoxCollider.enabled = true;
            }
        }
        else
        {
            DeltaPosition = Rigidbody.position - LastPosition;
            LastPosition = Rigidbody.position;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "drop_Cube" ||
            col.gameObject.tag == "drop_Floor")
        {
            // Find and make neighbor links
            foreach (var other in DetectSameNeighbors())
                MakeLink(other);
        }
    }

    void CreateDeathParticles()
    {
        // 
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    //var r1 = Random.Range(0, Mathf.PI);
                    //var r2 = Random.Range(0, Mathf.PI);
                    //var r3 = Random.Range(0, Mathf.PI);

                    var offset = new Vector3(x, y, z);
                    offset -= Vector3.one * 1.5F;

                    // 
                    var part = Instantiate(CubeParticlePrefab, Transform.parent);

                    // Clone Material
                    var part_MeshRenderer = part.GetComponent<MeshRenderer>();
                    part_MeshRenderer.material = new Material(MeshRenderer.material);

                    // Position Particle
                    var part_Transform = part.GetComponent<Transform>();
                    part_Transform.localPosition = Transform.localPosition + offset * 0.33F;
                    // part_Transform.rotation = Quaternion.Euler( r1, r2, r3 );

                    // Explosion Vector
                    var force = part_Transform.position - Transform.position;
                    force.x *= Random.Range(-1, +1); // 
                    force.z *= Random.Range(-1, +1); // 
                    force.y += Random.Range(0, +2);  // Upwards force

                    // Explosion Force
                    var part_Rigidbody = part.GetComponent<Rigidbody>();
                    part_Rigidbody.AddForce(force * 4F, ForceMode.VelocityChange);
                }
            }
        }
    }

    public void TakeDamage()
    {
        var chain = DiscoverAllSameNeighbors(true);
        var cubes = chain.Select(x => x.GetComponent<CubeBehaviour>());

        // Find cube with most life, damage that cube
        var s = cubes.OrderByDescending(x => x.HitPoints).First();
        var s_CubeBehaviour = s.GetComponent<CubeBehaviour>();
        s_CubeBehaviour.HitPoints--;

        // 
        AudioSource.PlayClipAtPoint(DamageSoundEffect, Rigidbody.position, 2F);

        // Damage Fx
        var lifeTotal = 0;
        foreach (var c in cubes)
            lifeTotal += c.HitPoints;

        // Chain is dead
        if (lifeTotal <= 0)
        {
            // For every neighbor, but not including itself
            foreach (var n in DiscoverAllSameNeighbors(true))
            {
                // Clear links ( to stop flood fill searching already destroyed objects )
                var n_CubeBehaviour = n.GetComponent<CubeBehaviour>();
                n_CubeBehaviour.CreateDeathParticles(); // Creates particles
                n_CubeBehaviour.SameNeighbors.Clear();  // Removes neighboring links

                // Remove this object
                Destroy(n);
            }
        }
        else
        {
            // Damage Fx
            foreach (var n in chain)
            {
                var i = Instantiate(CubeDamageFxPrefab, n.transform, false);
                i.transform.SetParent(null, true);
            }
        }
    }

    /// <summary>
    /// Flood fill to collect all the linked-set of neighbors.
    /// </summary>
    IEnumerable<GameObject> DiscoverAllSameNeighbors(bool includeSelf)
    {
        var set = new HashSet<GameObject>();
        var queue = new Queue<GameObject>();
        queue.Enqueue(gameObject);

        // While we are unaccounted
        while (queue.Count > 0)
        {
            var obj = queue.Dequeue();
            if (obj == null) continue;

            // If we haven't discovered this object, add its neighbors
            if (set.Add(obj))
            {
                var obj_CubeBehaviour = obj.GetComponent<CubeBehaviour>();
                foreach (var n in obj_CubeBehaviour.SameNeighbors)
                    queue.Enqueue(n);
            }
        }

        if (!includeSelf)
            set.Remove(gameObject);

        return set;
    }

    #region Neighbor Links

    /// <summary>
    /// Bi-directional link this cube with another.
    /// </summary>
    private void MakeLink(GameObject obj)
    {
        //Debug.LogFormat( "Connecting {0} to {1}", name, obj.name );

        var other = obj.GetComponent<CubeBehaviour>();
        SameNeighbors.Add(other.gameObject);
        other.SameNeighbors.Add(gameObject);
    }

    /// <summary>
    /// Bi-directional unlink this cube with another.
    /// </summary>
    private void RemoveLink(GameObject obj, float distance)
    {
        //name = "CULPRIT 1";
        //obj.name = "CULPRIT 2"; 
        //Debug.LogErrorFormat( "Breaking {0} to {1} ({2}/{3})", name, obj.name, distance, LinkBreakDistance );
        // TODO: Error where blocks think they are touching

        var other = obj.GetComponent<CubeBehaviour>();
        SameNeighbors.Remove(other.gameObject);
        other.SameNeighbors.Remove(gameObject);

        // If a link is severed, and its marked as no life, make it live a bit more
        if (other.HitPoints <= 0) other.HitPoints = 1;
        if (HitPoints <= 0) HitPoints = 1;
    }

    /// <summary>
    /// Searches all links and if any are too far away, remove the link.
    /// </summary>
    private void DetectAndBreakLinks()
    {
        foreach (var other in SameNeighbors.ToArray()) // ToArray() to prevent concurrent exception
        {
            var other_Transform = other.GetComponent<Transform>();
            var dist = Vector3.Distance(Transform.position, other_Transform.position);
            if (dist > LinkBreakDistance) RemoveLink(other, dist);
        }
    }

    #endregion

    private IEnumerable<GameObject> DetectSameNeighbors()
    {
        GameObject other;
        if (DetectSameNeighbors_Raycast(new Vector3(+1, 0, 0), out other)) yield return other;
        if (DetectSameNeighbors_Raycast(new Vector3(0, +1, 0), out other)) yield return other;
        if (DetectSameNeighbors_Raycast(new Vector3(0, 0, +1), out other)) yield return other;
        if (DetectSameNeighbors_Raycast(new Vector3(-1, 0, 0), out other)) yield return other;
        if (DetectSameNeighbors_Raycast(new Vector3(0, -1, 0), out other)) yield return other;
        if (DetectSameNeighbors_Raycast(new Vector3(0, 0, -1), out other)) yield return other;
        yield break;
    }

    private bool DetectSameNeighbors_Raycast(Vector3 direction, out GameObject obj)
    {
        RaycastHit hit;
        if (Physics.Raycast(Transform.position, direction, out hit, LinkBreakDistance))
        {
            obj = hit.collider.gameObject;
            if (obj.gameObject.tag == "drop_Cube")
            {
                // Return true if this cube matches the neighbor cube.
                var cube_MeshRenderer = obj.gameObject.GetComponent<MeshRenderer>();
                return cube_MeshRenderer.material.name == MeshRenderer.material.name;
            }
        }

        obj = null;
        return false;
    }
}

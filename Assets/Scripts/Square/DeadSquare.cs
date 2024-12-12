using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadSquare : MonoBehaviour
{
    [SerializeField] float explosionForce = 100;
    [SerializeField] float explosionRadius = 10;
    Collider[] colliders = new Collider[50];
    void ExplodeNonAlloc()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders);

        if (numColliders > 0)
        {
            for (int i = 0; i < numColliders; i++)
            {
                if (colliders[i].TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(explosionForce, new Vector3(transform.position.x + Random.Range(-2f, 2f), transform.position.y + Random.Range(-2f, 2f), transform.position.z), explosionRadius);
                    Destroy(rb.gameObject, Random.Range(3f, 10f));
                }
            }
        }
    }
    private void Start()
    {
        ExplodeNonAlloc();
        Destroy(gameObject, 10f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    // Start is called before the first frame update
    LineRenderer lineRenderer;
    Transform shootPoint;
    bool isShooting = false;

    int shootableMask;
    void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        shootPoint = transform.GetChild(2);
        shootableMask = LayerMask.GetMask("Shootable");
        lineRenderer.enabled = false;
    }

    IEnumerator FireEffect(float fireRate)
    {
        isShooting = true;
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(fireRate);
        lineRenderer.enabled = false;
        yield return new WaitForSeconds(fireRate);

        isShooting = false;
    }

    public RaycastHit[] Shoot(float fireRate, GameObject from, float damage)
    {
        RaycastHit[] hits = null;
        if (!isShooting)
        {
            StopAllCoroutines();
            StartCoroutine(FireEffect(fireRate));
            Ray shootRay = new Ray(transform.position, -transform.forward);
            hits = Physics.RaycastAll(shootRay, 200, shootableMask);
            foreach (RaycastHit hit in hits)
            {
                MonoBehaviour[] mb = hit.collider.gameObject.GetComponents<MonoBehaviour>();
                foreach(MonoBehaviour mono in mb)
                {
                    if (mono is IShootable)
                    {
                        IShootable shootable = (IShootable)mono;
                        shootable.OnGetShot(from, damage);
                    }
                }
     
            }

        }

        return hits;
    }
}

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
        shootableMask = LayerMask.GetMask("Shootable", "Wall");
        lineRenderer.enabled = false;
    }

    IEnumerator FireEffect(float fireRate)
    {
        isShooting = true;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, shootPoint.position);

        yield return new WaitForSeconds(fireRate);
        lineRenderer.enabled = false;
        yield return new WaitForSeconds(fireRate);

        isShooting = false;
    }

    public RaycastHit? Shoot(float fireRate, GameObject from, float damage)
    {
        RaycastHit hit;
        if (!isShooting)
        {
            StopAllCoroutines();
            StartCoroutine(FireEffect(fireRate));
            Ray shootRay = new Ray(transform.position, -transform.forward);
            if(Physics.Raycast(shootRay, out hit ,500, shootableMask))
            {
                MonoBehaviour mono = hit.collider.gameObject.GetComponent<MonoBehaviour>();

                lineRenderer.SetPosition(1, new Vector3(hit.collider.transform.position.x, shootPoint.position.y, hit.collider.transform.position.z));
                if (mono is IShootable)
                {
                    IShootable shootable = (IShootable)mono;
                    shootable.OnGetShot(from, damage);
                }
                return hit;
            }
           

        }

        return null;
    }
}

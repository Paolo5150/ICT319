using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    // Start is called before the first frame update
    public LineRenderer lineRenderer;
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

        yield return new WaitForSeconds(0.001f);
        lineRenderer.enabled = false;
        yield return new WaitForSeconds(fireRate);

        isShooting = false;
    }

    public GameObject Aim()
    {
        RaycastHit hit;
        Ray shootRay = new Ray(transform.position, -transform.forward);
        if (Physics.Raycast(shootRay, out hit, 500, shootableMask))
        {
            MonoBehaviour[] mono = hit.collider.gameObject.GetComponents<MonoBehaviour>();
            lineRenderer.SetPosition(1, new Vector3(hit.collider.transform.position.x, shootPoint.position.y, hit.collider.transform.position.z));
            return hit.collider.gameObject;
        }

        return null;
    }

    public GameObject Shoot(float fireRate, GameObject from, float damage)
    {
        GameObject hit;
        if (!isShooting)
        {
            hit = Aim();
            StopAllCoroutines();
            StartCoroutine(FireEffect(fireRate));
            if(hit != null)
            {
                MonoBehaviour[] mono = hit.GetComponents<MonoBehaviour>();

                lineRenderer.SetPosition(1, new Vector3(hit.transform.position.x, shootPoint.position.y, hit.transform.position.z));
                foreach(MonoBehaviour m in mono)
                {
                    if (m is IShootable)
                    {
                        IShootable shootable = (IShootable)m;
                        shootable.OnGetShot(from, damage);
                    }
                }
    
                return hit;
            }
           

        }

        return null;
    }
}

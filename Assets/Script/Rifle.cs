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
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(fireRate / 2.0f);
        lineRenderer.enabled = false;

    }


    public GameObject Shoot(float fireRate, GameObject from, float damage)
    {
        RaycastHit hit;
        Ray shootRay = new Ray(transform.position, -transform.forward);
        if (Physics.Raycast(shootRay, out hit, 500, shootableMask))
        {
            lineRenderer.SetPosition(0, shootPoint.position);

            lineRenderer.SetPosition(1, new Vector3(hit.point.x, shootPoint.position.y, hit.point.z));
            StartCoroutine(FireEffect(fireRate));

            MonoBehaviour[] mono = hit.collider.gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour m in mono)
            {
                if (m is IShootable)
                {
                    IShootable shootable = (IShootable)m;
                    shootable.OnGetShot(from, damage);
                }
            }

            return hit.collider.gameObject;
        }
      

        return null;
    }
}

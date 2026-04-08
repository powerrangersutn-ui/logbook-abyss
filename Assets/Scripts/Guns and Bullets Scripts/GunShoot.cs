using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [SerializeField] private GameObject referenceProyectile;
    [SerializeField] private Transform barrel;
    [SerializeField] private float bulletSpeed;

    private Vector3 destination;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnFire();
        }
    }

    private void OnFire()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000))
            destination = hit.point;
        else
            destination=ray.GetPoint(1000);
        CreateProyectile();
    }
    private void CreateProyectile()
    {
        GameObject proyectile=Instantiate(referenceProyectile,barrel.position,Quaternion.identity);
        Destroy(proyectile, 3);
        proyectile.GetComponent<Rigidbody>().AddForce((destination-proyectile.transform.position).normalized*bulletSpeed, ForceMode.Impulse);
    }
}

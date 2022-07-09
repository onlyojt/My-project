using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public GameObject upProjectile;
    public GameObject downProjectile;

    public BoxCollider2D mapBounds;

    private float xMin, xMax, yMin, yMax;
    private float camY, camX;
    private float camOrthsize;
    private float cameraRatio;
    private Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        xMin = mapBounds.bounds.min.x;
        xMax = mapBounds.bounds.max.x;
        yMin = mapBounds.bounds.min.y;
        yMax = mapBounds.bounds.max.y;
        mainCam = GetComponent<Camera>();
        camOrthsize = mainCam.orthographicSize;
        cameraRatio = (xMax + camOrthsize) / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector2 pos = upProjectile.transform.position - downProjectile.transform.position;
        if (pos.x < 0)
            pos.x *= -1;*/

        camY = Mathf.Clamp(upProjectile.transform.position.y, yMin + camOrthsize, yMax - camOrthsize);
        camX = Mathf.Clamp(upProjectile.transform.position.x, xMin + cameraRatio, xMax - cameraRatio);

        transform.position = new Vector3 (camX, transform.position.y, transform.position.z);
    }
}

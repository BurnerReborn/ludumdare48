using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Transform target;

    public Transform farBackground, middleBackground;

    public float minHeight, maxHeight;

    public bool stopFollow;

    //private float lastXPos;
    private Vector2 lastPos;

    /// a global running clock of the scene, updating each frame
    public static float Clock { get; private set;}

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 45;
    }

    // Start is called before the first frame update
    void Start()
    {
        //lastXPos = transform.position.x;
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        /* transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        float clampedY = Mathf.Clamp(transform.position.y, minHeight, maxHeight);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z); */

        if (!stopFollow)

        {
            // TODO camera: replaced for perspective view
            // transform.position = new Vector3(target.position.x, Mathf.Clamp(target.position.y, minHeight, maxHeight), transform.position.z);
            // transform.position = new Vector3(target.position.x, Mathf.Clamp(target.position.y, minHeight, maxHeight), target.position.z - 3*LayerManager.instance.depthUnit);
            transform.position = new Vector3(target.position.x, Mathf.Clamp(target.position.y, minHeight, maxHeight), target.position.z - 3*LayerManager.instance.depthUnit);

            //float amountToMoveX = transform.position.x - lastXPos;
            Vector2 amountToMove = new Vector2(transform.position.x - lastPos.x, transform.position.y - lastPos.y);

            farBackground.position = farBackground.position + new Vector3(amountToMove.x, amountToMove.y, 0f);
            middleBackground.position += new Vector3(amountToMove.x, amountToMove.y, 0f) * .5f;

            float backgroundDepth = transform.position.z + 120; // always out of reach, but close enough
            farBackground.position = new Vector3(farBackground.position.x, farBackground.position.y, backgroundDepth);
            middleBackground.position = new Vector3(middleBackground.position.x, middleBackground.position.y, backgroundDepth);

            //lastXPos = transform.position.x;
            lastPos = transform.position;
        }

        CameraController.Clock += Time.deltaTime;
    }
}

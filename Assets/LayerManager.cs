using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

/// resoponsible for handling collisions between the player and the platforms on different layers
public class LayerManager : MonoBehaviour
{
    public static LayerManager instance;

    [Tooltip("Determines the z-axis difference between each layer")]
    public float depthUnit;

    [Tooltip("The z-position the player respawns to, also the initial z-position of the player")]
    public float respawnZ = 0;

    private bool m_once;

    private float m_clock;

    private void Awake()
    {
        instance = this;
        onLayerTransition(respawnZ);

    }

    // Start is called before the first frame update
    void Start()
    {
        m_once = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerHealthController.instance.currentHealth <= 0) {
            // on death, we want to reset all colliders to the new player coordinates
            onLayerTransition(respawnZ);
        }
    }

    /// handler for when the player decides to switch planes
    public void onLayerTransition(float posZ) {
        PlatformEffector2D[] components = GameObject.FindObjectsOfType<PlatformEffector2D>();

        // Debug.LogFormat("[{0}] PlayerMask: {1}, Ground2 Mask: {2}", CameraController.Clock, LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground2"));
        foreach (var comp in components) {
            bool sameLayer = comp.transform.position.z == posZ;

            // if (sameLayer)
            //     Debug.LogFormat("[{0}] {1} Player on my layer! (z={2})", 
            //                     CameraController.Clock, comp.gameObject.name, comp.transform.position.z);

            // Debug.LogFormat("[{0}] {1} sameLayer? {2} colliderMask: {3}", 
                            // CameraController.Clock, comp.gameObject.name, sameLayer, comp.colliderMask);

            // check for all platforms that we're on the same layer
            if (sameLayer)
                comp.colliderMask = LayerMask.NameToLayer("Everything");
            else
                comp.colliderMask = 1<<LayerMask.NameToLayer("Everything") & ~(1<<LayerMask.NameToLayer("Player"));

        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Tilemaps;

/// resoponsible for handling collisions between the player and the platforms on different layers
public class LayerManager : MonoBehaviour
{
    public static LayerManager instance;

    [Tooltip("Determines the z-axis difference between each layer")]
    public float depthUnit;

    [Tooltip("The z-position the player respawns to, also the initial z-position of the player")]
    public float respawnZ = 0;

    [Tooltip("The furthest a player can jump in the x-direction. Disable check by setting this to 0")]
    public float xJumpRange;

    public float minZ;
    public float maxZ;

    private bool m_once;

    private float m_clock;

    private PlatformEffector2D[] m_nullableComponents;

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

    }

    public void OnPlayerRespawn() {
        // on death, we want to reset all colliders to the new player coordinates
        onLayerTransition(respawnZ);
    }

    /// checks      if the player can make a transition
    /// @towards    moving towards the camera -- decrementing z
    /// @cache      for optimization, this allows OnLayerTransition() to use the same calculations used here.
    ///             this assumes that the objects in the scene don't when onLayerTransition() is called 
    public bool CanTransitionLayer(Vector3 pos, bool towards, bool cache=false) {
        if (!towards && pos.z + depthUnit > maxZ)
            return false;
        if (towards && pos.z - depthUnit < minZ)
            return false;


        PlatformEffector2D[] components;
        if (cache) {
            // update the cached components if not updated already
            if (m_nullableComponents != null) {
                components =  m_nullableComponents;
            } else {
                m_nullableComponents = GameObject.FindObjectsOfType<PlatformEffector2D>();
                components = m_nullableComponents;
            }
        } else {
            components = GameObject.FindObjectsOfType<PlatformEffector2D>();
        }

        // calculate the new position Z if the player did make the transition
        float newPosZ = (towards) ? pos.z - depthUnit : pos.z + depthUnit;

        if (this.xJumpRange != 0) {
            bool withinX = false;
            foreach (var comp in components) {
                if (comp.transform.position.z == newPosZ) {
                    // check if we can safely transition to this layer
                    // for now, just check that we're within its x-axis range
                    if (   comp.transform.position.x <= pos.x + this.xJumpRange
                        && comp.transform.position.x >= pos.x - this.xJumpRange) {
                        withinX = true;
                        break;
                    }
                }
            }
            if (!withinX)
                return false;
        }

        return true;
    }

    /// handler for when the player decides to switch planes
    public void onLayerTransition(float posZ) {

        if (m_nullableComponents == null)
            m_nullableComponents = GameObject.FindObjectsOfType<PlatformEffector2D>();

        // Debug.LogFormat("[{0}] PlayerMask: {1}, Ground2 Mask: {2}", CameraController.Clock, LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground2"));
        foreach (var comp in m_nullableComponents) {
            float dz = comp.transform.position.z - posZ;
            bool sameLayer = dz == 0;

            // if (sameLayer)
            //     Debug.LogFormat("[{0}] {1} Player on my layer! (z={2})", 
            //                     CameraController.Clock, comp.gameObject.name, comp.transform.position.z);

            // Debug.LogFormat("[{0}] {1} sameLayer? {2} colliderMask: {3}", 
                            // CameraController.Clock, comp.gameObject.name, sameLayer, comp.colliderMask);

            // check for all platforms that we're on the same layer
            if (!sameLayer) {

                comp.colliderMask &= ~(1<<LayerMask.NameToLayer("Player"));
                comp.gameObject.layer = LayerMask.NameToLayer("Ground2");
            } else {
                comp.colliderMask |= 1<<LayerMask.NameToLayer("Player");
                comp.gameObject.layer = LayerMask.NameToLayer("Ground");
            }

            // setup the depth of that layer
            Tilemap tm = comp.gameObject.GetComponent<Tilemap>();
            SpriteRenderer sr = comp.gameObject.GetComponent<SpriteRenderer>();
            float alpha = 1 - (dz / depthUnit) / ((maxZ - minZ) / depthUnit);
            if (alpha == 0) alpha = 0.3f;
            if (tm != null) {
                tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, alpha);
            } else if (sr != null) {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            }

        }

        // we are done, clear the cached values
        m_nullableComponents = null;
    }
}
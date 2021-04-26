using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlatformController : MonoBehaviour
{

    [SerializeField]
    public bool m_platformCanBreak = false;
    [SerializeField]
    float m_timeToBreak = 2.0f;
    [SerializeField]
    float m_respawnTime = 5.0f;
    [SerializeField]

    private float _breakTimer;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_breakTimer != 0 
              && CameraController.Clock - _breakTimer >= m_timeToBreak) {
            _breakTimer = 0;
            // _respawnTimer = CameraController.Clock;
            // Debug.Log("_breakTimer (stop): " + _respawnTimer);
            LevelManager.instance.RespawnPlatform(this.gameObject, m_respawnTime);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!m_platformCanBreak)
            return;

        if (other.tag == "Player")
        {
            _breakTimer = CameraController.Clock;
            // Debug.Log("_breakTimer (start): " + _breakTimer);
        }
    }
}

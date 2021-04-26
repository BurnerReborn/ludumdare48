using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public FMODUnity.StudioEventEmitter[] soundEffects;

    public FMODUnity.StudioEventEmitter bgm, levelEndMusic, bossMusic;

    public enum Sfx {
        BossHit,
        BossImpact,
        BossShot,
        EnemyExplode,
        LevelSelected,
        MapMovement,
        PickupGem,
        PicupHealth,
        PlayerDeath,
        PlayerHurt,
        PlayerJump,
        WarpJingle,
        PlayerLand,
        PlayerMove
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySFX(Sfx soundToPlay)
    {
        Debug.Log("whyyyy " + soundToPlay);
        var sfx = soundEffects[(int)soundToPlay];
        if (sfx == null)
            return;

        Debug.Log("whyyyy " + sfx);

        // sfx.Stop();
        // soundEffects[soundToPlay].pitch = Random.Range(.9f, 1.1f);
        sfx.Play();
    }

    public void PlayLevelVictory()
    {
        if (bgm != null)
        {
            bgm.Stop();
            levelEndMusic.Play();
        }
    }

    public void PlayBossMusic()
    {
        if (bgm != null)
        {
            bgm.Stop();
            bossMusic.Play();
        }
    }

    public void StopBossMusic()
    {
        if (bgm != null)
        {
            bossMusic.Stop();
            bgm.Play();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }   
    }
    [Tooltip("0: English, 1: , 2: , 3: , 4: ")]
    public int language=0;

    public AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}



[System.Serializable]
public class AudioManager {

    public AudioSource contentAudio;
    public AudioSource sfxAudio;
}

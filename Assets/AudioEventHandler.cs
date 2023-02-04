using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEventHandler : MonoBehaviour 
{
    [SerializeField]
    private GameManager gm;
    public static AudioEventHandler instance;
    [SerializeField]
    private FmodAudioManager audioManager;

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
        //Unsubscribe to bite
    }
}

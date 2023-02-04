using System;
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
    private const string CONST_RAT = "Rat";
    private const string CONST_PIG = "Boar";
    private const string CONST_RABBIT = "Rabbit";



    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        Entity.OnEntityDeath += PlayAnimalKill;
    }
    private void OnDisable()
    {
        Entity.OnEntityDeath -= PlayAnimalKill;
    }

    private void PlayAnimalKill(Entity obj)
    {
        switch(obj.Data.CommonData.name)
        {
            case (CONST_PIG):
                FmodAudioManager.instance.PlayAndAttachOneShot(FmodSfxClass.sfxEnums.pigKill);
                break;
            case (CONST_RAT):
                FmodAudioManager.instance.PlayAndAttachOneShot(FmodSfxClass.sfxEnums.ratKill);
                break;
            case (CONST_RABBIT):
                FmodAudioManager.instance.PlayAndAttachOneShot(FmodSfxClass.sfxEnums.rabbitKill);
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
[System.Serializable]

//Defining each sfx.
public class FmodSfxClass

{
    public string name;
    public enum sfxEnums { 
        footsteps,
        ratKill,
        pigKill,
        rabbitKill,
        Bite,
        Switch,
        GraveEmerge
    }
    public  sfxEnums sfx;
    public EventReference path;
    public EventInstance instance;

    public void CreateInstance() => instance = RuntimeManager.CreateInstance(path);
}



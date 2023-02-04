using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
[System.Serializable]
public class FmodBGMClass 
{
    public string name;
    public enum bgmEnums
    {
        Upperworld,
        Underworld
    }
    public bgmEnums _bgmEnums;
    public EventReference path;
    public EventInstance instance;
    [ParamRef]
    public string levelParam;

    public void CreateInstance() => instance = RuntimeManager.CreateInstance(path);

}

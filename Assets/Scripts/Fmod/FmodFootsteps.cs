using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.InputSystem;

public class FmodFootsteps : MonoBehaviour
{
    [SerializeField]private Transform footstepsTransform;
    [SerializeField] private Rigidbody2D playerRig;
    [SerializeField] private ParamRef materialParam;
    [SerializeField]private LayerMask Ground;
    [SerializeField] private FmodAudioManager audioManager;
    public enum FootstepsMaterial {Wood , Gravel , Stone, Metal }
    public enum MoveState {Walk , Run , Crouch }
    private FootstepsMaterial footstepsMaterial;
    private int footstepsMaterialID;
    private MoveState moveState;

    public float curTime;
    public float timeBetweenSteps;

    private void Update()
    {
        CheckMovement();
    }
    private void CheckGroundMaterial()
    {
        RaycastHit ray;
        if (Physics.Raycast(transform.position,Vector3.down,out ray, 1 , Ground))
        {

        }
    }

    private void CheckMovement()
    {
        if (playerRig.velocity.magnitude > 0)
        {
            PlayStep();
        }
    }

    private void PlayStep()
    {
        curTime += Time.deltaTime;
        if(curTime >= timeBetweenSteps)
        {
            curTime = 0;
            audioManager.PlayAndAttachOneShot(FmodSfxClass.sfxEnums.footsteps,playerRig.transform.position);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance => _instance;

    private delegate void CameraState();
    private CameraState _cameraState;

    [SerializeField] private Camera _mainCam;
    public Camera MainCam => _mainCam;

    [SerializeField] private Transform _mainCamTransform;
    public Transform MainCamTransform => _mainCamTransform;

    [SerializeField] private float _size = 5f;

    private void Awake()
    {
        _mainCam = Camera.main;
        _mainCamTransform = _mainCam.transform;
        _cameraState = FollowPlayer;
    }
    private void Update()
    {
        _cameraState.Invoke();
    }
    private void FollowPlayer()
    {
        _mainCamTransform.position = GameManager.Instance.PlayerPrefab.transform.position;
    }
    private void FollowVampireLord()
    {

    }
}

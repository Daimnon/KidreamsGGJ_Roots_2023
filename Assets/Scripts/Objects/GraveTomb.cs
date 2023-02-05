using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveTomb : MonoBehaviour
{
    private EntityData _engravedVillagerData;
    public EntityData EngravedVillagerData { get => _engravedVillagerData; set => _engravedVillagerData = value; }

    private const int _maxGraves = 5;

    [SerializeField] private float _offsetFromDirtY = 1;
    [SerializeField] private bool _isEngraved = false;
    [SerializeField] private Rigidbody2D _rb;

    private void Update()
    {
        if (!_isEngraved && _engravedVillagerData && CheckGravePosInAir())
        {
            Engrave();
            GameManager.Instance.NewGraveDirt = null;
            _isEngraved = true;
        }
    }

    private bool CheckGravePosInAir()
    {
        bool isGraveInAir = GameManager.Instance.NewGraveDirt.transform.position.y <= transform.position.y ? true : false;
        return isGraveInAir;
    }

    private void Engrave()
    {
        _rb.gravityScale = 0;
        transform.position = GameManager.Instance.NewGraveDirt.transform.position + new Vector3(0f, _offsetFromDirtY, 0f);
        GameManager.Instance.Engraved.Add(_engravedVillagerData);
    }
}

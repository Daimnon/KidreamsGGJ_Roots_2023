using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Entity
{
    [SerializeField] private GameObject _graveDirt, _graveTomb;
    private Rigidbody2D _rb;
    private GameObject _currentGraveDirt, _currentGraveTomb;
    private Grave _currentGrave;

    [SerializeField] private SpriteRenderer _villigerGraphics;
    [SerializeField] private float _tombInstansiationOffsetY = 10f, _tombOffsetY = 3.25f;

    private const int _maxGraves = 5;

    [SerializeField] private Grave _gravePrefab;

    // TODO: projectiles (handle attack range in base class first?)
    // TODO: Grave system (separate class, but will hold the Villagers
    protected override void TransitionToAttacking(EntityState prevState)
    {
        base.TransitionToAttacking(prevState);
    }

    protected override void UpdateAttackingState()
    {
        base.UpdateAttackingState();
    }

    protected override void Die()
    {
        // base will Destroy the object - do stuff before
        base.Die();
    }

    private void Update()
    {
        if (_currentGraveTomb && CheckGravePosInAir())
        {
            Engrave();
        }
    }

    private void Engrave()
    {
        _rb.gravityScale = 0;
        _currentGraveTomb.transform.position = new Vector3(_currentGraveDirt.transform.position.x, _currentGraveDirt.transform.position.y + _tombOffsetY, _currentGraveDirt.transform.position.z);
        //_currentGrave.EntityData = Data;

        if (GameManager.Instance.Engraved.Count < _maxGraves)
            GameManager.Instance.Engraved.Add(this);

        Destroy(gameObject);
    }
    private bool CheckGravePosInAir()
    {
        bool isGraveInAir = _currentGraveTomb.transform.position.y >= _currentGraveDirt.transform.position.y ? true : false;
        return isGraveInAir;
    }
    
    public void CreateGrave()
    {
        CaptureEntity();
        _villigerGraphics.enabled = false;
        _rb = _currentGraveTomb.GetComponent<Rigidbody2D>();

        _currentGraveDirt = Instantiate(_graveDirt, transform, true);
        _currentGraveTomb = Instantiate(_graveTomb, new Vector3(_graveDirt.transform.position.x, _graveDirt.transform.position.y + _tombInstansiationOffsetY), Quaternion.identity);
    }
}

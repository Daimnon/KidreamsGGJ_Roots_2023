using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villiger : Entity
{
    [SerializeField] private GameObject _gravePrefab, _currentGraveGameObject, _currentTomb;
    [SerializeField] private Grave ;
    [SerializeField] private SpriteRenderer _villigerGraphics;
    [SerializeField] private float _graveInstansiationOffsetY = 10f;

    private const int _maxGraves = 5;

    private void Update()
    {
        if (_currentTomb && CheckGravePosInAir())
        {
            Engrave();
        }
    }

    private void Engrave()
    {
        _currentTomb.transform.position = transform.position;
        _currentTomb.EntityData = Data;

        if (GameManager.Instance.SavedGraves.Count < _maxGraves)
            GameManager.Instance.SavedGraves.Add(_currentTomb);

        Destroy(gameObject);
    }
    public void CreateGrave()
    {
        _villigerGraphics.enabled = false;
        _currentGraveGameObject = Instantiate(_gravePrefab, new Vector3(transform.position.x, transform.position.y + _graveInstansiationOffsetY), Quaternion.identity);
    }
    private bool CheckGravePosInAir()
    {
        bool isGraveInAir = _currentTomb.transform.position.y > transform.position.y ? true : false;

        return isGraveInAir;
    }
}

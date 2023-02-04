using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villiger : Entity
{
    [SerializeField] private GameObject _gravePrefab, _currentGraveGameObject;
    [SerializeField] private Grave _currentGrave;
    [SerializeField] private SpriteRenderer _villigerGraphics;
    [SerializeField] private float _graveInstansiationOffsetY = 10f;

    private const int _maxGraves = 5;

    private void Update()
    {
        if (_currentGrave && CheckGravePosInAir())
        {
            Engrave();
        }
    }

    private void Engrave()
    {
        _currentGrave.transform.position = transform.position;
        _currentGrave.EntityData = Data;

        if (GameManager.Instance.SavedGraves.Count < _maxGraves)
            GameManager.Instance.SavedGraves.Add(_currentGrave);

        Destroy(gameObject);
    }
    public void CreateGrave()
    {
        _villigerGraphics.enabled = false;
        _currentGraveGameObject = Instantiate(_gravePrefab, new Vector3(transform.position.x, transform.position.y + _graveInstansiationOffsetY), Quaternion.identity);
    }
    private bool CheckGravePosInAir()
    {
        bool isGraveInAir = _currentGrave.transform.position.y > transform.position.y ? true : false;

        return isGraveInAir;
    }
}

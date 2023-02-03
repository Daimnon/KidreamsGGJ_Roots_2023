using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    [SerializeField] private GameObject _gravesParent, _heartsParent;
    [SerializeField] private Transform _bloodFill;

    private void Awake()
    {
        _instance = this;
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < _heartsParent.transform.childCount; i++)
        {
            GameObject gameObject = _heartsParent.transform.GetChild(i).gameObject;

            if (GameManager.Instance.PlayerController.Hp <= i)
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        }
    }
    public void UpdateGraves()
    {
        for (int i = 0; i < _gravesParent.transform.childCount; i++)
        {
            GameObject gameObject = _gravesParent.transform.GetChild(i).gameObject;

            if (GameManager.Instance.PlayerController.EngravedAmount <= i)
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        }
    }
    public void UpdateBlood()
    {
        float bloodFillHeightPerBloodPoint = (float)GameManager.Instance.PlayerController.Hp / 10; // 10 = maxBlood;

        _bloodFill.localScale = new Vector3(_bloodFill.localScale.x, bloodFillHeightPerBloodPoint, _bloodFill.localScale.z);
    }
}

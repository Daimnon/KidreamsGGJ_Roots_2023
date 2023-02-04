using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class UnderworldOverlay : MonoBehaviour
{
    
    [Header("Anim - Transition 1")]
    [SerializeField] private Material _underworldOverlayMaterial;
    [SerializeField] private Color _transition1Color;
    [SerializeField] private float _noiseBlend1;
    [SerializeField] private float _animDuration1;
    
    [Header("Delay")]
    [SerializeField] private float _midAnimDelay;
    
    [Header("Anim - Transition 2 / Underworld")]
    [SerializeField] private Color _underworldColor;
    [SerializeField] private float _noiseBlend2;
    [SerializeField] private float _animDuration2;

    private static readonly int PropBlend = Shader.PropertyToID("_Blend");
    
    // greyscale constant
    // private static readonly int PropGreyscaleBlend = Shader.PropertyToID("_GreyscaleBlend");
    private static readonly int PropNoiseBlend = Shader.PropertyToID("_NoiseBlend");
    private static readonly int PropColorTint = Shader.PropertyToID("_Color");
    
    private bool _wasInit;

    public event Action OnUnderworldAnimationComplete;
    

    [Button("SetRegularMode")]
    public void SetRegularMode()
    {
        _underworldOverlayMaterial.SetFloat(PropBlend, 0f);
    }

    [Button("StartUnderworldAnim")]
    public void StartUnderworldAnim()
    {
        // Init states
        _underworldOverlayMaterial.SetColor(PropColorTint, _transition1Color);
        _underworldOverlayMaterial.SetFloat(PropBlend, 0f);
        _underworldOverlayMaterial.SetFloat(PropNoiseBlend, _noiseBlend1);
        
        // Blend to 1
        // Delay
        // Noise down && Color 
        DOTween.Sequence()
            .Append(_underworldOverlayMaterial.DOFloat(1f, PropBlend, _animDuration1))
            .AppendInterval(_midAnimDelay)
            .OnComplete(SecondPartAnims);
    }

    private void SecondPartAnims()
    {
        _underworldOverlayMaterial.DOColor(_underworldColor, PropColorTint, _animDuration2);
        _underworldOverlayMaterial.DOFloat(_noiseBlend2, PropNoiseBlend, _animDuration2)
            .OnComplete(() => OnUnderworldAnimationComplete?.Invoke());
    }
}

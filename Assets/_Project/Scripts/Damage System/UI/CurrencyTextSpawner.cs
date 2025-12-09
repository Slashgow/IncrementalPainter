using UnityEngine;

public class CurrencyTextSpawner : TextSpawner 
{
    [SerializeField, Range(0,100)] private int criticalMinLimit;

    public bool IsCritical(int amount) => amount > criticalMinLimit;
    private void OnEnable() => CurrencyHolder.OnPickUpCurrency += CurrencyHolder_OnPickUpCurrency;
    private void OnDisable() => CurrencyHolder.OnPickUpCurrency -= CurrencyHolder_OnPickUpCurrency;
    private void CurrencyHolder_OnPickUpCurrency(int amount, Vector3 worldPosition) => SpawnDamageText(amount, worldPosition, IsCritical(amount), "$");
    
}

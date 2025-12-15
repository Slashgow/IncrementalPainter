using TMPro;
using UnityEngine;

public class UICurrencyText : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;
    private void Start()
    {
        CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyText;
        UpdateCurrencyText(CurrencyManager.Instance.CurrentCurrency);
    }
    private void OnDestroy()
    {
        if(CurrencyManager.HasInstance)
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrencyText;
    }
    private void UpdateCurrencyText(int newCurrency)
    {
        currencyText.text = $"{FormatUtility.FormatValue(newCurrency)} $";
    }
}

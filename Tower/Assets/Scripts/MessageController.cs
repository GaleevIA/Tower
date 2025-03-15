using System;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class MessageController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textField;

    private LocalizationConfig _localizationConfig;

    public Action<TextMeshProUGUI> OnShowMessage;

    [Inject]
    public void Initialize(LocalizationConfig localizationConfig)
    {
        _localizationConfig = localizationConfig;
    }

    public void ShowMessage(string key)
    {
        var localization = _localizationConfig.config.Where(e => e.Key == key).Select(e => e.Value).FirstOrDefault();

        if (localization is not null)
        {
            _textField.text = localization;

            OnShowMessage?.Invoke(_textField);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class MessageController : MonoBehaviour, IMessageController
{
    [SerializeField]
    private TextMeshProUGUI _textField;

    private List<LocalizationStruct> _localization;
    private IAnimController _animController;

    [Inject]
    public void Initialize(ILocalizationConfig localizationConfig, IAnimController animController)
    {
        _animController = animController;
        _localization = localizationConfig.GetConfig();
    }

    public void ShowMessage(string key)
    {
        var localizedString = _localization.Where(e => e.Key == key).Select(e => e.Value).FirstOrDefault();

        if (localizedString is not null)
        {
            _textField.text = localizedString;

            _animController.ShowMessageAnim(_textField, null);
        }
    }
}

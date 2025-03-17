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
    private ELanguage _textLanguage;

    [Inject]
    public void Initialize(ILocalizationConfig localizationConfig, IAnimController animController, IGameConfig gameConfig)
    {
        _textLanguage = gameConfig.GetTextLanguage();
        _animController = animController;
        _localization = localizationConfig.GetConfig();
    }

    public void ShowMessage(string key)
    {
        var localizationStrings = _localization.Where(e => e.Key == key).Select(e => e.Value).FirstOrDefault();

        if (localizationStrings is null)
            return;

        var localizedString = localizationStrings.Where(e => e.language == _textLanguage).Select(e => e.localizedString).FirstOrDefault();

        if (localizedString is not null)
        {
            _textField.text = localizedString;

            _animController.ShowMessageAnim(_textField, null);
        }
    }
}

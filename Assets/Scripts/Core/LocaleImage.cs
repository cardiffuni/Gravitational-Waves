using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using TMPro;

public class LocaleImage : MonoBehaviour
{
    public string locale;
    public Button button;

    IEnumerator Start()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;

        button.onClick.AddListener(LocaleSelected);
    }

    static void LocaleSelected() {
        if (LocalizationSettings.AvailableLocales.Locales.Any(x => x.name == locale)){
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.First(x => x.name == locale);
        }
    }
}
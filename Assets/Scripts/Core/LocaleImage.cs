using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LocaleImage : MonoBehaviour {
    public string Locale;
    public Button Button;

    IEnumerator Start()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;

        Button.onClick.AddListener(LocaleSelected);
    }



    public void LocaleSelected() {
        if (LocalizationSettings.AvailableLocales.Locales.Any(x => x.name.ToLower().Contains(Locale.ToLower()))){
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.First(x => x.name.ToLower().Contains(Locale.ToLower()));
        }
    }
}
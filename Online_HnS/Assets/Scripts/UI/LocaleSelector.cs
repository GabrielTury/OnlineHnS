using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    private bool active = false;

    public void ChangeLocale()
    {
        if (active)
        {
            return;
        }
        StartCoroutine(SetLocale());
    }

    private IEnumerator SetLocale()
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        var localeID = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[UIUtils.Wrap(localeID + 1, -1, LocalizationSettings.AvailableLocales.Locales.Count - 1)];
        active = false;
    }
}

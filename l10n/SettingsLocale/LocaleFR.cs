using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Source de localisation pour le français.
/// </summary>
public class LocaleFR : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Initialise une nouvelle instance de la classe <see cref="LocaleFR"/>.
    /// </summary>
    /// <param name="setting">Paramètre de configuration.</param>
    public LocaleFR(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "BASIQUE" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Boutons" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Commutateurs de fonctionnalité" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Menus déroulants" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Enregistrer automatiquement" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} par Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Récupérer tous les packages au redémarrage" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Détermine s’il faut récupérer tous les packages (Cubemaps, LUTs) au redémarrage. Lorsqu’activé, Lumina rechargera tous les packages à chaque redémarrage du jeu, garantissant l’utilisation des versions les plus récentes et nettoyant les textures qui n’ont pas été chargées correctement." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Sauvegarde vos paramètres automatiquement lorsque l’interface utilisateur est ouverte ou fermée." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Documentation Lumina" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Ouvre les guides Lumina pour des informations et de la documentation dans un nouvel onglet." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Obtenir du support" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Vous avez des problèmes ? Cliquez sur ce bouton pour créer une archive ZIP de vos logs et paramètres, ouvre le dossier pour la trouver, et ouvre automatiquement notre invitation Discord pour rejoindre notre canal de support." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Faire un don" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Ouvre l’assistant de don PayPal dans un nouvel onglet. Merci de considérer me soutenir !" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Ouvrir le fichier des paramètres sauvegardés" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Ouvre le fichier des paramètres sauvegardés dans un nouvel onglet." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Utiliser le curseur de l’heure de la journée" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Affiche un curseur pour changer l’heure de la journée entre le jour et la nuit. Désactivé par défaut si Weather Plus ou un autre mode de changement d’heure est détecté." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Plus d’options peuvent être ajustées directement dans l’interface du jeu." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Réglages de latitude et longitude" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Activez ceci pour permettre des changements manuels de latitude et longitude via l’interface du jeu." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Active le MetroFramework pour Lumina, offrant une expérience UI plus moderne. Ceci est activé par défaut. L’option plein écran fenêtré doit être activée pour de meilleurs résultats." },
    };

    public void Unload()
    {
    }
}

using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Fuente de localización para Español.
/// </summary>
public class LocaleES : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="LocaleES"/>.
    /// </summary>
    /// <param name="setting">Parámetro de configuración.</param>
    public LocaleES(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "BÁSICO" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Botones" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Activación de funciones" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Desplegables" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Guardar automáticamente" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} por Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Cargar todos los paquetes al reiniciar" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Determina si se deben cargar todos los paquetes (Cubemaps, LUTs) al reiniciar. Al estar activado, Lumina recargará todos los paquetes cada vez que se reinicie el juego, asegurando el uso de versiones actualizadas y limpiando texturas que no se cargaron correctamente." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Guarda tu configuración automáticamente al abrir o cerrar la interfaz." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Documentación de Lumina" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Abre las guías y documentación de Lumina en una nueva pestaña." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Obtener soporte" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "¿Tienes problemas? Haz clic en este botón para crear un ZIP con tus registros y configuraciones, se abrirá la carpeta que lo contiene y también se abrirá automáticamente nuestro Discord para que puedas unirte al canal de soporte." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Donar" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Abre el asistente de donación de PayPal en una nueva pestaña. ¡Gracias por considerar apoyarme!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Abrir archivo de configuración guardado" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Abre el archivo de configuración guardado en una nueva pestaña." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Usar deslizador de hora del día" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Muestra un deslizador para cambiar entre día y noche. Está desactivado por defecto si se detecta Weather Plus u otro modo que cambia el tiempo." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Más opciones pueden ajustarse directamente en la interfaz del juego." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Ajustes de latitud y longitud" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Activa esto para permitir ajustes manuales de latitud y longitud desde la interfaz del juego." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Activa la MetroFramework en Lumina, ofreciendo una experiencia de interfaz más moderna. Está activado por defecto. Para mejores resultados, asegúrate de tener activada la opción de pantalla completa sin bordes." },

        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Activar volumen de Lumina" },
{ setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Requerido para que Lumina funcione correctamente. Activa un volumen de Unity HDRP que aplica configuraciones personalizadas de iluminación y post-procesamiento. Esto permite que los efectos visuales avanzados de Lumina, como la corrección de color y el mapeo de tonos, funcionen correctamente en el juego." },

    };

    public void Unload()
    {
    }
}

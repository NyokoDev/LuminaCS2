using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Fonte de localização para Português.
/// </summary>
public class LocalePT : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="LocalePT"/>.
    /// </summary>
    /// <param name="setting">Parâmetro de configuração.</param>
    public LocalePT(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "BÁSICO" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Botões" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Alternar Recursos" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Menus Suspensos" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Salvar automaticamente" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} por Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Recarregar todos os pacotes ao reiniciar" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Define se deve carregar todos os pacotes (Cubemaps, LUTs) ao reiniciar. Quando ativado, Lumina recarregará todos os pacotes sempre que o jogo reiniciar, garantindo o uso das versões mais recentes e limpando quaisquer texturas que não foram carregadas corretamente." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Salva suas configurações automaticamente ao abrir ou fechar a interface do usuário." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Documentação Lumina" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Abre os guias Lumina para informações e documentação em uma nova aba." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Obter Suporte" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Está com problemas? Clique neste botão para criar um ZIP com seus logs e configurações, abrir a pasta para encontrá-lo, e abrir automaticamente nosso convite do Discord para que você possa entrar no nosso canal de suporte." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Doar" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Abre o assistente de doação do PayPal em uma nova aba. Obrigado por considerar me apoiar!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Abrir arquivo de configurações salvas" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Abre o arquivo de configurações salvas em uma nova aba." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Usar controle deslizante de horário do dia" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Mostra um controle deslizante para alternar entre dia e noite. Desativado por padrão se o Weather Plus ou outro modo de mudança de tempo for detectado." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Mais opções podem ser ajustadas diretamente na interface do jogo." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Ajustes de Latitude e Longitude" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Ative isso para permitir alterações manuais de latitude e longitude através da interface do jogo." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Ativa o MetroFramework para Lumina, permitindo uma experiência de UI mais moderna. Isso está ativado por padrão. A opção de janela em tela cheia deve estar habilitada para melhores resultados." },

        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Ativar Volume Lumina" },
{ setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Necessário para o funcionamento correto do Lumina. Ativa um volume Unity HDRP que aplica configurações personalizadas de iluminação e pós-processamento. Isso permite que os efeitos visuais avançados do Lumina, como correção de cor e tone mapping, funcionem corretamente no jogo." },

    };

    public void Unload()
    {
    }
}

namespace TubaWinUi3.Models;

public sealed class HardwareInfoItem
{
    public required string Label { get; init; }

    public required string Value { get; set; }

    public string? BrandKey { get; set; }

    public bool IsVerified { get; set; }

    public string? NicknameValue { get; set; }

    /// <summary>显示用标签：按当前界面语言翻译（Label 本身是数据键，服务内部比较仍用中文）。</summary>
    public string DisplayLabel => Services.LocalizationService.TranslateHardwareLabel(Label);

    /// <summary>详情行提示（模板内 ToolTip 走绑定——附加属性不能用 Uid）。</summary>
    public string CopyHint => Services.LocalizationService.L("Hw_CopyHint", "点击复制");
}

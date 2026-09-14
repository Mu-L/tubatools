using System.ComponentModel;
using System.Runtime.CompilerServices;
using TubaWinUi3.Models;
using TubaWinUi3.Services;

namespace TubaWinUi3.Pages;

/// <summary>游戏选择网格里的一张卡片（内置档案或自定义游戏）。</summary>
public sealed class TunnelGameCard
{
    public GamePreset? Preset { get; init; }
    public CustomGame? Custom { get; init; }

    /// <summary>末尾的「自定义游戏」入口卡片。</summary>
    public bool IsAddCard { get; init; }

    public string Id => Preset?.Id ?? Custom?.Id ?? "";

    public string Name => IsAddCard ? "自定义游戏" : Preset?.Name ?? Custom?.Name ?? "";

    public string Glyph => IsAddCard ? "\uE710" : Preset?.Glyph ?? "\uE7FC";

    public string PortText => IsAddCard ? "" : $"端口 {(Preset?.DefaultPort ?? Custom?.Port ?? 0)}";

    public string ProtocolText
    {
        get
        {
            if (IsAddCard) return "";
            var protocol = Preset?.Protocol ?? Custom?.Protocol ?? GameTunnelProtocol.Tcp;
            return protocol.Describe();
        }
    }

    public string Tagline => IsAddCard ? "没有你的游戏？自己填端口" : Preset?.Tagline ?? Custom?.Note ?? "自定义游戏";

    public int Port => Preset?.DefaultPort ?? Custom?.Port ?? 0;

    public GameTunnelProtocol Protocol => Preset?.Protocol ?? Custom?.Protocol ?? GameTunnelProtocol.Tcp;

    /// <summary>卡片右侧的小标签：TCP / UDP。</summary>
    public string ProtocolBadge => ProtocolText;

    public bool HasProtocolBadge => !IsAddCard;

    public static TunnelGameCard FromPreset(GamePreset preset) => new() { Preset = preset };

    public static TunnelGameCard FromCustom(CustomGame game) => new() { Custom = game };

    public static TunnelGameCard AddCard() => new() { IsAddCard = true };
}

/// <summary>主页「最近联机」列表的一行。</summary>
public sealed class TunnelRecordRow
{
    public required string GameId { get; init; }
    public required string Role { get; init; }
    public required string GameName { get; init; }
    public required string Address { get; init; }
    public required int Port { get; init; }
    public GameTunnelProtocol Protocol { get; init; }
    public DateTimeOffset LastUsedUtc { get; init; }

    /// <summary>x:Bind 的按钮 Tag 用：把整行传回事件处理器。</summary>
    public TunnelRecordRow Self => this;

    public string RoleText => Role == "host" ? "我开的房" : "我加入的";

    public string Glyph => Role == "host" ? "\uE7FC" : "\uE8F1";

    /// <summary>只有自己开的房才需要「再邀请」。</summary>
    public Microsoft.UI.Xaml.Visibility InviteVisibility
        => Role == "host" ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;

    public string DetailText
    {
        get
        {
            var when = LastUsedUtc.ToLocalTime();
            var relative = DateTimeOffset.Now - when;
            var ago = relative.TotalMinutes < 1 ? "刚刚"
                : relative.TotalMinutes < 60 ? $"{(int)relative.TotalMinutes} 分钟前"
                : relative.TotalHours < 24 ? $"{(int)relative.TotalHours} 小时前"
                : relative.TotalDays < 30 ? $"{(int)relative.TotalDays} 天前"
                : when.ToString("yyyy-MM-dd");
            return $"{Protocol.Describe()} · {Port} · {ago}";
        }
    }

    public static TunnelRecordRow From(TunnelRecord record) => new()
    {
        GameId = record.GameId,
        Role = record.Role,
        GameName = string.IsNullOrWhiteSpace(record.GameName) ? "未命名游戏" : record.GameName,
        Address = record.Address ?? "",
        Port = record.Port,
        Protocol = record.Protocol,
        LastUsedUtc = record.LastUsedUtc
    };
}

/// <summary>带属性通知的简易基类（仅用于需要就地刷新的少量属性）。</summary>
public abstract class TunnelObservable : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Raise([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        Raise(name);
        return true;
    }
}

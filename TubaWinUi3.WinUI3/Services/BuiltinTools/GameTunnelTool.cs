namespace TubaWinUi3.Services;

public sealed class GameTunnelTool : IBuiltinTool
{
    public string Id => "game-tunnel";
    public string Name => "游戏联机助手";
    public string Description => "虚拟局域网联机：我的世界、泰拉瑞亚、幻兽帕鲁等，把两台电脑接进同一个虚拟网络，朋友粘贴邀请码就能进，不需要公网 IP 也不用改路由器";
    public string Glyph => "\uE774";
    public string Category => "其他工具";
    public BuiltinToolKind Kind => BuiltinToolKind.InstantAction;

    public Task ExecuteAsync(BuiltinToolContext context)
    {
        App.MainWindow?.NavigateToToolPage(typeof(TubaWinUi3.Pages.GameTunnelPage));
        return Task.CompletedTask;
    }
}

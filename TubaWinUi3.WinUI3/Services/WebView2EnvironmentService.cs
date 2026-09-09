using Microsoft.Web.WebView2.Core;

namespace TubaWinUi3.Services;

/// <summary>
/// 共享的 WebView2 环境：用户数据目录固定在 %LocalAppData%\TubaWinUi3\WebView2，
/// 避免应用安装在不可写目录（如权限受限的 D 盘目录）时，
/// WebView2 无法在 exe 旁创建默认的 *.exe.WebView2 数据目录而报错。
/// </summary>
public static class WebView2EnvironmentService
{
    // TEMP-DIAG: 临时诊断日志路径（打包环境捕获真实异常用，诊断完删除）。
    private static readonly string DiagLog = @"C:\Users\luolan\AppData\Local\TubaWinUi3\wv2-diag.log";

    private static readonly Lazy<Task<CoreWebView2Environment>> _environment = new(
        () => CoreWebView2Environment.CreateWithOptionsAsync(
            browserExecutableFolder: null,
            userDataFolder: Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "TubaWinUi3", "WebView2"),
            options: new CoreWebView2EnvironmentOptions()).AsTask());

    /// <summary>获取共享环境（所有 WebView2 实例共用同一用户数据目录与浏览器进程）。</summary>
    public static Task<CoreWebView2Environment> GetAsync()
    {
        var task = _environment.Value;
        // TEMP-DIAG
        _ = task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                try
                {
                    var ex = t.Exception?.GetBaseException() ?? new Exception("unknown");
                    Directory.CreateDirectory(Path.GetDirectoryName(DiagLog)!);
                    File.AppendAllText(DiagLog,
                        $"[{DateTime.Now:HH:mm:ss.fff}] faulted: {ex.GetType().FullName}: {ex.Message} (0x{ex.HResult:X8})\n" +
                        $"  stack: {ex.StackTrace}\n" +
                        $"  packaged={RuntimeHelper.IsMsixPackaged} localappdata={Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\n" +
                        $"  WEBVIEW2_USER_DATA_FOLDER={Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER")}\n");
                }
                catch { }
            }
        }, TaskScheduler.Default);
        return task;
    }
}

namespace PagAI.UI.Services;

public enum ToastType
{
    Success,
    Error,
    Info,
    Warning
}

public static class ToastService
{
    public static event Action<string, ToastType>? OnShow;

    public static void Success(string message)
    {
        OnShow?.Invoke(message, ToastType.Success);
    }

    public static void Error(string message)
    {
        OnShow?.Invoke(message, ToastType.Error);
    }

    public static void Info(string message)
    {
        OnShow?.Invoke(message, ToastType.Info);
    }

    public static void Warning(string message)
    {
        OnShow?.Invoke(message, ToastType.Warning);
    }
}
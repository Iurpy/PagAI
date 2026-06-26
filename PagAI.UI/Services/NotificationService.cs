namespace PagAI.UI.Services;

public static class NotificationService
{
    public static void Success(string message)
    {
        ToastService.Success(message);
    }

    public static void Error(string message)
    {
        ToastService.Error(message);
    }

    public static void Warning(string message)
    {
        ToastService.Warning(message);
    }

    public static void Info(string message)
    {
        ToastService.Info(message);
    }
}
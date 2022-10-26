namespace APRP.Web.Extensions
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}

namespace BusinessLogic.Interfaces
{
    public interface IMailService
    {
        Task SendMailAsync(string subject, string body, string toSend);
    }
}

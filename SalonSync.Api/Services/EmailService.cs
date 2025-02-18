using System.Net;
using System.Net.Mail;

namespace SalonSync.Api.Services;
public class EmailService
{
    /// <summary>
    /// envia um email de dentro do sistema usando uma conexão smtp
    /// </summary>
    /// <param name="toName">Nome Destinatario</param>
    /// <param name="toEmail">Email Destinatario</param>
    /// <param name="subject">Assunto</param>
    /// <param name="body">Corpo Email</param>
    /// <param name="fromName">Nome Remetente</param>
    /// <param name="fromEmail">Email Remetente</param>
    /// <returns>Retorna verdadeiro caso consiga e falso caso não</returns>
    public bool Send(
        string toName,
        string toEmail,
        string subject,
        string body)
    {
        //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //criando uma instancia do smtpClient
        var smtpClient = new SmtpClient(ApiConfiguration.Smtp.Host, ApiConfiguration.Smtp.Port);
        //passando as credenciais
        smtpClient.Credentials = new NetworkCredential(ApiConfiguration.Smtp.UserName, ApiConfiguration.Smtp.Password);
        //método de entrega
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //informando que to usando porta segura
        smtpClient.EnableSsl= true;
        smtpClient.UseDefaultCredentials = false;

        //criando a mensagem de email
        var mail = new MailMessage();

        mail.From = new MailAddress(ApiConfiguration.Smtp.UserName, ApiConfiguration.DefaultFromName);
        mail.To.Add(new MailAddress(toEmail, toName));
        mail.Subject = subject; //assunto
        mail.Body = body; //corpo
        mail.IsBodyHtml = true; //marca que posso mandar como html

        try
        {
            smtpClient.Send(mail);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
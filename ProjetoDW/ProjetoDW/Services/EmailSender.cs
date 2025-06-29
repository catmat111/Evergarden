using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using ProjetoDW.Models;
using System.Threading.Tasks;

namespace ProjetoDW.Services
{
    /// <summary>
    /// Serviço responsável pelo envio de emails, implementando a interface IEmailSender do ASP.NET Identity.
    /// Utiliza a biblioteca MailKit para a comunicação com o servidor SMTP.
    /// base no código de: https://macoratti.net/22/06/aspn_mailkitapi1.html
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="EmailSender"/>.
        /// As configurações de email são injetadas através do sistema de DI (Dependency Injection) da aplicação.
        /// </summary>
        /// <param name="emailSettings">Um wrapper IOptions que contém as configurações de EmailSettings lidas a partir de appsettings.json.</param>
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        /// <summary>
        /// Compõe e envia um email de forma assíncrona.
        /// </summary>
        /// <param name="email">O endereço de email do destinatário.</param>
        /// <param name="subject">O assunto do email.</param>
        /// <param name="htmlMessage">A mensagem do email em formato HTML.</param>
        /// <returns>Uma Task que representa a operação de envio assíncrona.</returns>
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Cria uma nova mensagem de email usando MimeKit.
            var mimeMessage = new MimeMessage();
            
            // Define o remetente do email, usando o nome e o email das configurações.
            mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            
            // Define o destinatário do email.
            mimeMessage.To.Add(MailboxAddress.Parse(email));
            
            // Define o assunto.
            mimeMessage.Subject = subject;

            // Cria o corpo do email, especificando que a mensagem é em HTML.
            var builder = new BodyBuilder { HtmlBody = htmlMessage };
            mimeMessage.Body = builder.ToMessageBody();

            // Usa um cliente SMTP do MailKit para enviar a mensagem.
            using var client = new SmtpClient();
            
            // Conecta-se ao servidor SMTP de forma segura (usando StartTls).
            await client.ConnectAsync(_emailSettings.Server, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            
            // Autentica-se no servidor com o nome de utilizador e password das configurações.
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            
            // Envia a mensagem de email.
            await client.SendAsync(mimeMessage);
            
            // Desconecta-se do servidor de forma limpa.
            await client.DisconnectAsync(true);
        }
    }
}
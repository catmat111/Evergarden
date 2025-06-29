namespace ProjetoDW.Models
{
    /// <summary>
    /// Representa as configurações para o serviço de envio de email (SMTP).
    /// Estes valores são tipicamente lidos de um ficheiro de configuração como appsettings.json.
    /// base no código de: https://macoratti.net/22/06/aspn_mailkitapi1.html
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// O nome do remetente a ser exibido no email (ex: "Equipa de Suporte").
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// O endereço de email do remetente.
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        /// O endereço do servidor SMTP (ex: "smtp.gmail.com").
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// A porta do servidor SMTP (ex: 587 para TLS).
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Indica se a conexão com o servidor SMTP deve usar SSL/TLS.
        /// </summary>
        public bool UseSSL { get; set; }

        /// <summary>
        /// O nome de utilizador para autenticação no servidor SMTP.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// A password para autenticação no servidor SMTP.
        /// </summary>
        public string Password { get; set; }
    }
}
namespace ProjetoDW.Models
{
    /// <summary>
    /// Fornece um modelo para a página de erro, contendo informações sobre a solicitação que falhou.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Obtém ou define o identificador único da solicitação (Request ID).
        /// Este ID pode ser usado para rastrear o erro nos logs do servidor.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Obtém um valor que indica se o RequestId deve ser exibido.
        /// Retorna verdadeiro se o RequestId não for nulo ou vazio.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
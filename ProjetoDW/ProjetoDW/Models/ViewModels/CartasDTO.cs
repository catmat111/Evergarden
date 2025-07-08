namespace AppFotos.Models.ViewModels {

   /// <summary>
   /// dados de uma Carta, para serem usados na API
   /// </summary>
   public class CartasDTO {

      /// <summary>
      /// Título da Carta
      /// </summary>
      public string Titulo { get; set; } = string.Empty;

      /// <summary>
      /// Descrição da Carta
      /// </summary>
      public string? Descricao { get; set; }
      
      /// <summary>
      /// Data para enviar a Carta
      /// </summary>
      public DateOnly? DataEnvio { get; set; }
      
      /// <summary>
      /// Utilizador Remetente
      /// </summary>
      public int UtilizadorRemetenteFk { get; set; }

      /// <summary>
      /// Utilizador Destinatário
      /// </summary>
      public int? UtilizadorDestinatarioFk { get; set; }

      

   }
}

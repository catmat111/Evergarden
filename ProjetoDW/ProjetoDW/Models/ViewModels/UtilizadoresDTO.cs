namespace ProjetoDW.Models.ViewModels;

public class UtilizadoresDTO
{
        public int Id { get; set; }

        // Esta propriedade parece ser redundante, uma vez que IdentityUserID já estabelece a ligação.
        // A ligação real é feita através do IdentityUserID.

        

        public string Nome { get; set; }
        
        /// <summary>
        /// Propriedade usada para o upload do ficheiro de imagem. Não é mapeada para a base de dados.
        /// </summary>

        public string ImagemPath { get; set; } // O caminho para a imagem é guardado na BD.

        
        public string Telemovel { get; set; }

        public string Email { get; set; }

        
        public DateOnly? DataNascimento { get; set; }
        
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjetoDW.Models
{
    /// <summary>
    /// Representa o perfil de um utilizador no sistema, estendendo a informação
    /// base do ASP.NET Identity. Pode ser um Remetente ou um Destinatário.
    /// </summary>
    public class Utilizadores
    {
        [Key]
        public int Id { get; set; }

        // Esta propriedade parece ser redundante, uma vez que IdentityUserID já estabelece a ligação.
        // A ligação real é feita através do IdentityUserID.
        public string? IdentityUser { get; set; }

        // --- RELACIONAMENTO COM ASPNET IDENTITY (AspNetUsers) ---
        // Esta é a chave estrangeira para a tabela AspNetUsers do Identity.
        // Estabelece uma relação 1-para-1 entre um registo nesta tabela Utilizadores
        // e um registo na tabela AspNetUsers. Cada Utilizador tem uma única conta de login.
        public string IdentityUserID { get; set; }

        [Required(ErrorMessage = "Nome do Destinatário necessário!")]
        public string Nome { get; set; }
        
        /// <summary>
        /// Propriedade usada para o upload do ficheiro de imagem. Não é mapeada para a base de dados.
        /// </summary>
        [NotMapped]
        public IFormFile? Imagem { get; set; }

        [Display(Name = "Imagem")]
        public string ImagemPath { get; set; } // O caminho para a imagem é guardado na BD.

        [Required(ErrorMessage = "Telemóvel do Destinatário necessário!")]
        [Display(Name = "Telemóvel")]
        [RegularExpression(@"^9\d{8}$", ErrorMessage = "O número de telemóvel deve começar por 9 e ter 9 dígitos.")]
        public string Telemovel { get; set; }

        [Required(ErrorMessage = "Email do Destinatário necessário!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Data de Nascimento do Destinatário necessária!")]
        [Display(Name = "Data de Nascimento")]
        public DateOnly? DataNascimento { get; set; }
        
        /// <summary>
        /// Validação customizada para a data de nascimento.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DataNascimento.HasValue && DataNascimento.Value >= DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult(
                    "A data de nascimento tem de ser anterior à data atual.",
                    new[] { nameof(DataNascimento) });
            }
        }
        
        // --- RELACIONAMENTO DE AUTO-REFERÊNCIA (SELF-REFERENCING RELATIONSHIP) ---
        // Um Utilizador (Destinatário) tem UM Remetente (que também é um Utilizador). (Relação N-1)
        // Um Utilizador (Remetente) pode ter MUITOS Destinatários. (Relação 1-N)
        public int? RemetenteId { get; set; }
        
        [ForeignKey("RemetenteId")]
        public Utilizadores Remetente { get; set; }

        /// <summary>
        /// Lista de Destinatários que este Utilizador (se for um Remetente) criou.
        /// </summary>
        public List<Utilizadores> UtilizadoresDestinatarios { get; set; } = new();
    }
}

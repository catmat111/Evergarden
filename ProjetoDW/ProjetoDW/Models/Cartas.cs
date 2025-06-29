using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoDW.Models
{
    /// <summary>
    /// Representa uma carta no sistema.
    /// </summary>
    public class Cartas
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tem de dar um Título á sua carta!")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Tem de dar conteúdo á sua carta!")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Data a ser enviada")]
        public DateOnly? DataEnvio { get; set; }
        
        /// <summary>
        /// Validação customizada para garantir que a Data de Envio não é no passado.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DataEnvio.HasValue && DataEnvio.Value < DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult(
                    "A Data de Envio tem de ser superior ou igual à data atual.",
                    new[] { nameof(DataEnvio) });
            }
        }

        [Display(Name = "Data de Criação")]
        public DateOnly DataCriacao { get; set; }

        // --- RELACIONAMENTO COM UTILIZADORES (REMETENTE) ---
        // Uma Carta tem UM UtilizadorRemetente (Relação N-1)
        // Um Utilizador pode ser remetente de MUITAS Cartas (Relação 1-N)
        [Display(Name = "Remetente")]
        public int UtilizadorRemetenteFk { get; set; }

        [ForeignKey(nameof(UtilizadorRemetenteFk))]
        public Utilizadores? UtilizadorRemetente { get; set; }

        // --- RELACIONAMENTO COM UTILIZADORES (DESTINATÁRIO) ---
        // Uma Carta tem UM UtilizadorDestinatario (Relação N-1)
        // Um Utilizador pode ser destinatário de MUITAS Cartas (Relação 1-N)
        [Display(Name = "Destinatário")]
        public int? UtilizadorDestinatarioFk { get; set; }

        [ForeignKey(nameof(UtilizadorDestinatarioFk))]
        public Utilizadores? UtilizadorDestinatario { get; set; }
        
        // --- RELACIONAMENTO COM CATEGORIAS ---
        // Uma Carta pode ter MUITAS Categorias.
        // Uma Categoria pode estar em MUITAS Cartas.
        // Relação Muitos-para-Muitos (N-N). O Entity Framework criará uma tabela de junção.
        public List<Categorias> Categorias { get; set; } = new List<Categorias>();
    }
}
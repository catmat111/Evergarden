using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace ProjetoDW.Models
{
    /// <summary>
    /// Representa uma categoria que pode ser associada a uma ou mais cartas.
    /// </summary>
    public class Categorias
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Tem data?")]
        public bool Tipo { get; set; }

        [Required(ErrorMessage = "Tem de colocar um nome para a sua Categoria!")]
        [Display(Name = "Categoria")]
        public string Nome { get; set; }

        // --- RELACIONAMENTO COM IDENTITYUSER (CRIADOR) ---
        // Uma Categoria tem UM UtilizadorCriador (Relação N-1)
        // Um IdentityUser pode ser o criador de MUITAS Categorias (Relação 1-N)
        public string UtilizadorCriadorId { get; set; }

        [ForeignKey(nameof(UtilizadorCriadorId))]
        public IdentityUser UtilizadorCriador { get; set; }

        // --- RELACIONAMENTO COM CARTAS ---
        // Uma Categoria pode estar em MUITAS Cartas.
        // Uma Carta pode ter MUITAS Categorias.
        // Relação Muitos-para-Muitos (N-N). O Entity Framework usará a tabela de junção
        // que foi definida a partir do modelo 'Cartas'.
        public List<Cartas> Cartas { get; set; } = new List<Cartas>();
    }
}
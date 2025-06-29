using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjetoDW.Models
{
    /// <summary>
    /// Representa uma tarefa (to-do item) no sistema.
    /// Cada tarefa pertence a um utilizador específico.
    /// </summary>
    public class Tarefa
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Tarefa")]
        public string Nome { get; set; }

        [Display(Name = "Terminado")]
        public bool Terminado { get; set; }

        // --- RELACIONAMENTO COM IDENTITYUSER ---
        // Uma Tarefa pertence a UM Utilizador (Relação N-1)
        // Um IdentityUser pode ter MUITAS Tarefas (Relação 1-N)
        public string UtilizadorId { get; set; }

        [ForeignKey("UtilizadorId")]
        public IdentityUser Utilizador { get; set; }
    }
}
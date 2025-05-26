using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjetoDW.Models;

public class Tarefa
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Tarefa")]
    public string Nome { get; set; }

    [Display(Name = "Terminado")]
    public bool Terminado { get; set; }
    
    public string UtilizadorId { get; set; }

    [ForeignKey("UtilizadorId")]
    public IdentityUser Utilizador { get; set; }
    
}
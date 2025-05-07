using System.ComponentModel.DataAnnotations;

namespace ProjetoDW.Models;

public class Tarefa
{
    [Key]
    public int Id { get; set; }
    
    [Display(Name = "Tarefa")]
    public string Nome { get; set; }
    
    [Display(Name = "Terminado")]
    public bool Terminado { get; set; }
    
}
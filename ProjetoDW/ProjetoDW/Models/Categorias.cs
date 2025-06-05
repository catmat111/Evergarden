using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace ProjetoDW.Models;

public class Categorias
{
    [Key] public int Id { get; set; }

    [Display(Name = "Tem data?")] 
    public bool Tipo { get; set; }

    [Required(ErrorMessage = "Tem de colocar um nome para a sua Categoria!")]
    [Display(Name = "Categoria")] 
    public string Nome { get; set; }

    
    public string UtilizadorCriadorId { get; set; }

    [ForeignKey(nameof(UtilizadorCriadorId))]
    public IdentityUser UtilizadorCriador { get; set; } // Navegação


    public List<Cartas> Cartas { get; set; } = new List<Cartas>();
    
}
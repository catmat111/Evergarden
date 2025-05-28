using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace ProjetoDW.Models;

public class Categorias
{
    [Key] public int Id { get; set; }

    [Display(Name = "Tem data?")] public bool Tipo { get; set; }

    [Display(Name = "Categoria")] public string Nome { get; set; }

    public IdentityUser UtilizadorCriador { get; set; } // Navegação


    public List<Cartas> Cartas { get; set; } = new List<Cartas>();
    
}
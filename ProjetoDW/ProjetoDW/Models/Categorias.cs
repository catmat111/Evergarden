using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace ProjetoDW.Models;

public class Categorias
{
    [Key] public int Id { get; set; }
    public string UtilizadoresFk { get; set; } // FK com o ID do utilizador

    [ForeignKey("UtilizadoresFk")]
    public IdentityUser Utilizador { get; set; } // Propriedade de navegação

    [Display(Name = "Tem data?")] public bool Tipo { get; set; }

    [Display(Name = "Categoria")] public string Nome { get; set; }
    
    public IdentityUser UtilizadorCriador { get; set; } // Navegação

    
    public List<Cartas> Cartas { get; set; } = new List<Cartas>();
    

}
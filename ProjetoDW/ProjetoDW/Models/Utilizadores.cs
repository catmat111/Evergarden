using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjetoDW.Models;

public class Utilizadores
{
    public int Id { get; set; }
    public string IdentityUserId { get; set; }

    [ForeignKey("IdentityUserId")]
    public IdentityUser IdentityUser { get; set; }

    public string Nome { get; set; }
    
    [NotMapped]
    public IFormFile Imagem { get; set; }
    public string ImagemPath { get; set; } // guardado na BD
    
    [Display(Name = "Telemóvel")]
    public string Telemovel { get; set; }
    
    public string Email { get; set; }
    
    public int NIF { get; set; }
    
    [Display(Name = "Data de Nascimento")]
    public DateTime DataNascimento { get; set; }
    
}
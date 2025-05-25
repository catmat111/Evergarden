using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjetoDW.Models;

public class Utilizadores
{
    public int Id { get; set; }
    
    

    
    public string? IdentityUser { get; set; }
    
    public string IdentityUserID { get; set; }
    
    [Required]
    public string Nome { get; set; }
    
    [NotMapped]
    public IFormFile? Imagem { get; set; }
    
    [Display(Name = "Imagem")]
    public string ImagemPath { get; set; } // guardado na BD
    
    [Required]
    [Display(Name = "Telemóvel")]
    public string Telemovel { get; set; }
    
    [Required]
    public string Email { get; set; }
    
    [Required]
    [Display(Name = "Data de Nascimento")]
    public DateOnly DataNascimento { get; set; }
    
    public int? RemetenteId { get; set; }
    
    [ForeignKey("RemetenteId")]
    public Utilizadores Remetente { get; set; }

    public List<Utilizadores> UtilizadoresDestinatarios { get; set; } = new();


}
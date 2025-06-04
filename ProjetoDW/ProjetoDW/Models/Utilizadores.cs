using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjetoDW.Models;

public class Utilizadores
{
    public int Id { get; set; }
    
    public string? IdentityUser { get; set; }
    
    public string IdentityUserID { get; set; }
    
    [Required(ErrorMessage = "Nome do Destinatário necessário!")]
    public string Nome { get; set; }
    
    [NotMapped]
    public IFormFile? Imagem { get; set; }
    
    [Display(Name = "Imagem")]
    public string ImagemPath { get; set; } // guardado na BD
    
    [Required(ErrorMessage = "Telemóvel do Destinatário necessário!")]
    [Display(Name = "Telemóvel")]
    [RegularExpression(@"^9\d{8}$", ErrorMessage = "O número de telemóvel deve começar por 9 e ter 9 dígitos.")]
    public string Telemovel { get; set; }
    
    [Required(ErrorMessage = "Email do Destinatário necessário!")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Data de Nascimento do Destinatário necessária!")]
    [Display(Name = "Data de Nascimento")]
    public DateOnly? DataNascimento { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataNascimento >= DateOnly.FromDateTime(DateTime.Today))
        {
            yield return new ValidationResult(
                "A data de nascimento tem de ser anterior à data atual.",
                new[] { nameof(DataNascimento) });
        }
    }
    
    
    public int? RemetenteId { get; set; }
    
    [ForeignKey("RemetenteId")]
    public Utilizadores Remetente { get; set; }

    public List<Utilizadores> UtilizadoresDestinatarios { get; set; } = new();


}
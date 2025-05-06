using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProjetoDW.Models;

public class UtilizadoresR
{
    public int Id { get; set; }
    
    public string Nome { get; set; }
    
    public string Password { get; set; }
    
    [NotMapped]
    public IFormFile Imagem { get; set; }
    public string ImagemPath { get; set; } // guardado na BD
    
    [Display(Name = "Telemóvel")]
    public string Telemovel { get; set; }
    
    public string Email { get; set; }
    
    public int NIF { get; set; }
    
    public int Idade { get; set; }
    
    [Display(Name = "Data de Nascimento")]
    public DateTime DataNascimento { get; set; }
    
}
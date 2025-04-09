using System.ComponentModel.DataAnnotations;

namespace ProjetoDW.Models;

public class UtilizadoresD
{
    public int Id { get; set; }
    
    public string Nome { get; set; }
    
    public string Password { get; set; }
    
    public string Imagem { get; set; }
    
    [Display(Name = "Telemóvel")]
    public string Telemovel { get; set; }
    
    public string Email { get; set; }
    
    public int NIF { get; set; }
    
    public int Idade { get; set; }
    
    [Display(Name = "Data de Nascimento")]
    public DateTime DataNascimento { get; set; }
    
}
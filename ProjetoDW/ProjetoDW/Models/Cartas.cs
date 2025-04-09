using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoDW.Models;

public class Cartas
{
    [Key]
    public int Id { get; set; }
    
    [Display(Name  = "Título")]
    public string Titulo { get; set; }
    
    [Display(Name = "Descrição")]
    public string Descricao { get; set; }
    
    [Display(Name = "Tópico")]
    public string Topico { get; set; }
    
    [Display(Name = "Data a ser enviada")]
    public DateTime DataEnvio { get; set; }
    
    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; }
    
    [Display(Name = "Remetente")]
    [ForeignKey(nameof(UtilizadoresR))]
    public int UtilizadoresEFk { get; set; }
    
    [Display(Name = "Destinatário")]
    [ForeignKey(nameof(UtilizadoresD))]
    public int UtilizadoresDFk { get; set; }
}
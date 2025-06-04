using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoDW.Models;

public class Cartas
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tem de dar um Título á sua carta!")]
    [Display(Name = "Título")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "Tem de dar conteúdo á sua carta!")]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; }

    

    [Display(Name = "Data a ser enviada")]
    public DateOnly? DataEnvio { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataEnvio < DateOnly.FromDateTime(DateTime.Today))
        {
            yield return new ValidationResult(
                "A Data de Envio tem de ser superior ou igual à data atual.",
                new[] { nameof(DataEnvio) });
        }
    }

    [Display(Name = "Data de Criação")]
    public DateOnly DataCriacao { get; set; }

    [Display(Name = "Remetente")]
    public int UtilizadorRemetenteFk { get; set; }

    [ForeignKey(nameof(UtilizadorRemetenteFk))]
    public Utilizadores? UtilizadorRemetente { get; set; }

    [Display(Name = "Destinatário")]
    public int? UtilizadorDestinatarioFk { get; set; }

    [ForeignKey(nameof(UtilizadorDestinatarioFk))]
    public Utilizadores? UtilizadorDestinatario { get; set; }
    
    
    public List<Categorias> Categorias { get; set; } = new List<Categorias>();


}
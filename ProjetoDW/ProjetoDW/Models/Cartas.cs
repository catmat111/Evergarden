using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoDW.Models;

public class Cartas
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Título")]
    public string Titulo { get; set; }

    [Display(Name = "Descrição")]
    public string Descricao { get; set; }

    [Display(Name = "Tópico")] 
    public string Topico { get; set; } = "Geral";

    [Display(Name = "Data a ser enviada")]
    public DateTime DataEnvio { get; set; }

    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; }

    [Display(Name = "Remetente")]
    public int UtilizadorRemetenteFk { get; set; }

    [ForeignKey(nameof(UtilizadorRemetenteFk))]
    public Utilizadores? UtilizadorRemetente { get; set; }

    [Display(Name = "Destinatário")]
    public int UtilizadorDestinatarioFk { get; set; }

    [ForeignKey(nameof(UtilizadorDestinatarioFk))]
    public Utilizadores? UtilizadorDestinatario { get; set; }
    
    public List<Categorias> Categorias { get; set; } = new List<Categorias>();
}
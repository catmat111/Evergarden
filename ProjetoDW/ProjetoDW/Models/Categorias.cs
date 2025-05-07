using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProjetoDW.Models;

public class Categorias
{
    [Key] public int Id { get; set; }

    [Display(Name = "Tem data?")] public bool Tipo { get; set; }

    [Display(Name = "Categoria")] public string Nome { get; set; }

    [Display(Name = "Tópico")] public string Topico { get; set; }

    [Display(Name = "Data a ser enviada")] public DateTime DataEnvio { get; set; }

    [Display(Name = "Data de Criação")] public DateTime DataCriacao { get; set; }


    [Display(Name = "Utilizador")]
    [ForeignKey(nameof(UtilizadorCriador))]
    public int UtilizadoresFk { get; set; }

    public Utilizadores? UtilizadorCriador { get; set; }
    
    public List<Cartas> Cartas { get; set; } = new List<Cartas>();

}
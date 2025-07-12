using Microsoft.AspNetCore.Identity;

namespace ProjetoDW.Models.ViewModels;

public class CategoriasDTO
{
    public int Id { get; set; }

    public bool Tipo { get; set; }

    
    public string Nome { get; set; }

    // --- RELACIONAMENTO COM IDENTITYUSER (CRIADOR) ---
    // Uma Categoria tem UM UtilizadorCriador (Relação N-1)
    // Um IdentityUser pode ser o criador de MUITAS Categorias (Relação 1-N)
    public string UtilizadorCriadorId { get; set; }

    public IdentityUser UtilizadorCriador { get; set; }

    // --- RELACIONAMENTO COM CARTAS ---
    // Uma Categoria pode estar em MUITAS Cartas.
    // Uma Carta pode ter MUITAS Categorias.
    // Relação Muitos-para-Muitos (N-N). O Entity Framework usará a tabela de junção
    // que foi definida a partir do modelo 'Cartas'.
    public List<Cartas> Cartas { get; set; } = new List<Cartas>();
}
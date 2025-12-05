using System.ComponentModel.DataAnnotations;

namespace SampleApp_MPI.Models.ViewModels;

public class ClientSearchViewModel
{
    [Required(ErrorMessage = "Search term is required")]
    public string SearchTerm { get; set; }
    public ICollection<Patient>? SearchResults { get; set; }
}

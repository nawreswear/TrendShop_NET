using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class CreateRoleViewModel
    {
      
            [Required]
            [Display(Name = "Role")]
            public string RoleName { get; set; }
        
    }
}

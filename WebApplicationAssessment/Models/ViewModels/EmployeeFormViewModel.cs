using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WebApplicationAssessment.Validation;

namespace WebApplicationAssessment.Models.ViewModels
{
    public class EmployeeFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [PastDate]
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-20);

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?[0-9\s\-\(\)]{7,15}$", ErrorMessage = "Please enter a valid phone number format.")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select at least one skill.")]
        [Display(Name = "Assigned Skills")]
        public List<int> SelectedSkillIds { get; set; } = new();

        public List<SelectListItem> AvailableSkills { get; set; } = new();
    }
}

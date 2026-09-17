using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplicationAssessment.Models;
using WebApplicationAssessment.Models.ViewModels;
using WebApplicationAssessment.Repositories;

namespace WebApplicationAssessment.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _empRepository;

        public EmployeesController(IEmployeeRepository empRepository)
        {
            _empRepository = empRepository;
        }

        public async Task<IActionResult> Index()
        {

            var employees = await _empRepository.GetAllEmployeesAsync();
            return View(employees);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new EmployeeFormViewModel
            {
                AvailableSkills = await GetAvailableSkillsAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool exists = await _empRepository.EmployeeExistsAsync(model.FirstName, model.LastName, model.DateOfBirth, model.Phone);
                if (exists)
                {
                    ViewBag.DuplicateMessage = "An employee with the same Phone Number or Personal Details (First Name, Last Name, Date of Birth) already exists.";
                    model.AvailableSkills = await GetAvailableSkillsAsync();
                    return View(model);
                }

                var employee = new Employee
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Phone = model.Phone
                };

                await _empRepository.AddEmployeeAsync(employee, model.SelectedSkillIds);
                return RedirectToAction(nameof(Index));
            }

            model.AvailableSkills = await GetAvailableSkillsAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _empRepository.GetEmployeeByIdAsync(id.Value);
            if (employee == null) return NotFound();

            var viewModel = new EmployeeFormViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DateOfBirth = employee.DateOfBirth,
                Phone = employee.Phone,
                SelectedSkillIds = employee.Skills.Select(s => s.Id).ToList(),
                AvailableSkills = await GetAvailableSkillsAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                bool exists = await _empRepository.EmployeeExistsAsync(model.FirstName, model.LastName, model.DateOfBirth, model.Phone, model.Id);
                if (exists)
                {
                    ViewBag.DuplicateMessage = "Another employee with the same Phone Number or Personal Details (First Name, Last Name, Date of Birth) already exists.";
                    model.AvailableSkills = await GetAvailableSkillsAsync();
                    return View(model);
                }

                var employeeToUpdate = await _empRepository.GetEmployeeByIdAsync(id);
                if (employeeToUpdate == null) return NotFound();

                employeeToUpdate.FirstName = model.FirstName;
                employeeToUpdate.LastName = model.LastName;
                employeeToUpdate.DateOfBirth = model.DateOfBirth;
                employeeToUpdate.Phone = model.Phone;

                await _empRepository.UpdateEmployeeAsync(employeeToUpdate, model.SelectedSkillIds);

                return RedirectToAction(nameof(Index));
            }

            model.AvailableSkills = await GetAvailableSkillsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _empRepository.DeleteEmployeeAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetAvailableSkillsAsync()
        {
            var skills = await _empRepository.GetAllSkillsAsync();
            return skills.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();
        }
    }
}

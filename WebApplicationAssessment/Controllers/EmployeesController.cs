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
            try
            {
                var employees = await _empRepository.GetAllEmployeesAsync();
                return View(employees);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load employee list. Please try again later.";
                return View(Enumerable.Empty<Employee>());
            }
        }

        #region Create
        /// <summary>
        /// Displays the form for creating a new employee.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new EmployeeFormViewModel
                {
                    AvailableSkills = await GetAvailableSkillsAsync()
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load form options. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Handles the submission of the form for creating a new employee.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeFormViewModel model)
        {
            try
            {
                throw new NotImplementedException("The Create method is not yet implemented.");
                if (ModelState.IsValid)
                {
                    bool exists = await _empRepository.EmployeeExistsAsync(model.DateOfBirth, model.Phone);
                    if (exists)
                    {
                        ViewBag.DuplicateMessage = "An employee with the same Phone Number and Date of Birth already exists.";
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
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while saving the employee record. Please try again.";
            }

            model.AvailableSkills = await GetAvailableSkillsAsync();
            return View(model);
        }
        #endregion

        #region Edit
        /// <summary>
        /// Displays the form for editing an existing employee.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load employee details. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Handles the submission of the form for editing an existing employee.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    bool exists = await _empRepository.EmployeeExistsAsync(model.DateOfBirth, model.Phone, model.Id);
                    if (exists)
                    {
                        ViewBag.DuplicateMessage = "Another employee with the same Phone Number and Date of Birth already exists.";
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
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"]  = "An unexpected error occurred while updating the employee. Please try again.";
            }

            model.AvailableSkills = await GetAvailableSkillsAsync();
            return View(model);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Handles the deletion of an employee.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _empRepository.DeleteEmployeeAsync(id);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the employee. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

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

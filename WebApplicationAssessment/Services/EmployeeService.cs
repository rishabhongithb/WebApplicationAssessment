using Microsoft.EntityFrameworkCore;
using WebApplicationAssessment.Data;
using WebApplicationAssessment.Models;
using WebApplicationAssessment.Repositories;

namespace WebApplicationAssessment.Services
{
    public class EmployeeService : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all employees from the database along with their associated skill collections.
        /// </summary>
        /// <returns>A collection of <see cref="Employee"/> entities with populated skills.</returns>
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.Include(e => e.Skills).ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific employee by ID, including assigned skills.
        /// </summary>
        /// <param name="id">The ID of the employee to retrieve.</param>
        /// <returns>The <see cref="Employee"/> entity if found; otherwise, null.</returns>
        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.Include(e => e.Skills).FirstOrDefaultAsync(e => e.Id == id);
        }

        /// <summary>
        /// Retrieves all skills from the database.
        /// </summary>
        /// <returns>A collection of <see cref="Skill"/> entities.</returns>
        public async Task<IEnumerable<Skill>> GetAllSkillsAsync()
        {
            return await _context.Skills.ToListAsync();
        }


        /// <summary>
        /// Adds a new employee to the database and associates selected skills with the employee.
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="selectedSkillIds"></param>
        public async Task AddEmployeeAsync(Employee employee, IEnumerable<int> selectedSkillIds)
        {
            var selectedSkills = await _context.Skills.Where(s => selectedSkillIds.Contains(s.Id)).ToListAsync();

            foreach (var skill in selectedSkills)
            {
                employee.Skills.Add(skill);
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing employee's information and associated skills in the database.
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="selectedSkillIds"></param>
        public async Task UpdateEmployeeAsync(Employee employee, IEnumerable<int> selectedSkillIds)
        {
            employee.Skills.Clear();
            var selectedSkills = await _context.Skills.Where(s => selectedSkillIds.Contains(s.Id)).ToListAsync();

            foreach (var skill in selectedSkills)
            {
                employee.Skills.Add(skill);
            }

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an employee from the database based on the provided ID.
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Checks if an employee with the same phone number 
        /// and date of birth already exists in the database, 
        /// </summary>
        /// <param name="dateOfBirth"></param>
        /// <param name="phone"></param>
        /// <param name="excludeId"></param>
        /// <returns></returns>
        public async Task<bool> EmployeeExistsAsync(DateTime dateOfBirth, string phone, int excludeId = 0)
        {
            return await _context.Employees.AnyAsync(e =>
                    e.Id != excludeId && (
                    e.Phone.Trim() == phone.Trim() && e.DateOfBirth.Date == dateOfBirth.Date
            ));
        }
    }
}

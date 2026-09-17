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

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.Include(e => e.Skills).ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.Include(e => e.Skills).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Skill>> GetAllSkillsAsync()
        {
            return await _context.Skills.ToListAsync();
        }

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

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> EmployeeExistsAsync(string firstName, string lastName, DateTime dateOfBirth, string phone, int excludeId = 0)
        {
            return await _context.Employees.AnyAsync(e =>
                    e.Id != excludeId && (
                    e.Phone.Trim() == phone.Trim() && e.DateOfBirth.Date == dateOfBirth.Date
            ));
        }
    }
}

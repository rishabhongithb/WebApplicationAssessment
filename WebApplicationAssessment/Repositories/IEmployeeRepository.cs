using WebApplicationAssessment.Models;

namespace WebApplicationAssessment.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<IEnumerable<Skill>> GetAllSkillsAsync();
        Task AddEmployeeAsync(Employee employee, IEnumerable<int> selectedSkillIds);
        Task UpdateEmployeeAsync(Employee employee, IEnumerable<int> selectedSkillIds);
        Task DeleteEmployeeAsync(int id);
        bool EmployeeExists(int id);
    }
}

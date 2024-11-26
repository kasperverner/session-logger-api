using SessionLogger.Common;
using SessionLogger.Customers;
using SessionLogger.Projects;
using SessionLogger.Users;

namespace SessionLogger.Schedules;

public class ProjectSchedule : Schedule
{
    private ProjectSchedule() { }
    
    public ProjectSchedule(Period period, Project project, Department department, int approvedHours, CustomerContact? approvedBy = null) : base(period)
    {
        ProjectId = project.Id;
        Project = project;
        
        DepartmentId = department.Id;
        Department = department;
        
        ApprovedHours = approvedHours;
        
        ApprovedById = approvedBy?.Id;
        ApprovedBy = approvedBy;
    }

    public Guid ProjectId { get; init; }
    public Project Project { get; init; }
    public Guid DepartmentId { get; init; }
    public Department Department { get; init; }
    public Guid? ApprovedById { get; private set; }
    public CustomerContact? ApprovedBy { get; private set; }
    public int ApprovedHours { get; private set; }
    
    public void UpdateApprovedHours(int approvedHours)
    {
        if (ApprovedHours == approvedHours)
            return;
        
        ApprovedHours = approvedHours;
    }
    
    public void UpdateApprovedBy(CustomerContact approvedBy)
    {
        if (ApprovedById == approvedBy.Id)
            return;
        
        ApprovedById = approvedBy.Id;
        ApprovedBy = approvedBy;
    }
}

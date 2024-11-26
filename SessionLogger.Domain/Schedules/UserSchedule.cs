using SessionLogger.Common;
using SessionLogger.Users;

namespace SessionLogger.Schedules;

public class UserSchedule : Schedule
{
    private UserSchedule() { }
    public UserSchedule(Period period, User user, AbsenceType absenceType) : base(period)
    {
        UserId = user.Id;
        User = user;
        
        AbsenceType = absenceType;
    }

    public Guid UserId { get; init; }
    public User User { get; init; }
    public AbsenceType AbsenceType { get; private set; }
    
    public void UpdateAbsenceType(AbsenceType absenceType)
    {
        if (AbsenceType == absenceType)
            return;
        
        AbsenceType = absenceType;
    }
}
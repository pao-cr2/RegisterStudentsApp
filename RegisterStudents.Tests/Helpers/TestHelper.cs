using RegisterStudents.API.Controllers;
using RegisterStudents.API.Data;
using RegisterStudents.API.Services;

namespace RegisterStudents.Tests.Helpers
{
    public static class TestControllerFactory
    {
        public static EnrollmentsController CreateEnrollmentsController(AppDbContext context)
        {
            var service = new EnrollmentService(context);
            return new EnrollmentsController(service);
        }
    }
}

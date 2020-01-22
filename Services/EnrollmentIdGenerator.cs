using System;

namespace LibraryApi.Services
{
    public class EnrollmentIdGenerator : IGenerateEnrollmentIds
    {
        public Guid GetNewId()
        {
            return Guid.NewGuid();
        }
    }
}

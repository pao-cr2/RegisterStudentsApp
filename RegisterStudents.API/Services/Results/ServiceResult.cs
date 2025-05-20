using System.Collections.Generic;

namespace RegisterStudents.API.Services.Results
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }

        public T Data { get; set; }

        public IEnumerable<string> Errors { get; set; }

        // Constructor privado para forzar el uso de los métodos estáticos
        private ServiceResult() { }

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data,
                Errors = new List<string>()
            };
        }

        public static ServiceResult<T> Failure(IEnumerable<string> errors)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Data = default,
                Errors = errors
            };
        }

        // Overload para permitir pasar un solo string
        public static ServiceResult<T> Failure(string error)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Data = default,
                Errors = new List<string> { error }
            };
        }
    }
}

using Microsoft.EntityFrameworkCore;
using RegisterStudents.API.Data;
using RegisterStudents.API.Models;
using RegisterStudents.API.Services.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegisterStudents.API.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly AppDbContext _context;

        public TeacherService(AppDbContext context)
        {
            _context = context;
        }

        // Implementación de la propiedad requerida por la interfaz
        public IQueryable<Teacher> Teachers => _context.Teachers;

        public async Task<ServiceResult<List<Teacher>>> GetAllTeachersAsync()
        {
            try
            {
                var teachers = await _context.Teachers.ToListAsync();
                return ServiceResult<List<Teacher>>.Success(teachers);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Teacher>>.Failure("Error retrieving teachers: " + ex.Message);
            }
        }

        public async Task<ServiceResult<Teacher>> GetTeacherByIdAsync(int id)
        {
            try
            {
                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher == null)
                    return ServiceResult<Teacher>.Failure("Teacher not found.");

                return ServiceResult<Teacher>.Success(teacher);
            }
            catch (Exception ex)
            {
                return ServiceResult<Teacher>.Failure("Error retrieving teacher: " + ex.Message);
            }
        }

        public async Task<ServiceResult<Teacher>> CreateTeacherAsync(Teacher teacher)
        {
            if (string.IsNullOrWhiteSpace(teacher.FirstName) || string.IsNullOrWhiteSpace(teacher.LastName))
                return ServiceResult<Teacher>.Failure("First name and last name are required.");

            if (string.IsNullOrWhiteSpace(teacher.Email))
                return ServiceResult<Teacher>.Failure("Email is required.");

            try
            {
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
                return ServiceResult<Teacher>.Success(teacher);
            }
            catch (Exception ex)
            {
                return ServiceResult<Teacher>.Failure("Error creating teacher: " + ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> UpdateTeacherAsync(Teacher updatedTeacher)
        {
            if (string.IsNullOrWhiteSpace(updatedTeacher.FirstName) || string.IsNullOrWhiteSpace(updatedTeacher.LastName))
                return ServiceResult<bool>.Failure("First name and last name are required.");

            if (string.IsNullOrWhiteSpace(updatedTeacher.Email))
                return ServiceResult<bool>.Failure("Email is required.");

            try
            {
                var existingTeacher = await _context.Teachers.FindAsync(updatedTeacher.Id);
                if (existingTeacher == null)
                    return ServiceResult<bool>.Failure("Teacher not found.");

                existingTeacher.FirstName = updatedTeacher.FirstName;
                existingTeacher.LastName = updatedTeacher.LastName;
                existingTeacher.Email = updatedTeacher.Email;

                await _context.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure("Error updating teacher: " + ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> DeleteTeacherAsync(int id)
        {
            try
            {
                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher == null)
                    return ServiceResult<bool>.Failure("Teacher not found.");

                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure("Error deleting teacher: " + ex.Message);
            }
        }

        public async Task<bool> TeacherExistsAsync(int id)
        {
            try
            {
                return await _context.Teachers.AnyAsync(t => t.Id == id);
            }
            catch
            {
                return false;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

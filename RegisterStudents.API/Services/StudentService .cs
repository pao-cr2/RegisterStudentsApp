using Microsoft.EntityFrameworkCore;
using RegisterStudents.API.Data;
using RegisterStudents.API.Dto;
using RegisterStudents.API.Models;
using RegisterStudents.API.Services.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegisterStudents.API.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<List<StudentReadDto>>> GetAllStudentsAsync()
        {
            try
            {
                var students = await _context.Students
                    .Select(s => new StudentReadDto
                    {
                        Id = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Email = s.Email,
                        Credits = s.Credits,
                        TeacherId = s.TeacherId
                    }).ToListAsync();

                return ServiceResult<List<StudentReadDto>>.Success(students);
            }
            catch (Exception ex)
            {
                // Log ex.Message si tienes un logger configurado
                return ServiceResult<List<StudentReadDto>>.Failure("Error retrieving students: " + ex.Message);
            }
        }

        public async Task<ServiceResult<StudentReadDto>> GetStudentByIdAsync(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                    return ServiceResult<StudentReadDto>.Failure("Student not found.");

                var studentDto = new StudentReadDto
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Email = student.Email,
                    Credits = student.Credits,
                    TeacherId = student.TeacherId
                };

                return ServiceResult<StudentReadDto>.Success(studentDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<StudentReadDto>.Failure("Error retrieving student: " + ex.Message);
            }
        }

        public async Task<ServiceResult<StudentReadDto>> CreateStudentAsync(StudentCreateDto studentDto)
        {
            if (string.IsNullOrWhiteSpace(studentDto.FirstName) ||
                string.IsNullOrWhiteSpace(studentDto.LastName) ||
                string.IsNullOrWhiteSpace(studentDto.Email))
            {
                return ServiceResult<StudentReadDto>.Failure("First name, last name and email are required.");
            }

            try
            {
                var student = new Student
                {
                    FirstName = studentDto.FirstName.Trim(),
                    LastName = studentDto.LastName.Trim(),
                    Email = studentDto.Email.Trim(),
                    Credits = studentDto.Credits,
                    TeacherId = studentDto.TeacherId
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                var createdDto = new StudentReadDto
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Email = student.Email,
                    Credits = student.Credits,
                    TeacherId = student.TeacherId
                };

                return ServiceResult<StudentReadDto>.Success(createdDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<StudentReadDto>.Failure("Error creating student: " + ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> UpdateStudentAsync(StudentUpdateDto studentDto)
        {
            if (string.IsNullOrWhiteSpace(studentDto.FirstName) ||
                string.IsNullOrWhiteSpace(studentDto.LastName) ||
                string.IsNullOrWhiteSpace(studentDto.Email))
            {
                return ServiceResult<bool>.Failure("First name, last name and email are required.");
            }

            try
            {
                var student = await _context.Students.FindAsync(studentDto.Id);
                if (student == null)
                    return ServiceResult<bool>.Failure("Student not found.");

                student.FirstName = studentDto.FirstName.Trim();
                student.LastName = studentDto.LastName.Trim();
                student.Email = studentDto.Email.Trim();
                student.Credits = studentDto.Credits;
                student.TeacherId = studentDto.TeacherId;

                _context.Entry(student).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure("Error updating student: " + ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> DeleteStudentAsync(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                    return ServiceResult<bool>.Failure("Student not found.");

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure("Error deleting student: " + ex.Message);
            }
        }

        public async Task<bool> StudentExistsAsync(int id)
        {
            try
            {
                return await _context.Students.AnyAsync(s => s.Id == id);
            }
            catch
            {
                return false;
            }
        }
    }
}

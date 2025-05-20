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
    public class SubjectService : ISubjectService
    {
        private readonly AppDbContext _context;

        public SubjectService(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Subject> Subjects => _context.Subjects;

        public async Task<ServiceResult<List<Subject>>> GetAllSubjectsAsync()
        {
            try
            {
                var subjects = await _context.Subjects.ToListAsync();
                return ServiceResult<List<Subject>>.Success(subjects);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Subject>>.Failure("Error retrieving subjects: " + ex.Message);
            }
        }

        public async Task<ServiceResult<Subject>> GetSubjectByIdAsync(int id)
        {
            try
            {
                var subject = await _context.Subjects.FindAsync(id);
                if (subject == null)
                    return ServiceResult<Subject>.Failure("Subject not found.");

                return ServiceResult<Subject>.Success(subject);
            }
            catch (Exception ex)
            {
                return ServiceResult<Subject>.Failure("Error retrieving subject: " + ex.Message);
            }
        }

        public async Task<ServiceResult<Subject>> CreateSubjectAsync(Subject subject)
        {
            if (string.IsNullOrWhiteSpace(subject.Name))
                return ServiceResult<Subject>.Failure("Subject name is required.");

            try
            {
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();
                return ServiceResult<Subject>.Success(subject);
            }
            catch (Exception ex)
            {
                return ServiceResult<Subject>.Failure("Error creating subject: " + ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> UpdateSubjectAsync(Subject updatedSubject)
        {
            if (string.IsNullOrWhiteSpace(updatedSubject.Name))
                return ServiceResult<bool>.Failure("Subject name is required.");

            try
            {
                var existingSubject = await _context.Subjects.FindAsync(updatedSubject.Id);
                if (existingSubject == null)
                    return ServiceResult<bool>.Failure("Subject not found.");

                existingSubject.Name = updatedSubject.Name;
                existingSubject.Credits = updatedSubject.Credits;

                await _context.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure("Error updating subject: " + ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> DeleteSubjectAsync(int id)
        {
            try
            {
                var subject = await _context.Subjects.FindAsync(id);
                if (subject == null)
                    return ServiceResult<bool>.Failure("Subject not found.");

                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure("Error deleting subject: " + ex.Message);
            }
        }

        public async Task<bool> SubjectExistsAsync(int id)
        {
            try
            {
                return await _context.Subjects.AnyAsync(s => s.Id == id);
            }
            catch
            {
                return false;
            }
        }
    }
}

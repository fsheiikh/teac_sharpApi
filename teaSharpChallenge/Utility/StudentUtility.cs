using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using teaSharpChallenge.Models;
using Newtonsoft.Json.Linq;
using System.Web.Helpers;

namespace teaSharpChallenge.Utility
{
    public class StudentUtility
    {
        private SchoolEntities db = new SchoolEntities();

        public List<Student> GetAllFormattedStudentsData()
        {
            try
            {
                List<Student> allStudentsData = new List<Student>();

                foreach (Person person in db.People)
                {
                    if (IsStudent(person))
                        allStudentsData.Add(FormatStudentData(person.PersonID));
                }

                return allStudentsData;
            }
            catch (Exception e)
            {
                //Log/Handle Exception
                return null;
            }

        }
        public Student FormatStudentData(int personID)
        {
            try
            {
                Student student = new Student();
                Person person = db.People.Find(personID);

                if (IsStudent(person))
                {
                    student.studentId = person.PersonID;
                    student.firstName = person.FirstName;
                    student.lastName = person.LastName;
                    student.gpa = CalculateGPA(person);
                    student.grades = GetGrades(person);
                }
                else
                {
                    //Log/Handle Exception
                    return null;
                }
                return student;

            }
            catch(Exception e)
            {
                //Log Exception
                return null;
            }
        }

        //Calculate GPA via LINQ for easy manuevering and customization to calculation conditions
        private decimal? CalculateGPA(Person person)
        {
            decimal? gradePointAverage = person.StudentGrades
                .Select(stdGrade => stdGrade.Grade)
                .Where(grade => grade != null)
                .Average();

            return gradePointAverage;
        }

        private List<Grade> GetGrades(Person person)
        {
            List<Grade> grades = new List<Grade>();

            foreach (StudentGrade studentGrade in person.StudentGrades)
            {
                Grade grade = new Grade();

                grade.courseID = studentGrade.Course.CourseID;
                grade.title = studentGrade.Course.Title;
                grade.credits = studentGrade.Course.Credits;
                grade.grade = studentGrade.Grade;
                //check for NULLS

                grades.Add(grade);
            }
   
            return grades;
        }
       

        public StudentGradeResponse AddStudentGrade(StudentGradeRequest studentGradeRequest)
        {
           try
            {
                StudentGrade studentGrade = new StudentGrade();
                StudentGradeResponse response = new StudentGradeResponse();

                if (ValidateGradeRequest(studentGradeRequest))
                {
                    //edit existing student grade that is missing grade
                    if (StudentGradeExists(studentGradeRequest.studentId, studentGradeRequest.courseId))
                    {
                        studentGrade = db.StudentGrades.Where(c => c.CourseID == studentGradeRequest.courseId && c.StudentID == studentGradeRequest.studentId).SingleOrDefault();
                        studentGrade.Grade = studentGradeRequest.grade;
                        db.SaveChanges();
                    }
                    else //if student/course combo is not present, create a new one
                    {
                        Person student = db.People.Where(p => p.PersonID == studentGradeRequest.studentId).SingleOrDefault();
                        Course course = db.Courses.Where(c => c.CourseID == studentGradeRequest.courseId).SingleOrDefault();

                        studentGrade.Person = student;
                        studentGrade.Course = course;

                        studentGrade.Grade = studentGradeRequest.grade;

                        db.StudentGrades.Add(studentGrade);
                        db.SaveChanges();
                    }

                    response.gradeId = studentGrade.EnrollmentID;
                    response.courseId = studentGrade.CourseID;
                    response.studentId = studentGrade.StudentID;
                    response.grade = studentGrade.Grade;

                }
                else
                {
                    return null;
                }

                return response;
            }
            catch (Exception e)
            {
                //Log/Handle Exception
                throw;
                
            }
        }

        //check the constraints on this request and return apporpriate boolean
        public bool ValidateGradeRequest(StudentGradeRequest studentGradeRequest)
        {
            bool valid = true;
            var studentId = studentGradeRequest.studentId;
            var courseId = studentGradeRequest.courseId;

            if (!PersonExists(studentId) || !IsStudent(studentId))
                valid = false;

            if (!CourseExists(courseId))
                valid = false;

            if (!IsGradeEntryValid(studentGradeRequest.grade))
                valid = false;

            if (!CheckStudentGradeUniqueness(studentId, courseId))
                valid = false;


            return valid;
        }

        //if student already has a grade present for said course, return false as only 1 grade entry is allowed per student
        private bool CheckStudentGradeUniqueness(int? studentId, int? courseId)
        {

            bool valid = db.StudentGrades.Any(c => c.CourseID == courseId &&
                                                          c.StudentID == studentId &&
                                                          c.Grade != null) 
                                                          ? false : true;

            return valid;
        }

        private bool IsGradeEntryValid(decimal? gradeEntry)
        {
            if (gradeEntry == null)
                return true;

            if (gradeEntry >= 0.00m && gradeEntry <= 4.00m)
                return true;
            else
                return false;  
        }

        private bool IsStudent(Person person)
        {
            if (person.Discriminator.ToUpper() == "STUDENT")
                return true;
            else
                return false;
        }

        private bool IsStudent(int? id)
        {
            Person person = db.People.Find(id);

            if (PersonExists(id) && person.Discriminator.ToUpper() == "STUDENT")
                return true;
            else
                return false;
        }
        private bool PersonExists(int? personId)
        {
            return db.People.Count(p => p.PersonID == personId) > 0;
        }

        private bool CourseExists(int? courseId)
        {
            return db.Courses.Count(c => c.CourseID == courseId) > 0;
        }

        private bool StudentGradeExists(int? studentId, int? courseId)
        {
            return db.StudentGrades.Count(c => c.CourseID == courseId && c.StudentID == studentId) > 0;
        }

        public bool EntryNotFound(int? studentId, int? courseId)
        {
            if (!CourseExists(courseId) || !PersonExists(studentId))
                return true;

            return false;
        }
    }
}
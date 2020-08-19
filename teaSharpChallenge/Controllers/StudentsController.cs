using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Web.Http.Results;
using teaSharpChallenge.Models;
using teaSharpChallenge.Utility;

namespace teaSharpChallenge.Controllers
{
    public class StudentsController : ApiController
    {
        private SchoolEntities db = new SchoolEntities();
        private StudentUtility studentUtility = new StudentUtility();

        // GET: api/Students/5/transcript
        [Route("students/{id}/transcript")]
        public IHttpActionResult GetTranscript(int id)
        {
            Student student = studentUtility.FormatStudentData(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(JToken.FromObject(student));
        }

        // GET: students
        [Route("students")]
        public IHttpActionResult GetStudents()
        {
            return Ok(JToken.FromObject(studentUtility.GetAllFormattedStudentsData()));
        }

        // POST: api/Students
        [Route("grades")]
        public IHttpActionResult PostGrade(StudentGradeRequest studentGradeRequest)
        {
            if (!ModelState.IsValid || studentGradeRequest == null || !IsDataPresentInRequest(studentGradeRequest))
            {
                return BadRequest("400");
            }
            if (studentUtility.EntryNotFound(studentGradeRequest.studentId, studentGradeRequest.courseId))
            {
                return NotFound();
            }

            var response = studentUtility.AddStudentGrade(studentGradeRequest);

            if (response != null)
                return Ok(JToken.FromObject(response));
            else
                return BadRequest("406");
        }


        private bool IsDataPresentInRequest(StudentGradeRequest req)
        {
            if (req.courseId == null || req.studentId == null || req.grade == null)
                return false;
            else
                return true;

        }
    }
}
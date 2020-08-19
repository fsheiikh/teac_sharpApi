using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace teaSharpChallenge.Models
{
    public class StudentGradeRequest
    {
        public int? studentId { get; set; }

        public int? courseId { get; set; }

        public decimal? grade { get; set; }
    }
}

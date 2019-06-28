using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ConsultationApp.Models
{
    public class Consultation
    {
        public int Id { get; set; }

        public string Teacher { get; set; }

        public string Subject { get; set; }

        public Difficulties Difficulty { get; set; }

        public string Title { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime? Time { get; set; }

        //[DataType(DataType.Date)]
        //public DateTime? ValidThrough { get; set; }

        public Days Day { get; set; }

        public bool IsRegistered { get; set; }

        public string TUsername { get; set; }

        public string SUsername { get; set; }

        //public string UserID { get; set; }

        //public virtual ApplicationUser User { get; set; }

    }

    public enum Days
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5
    }

    public enum Difficulties
    {
        Exam = 1,
        Test = 2,
        [Display(Name ="Pop Quiz")]
        PopQuiz = 3

    }
}
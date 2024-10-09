namespace WebApplication1.Models
{
    public class FeedbackModels
    {
        public int Id { get; set; } // Unique identifier for the feedback
        public string TeacherEmail { get; set; } // Email of the teacher the feedback is for
        public string StudentName { get; set; } // Name of the student submitting the feedback
        public string Message { get; set; } // Feedback message from the student
        public string SubmissionDate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Models
{
    public class QuestionPutRequest
    {
        [StringLength(100)]
        public string Title { get; set; }   
        public string Content { get; set; }
    }
}
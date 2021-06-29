using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Models
{
    public class QuestionPostRequest
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please provided some content for the question")]
        public string Content { get; set; }
    }
}
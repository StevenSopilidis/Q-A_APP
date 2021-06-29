using Backend.Data.Models;

namespace Backend.Data
{
    public interface IQuestionCache
    {
        QuestionGetSingleResponse Get(int questionId);
        void Remove(int questionid);
        void Set(QuestionGetSingleResponse question);
    }
}
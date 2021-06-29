using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Data.Models;

namespace Backend.Data
{
    public interface IDataRepository
    {
        IEnumerable<QuestionGetManyResponse> GetQuestions();
        IEnumerable<QuestionGetManyResponse> GetQuestionsWithAnswers();
        IEnumerable<QuestionGetManyResponse> GetQuestionsΒySearch(string search);
        IEnumerable<QuestionGetManyResponse> GetQuestionBySearchWithPaging(string search,int pageNumber, int pageSize);
        IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions();
        Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestionsAsync();
        QuestionGetSingleResponse GetQuestion(int questionId);
        bool QuestionExists (int questionId);
        AnswerGetResponse GetAnswer (int answerId);
        
        QuestionGetSingleResponse PostQuestion(QuestionsPostFullRequest question);
        QuestionGetSingleResponse PutQuestion(int questionId, QuestionPutRequest question);
        void DeleteQuestion(int questionId);
        AnswerGetResponse PostAnswer(AnswerPostFullRequest answer);
    }
}
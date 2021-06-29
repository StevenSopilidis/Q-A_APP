using System.Collections.Generic;
using System.Linq;
using Backend.Data.Models;
using Dapper;
using static Dapper.SqlMapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Backend.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;
        public DataRepository(IConfiguration config)
        {
            _connectionString = config["ConnectionStrings:DefaultConnection"];
            
        }
        public void DeleteQuestion(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"EXEC dbo.Question_Delete @QuestionId=@QuestionId",
                    new { QuestionId= questionId}
                );
            }
        }
        public AnswerGetResponse GetAnswer(int answerId)
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<AnswerGetResponse>
                (
                    @"EXEC dbo.Answer_Get_ByAnswerId @AnswerId = @AnswerId",
                    new { AnswerId = answerId }
                );
            }
        }

        public QuestionGetSingleResponse GetQuestion(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using(GridReader results = 
                    connection.QueryMultiple(
                        @"EXEC dbo.Question_GetSingle @QuestionId=@QuestionId;
                        EXEC dbo.Answer_Get_ByQuestionId @QuestionId=@QuestionId",
                        new {QuestionId= questionId}
                    )
                )
                {
                    var question = results.Read<QuestionGetSingleResponse>().FirstOrDefault();
                    if(question != null)
                    {
                        question.Answers = results.Read<AnswerGetResponse>().ToList();
                    }
                    return question;
                }
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetQuestionBySearchWithPaging(string search, int pageNumber, int pageSize)
        {
            var parameters = new 
            {
                Search= search,
                PageNumber= pageNumber,
                PageSize= pageSize,
            };
            using(var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<QuestionGetManyResponse>
                (
                    @"EXEC dbo.Question_GetMany_BySearch_WithPaging
                    @Search= @Search,
                    @PageNumber= @PageNumber,
                    @PageSize= @PageSize",
                    parameters
                );
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetQuestions()
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<QuestionGetManyResponse>
                (@"EXEC dbo.Question_GetMany");
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetQuestionsWithAnswers()
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var questionDictionary = new Dictionary<int, QuestionGetManyResponse>();
                //dapper multi-mapping
                return connection.Query<
                    QuestionGetManyResponse,
                    AnswerGetResponse,
                    QuestionGetManyResponse
                >("EXEC dbo.Question_GetMany_With_Answers", map: (q, a) => {
                    QuestionGetManyResponse question;
                    //if question does not exist in the dictionary
                    if(!questionDictionary.TryGetValue(q.QuestionId, out question))
                    {
                        question= q;
                        question.Answers = new List<AnswerGetResponse>();
                        questionDictionary.Add(question.QuestionId, question);
                    } 
                    question.Answers.Add(a);
                    return question;
                },
                splitOn: "QuestionId"
                )
                .Distinct()
                .ToList();
            }
                
        }

        public IEnumerable<QuestionGetManyResponse> GetQuestionsΒySearch(string search)
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<QuestionGetManyResponse>(
                    @"EXEC dbo.Question_GetMany_By_Search @Search=@Search",
                    new {Search = search}
                );
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions()
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<QuestionGetManyResponse>(
                    @"EXEC dbo.Question_GetUnanswered"
                );
            }
        }

        public async Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestionsAsync()
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<QuestionGetManyResponse>(
                    "EXEC dbo.Questions_GetUnanswered"
                );
            }
        }

        public AnswerGetResponse PostAnswer(AnswerPostFullRequest answer)
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirst<AnswerGetResponse>
                (
                    @"EXEC dbo.Answer_Post @QuestionId=@QuestionId, @Content=@Content, 
                    @UserId=@UserId, @UserName=@UserName, @Created=@Created",
                    answer
                );
            }
        }

        public QuestionGetSingleResponse PostQuestion(QuestionsPostFullRequest question)
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var questionId = connection.QueryFirst<int>(
                    @"EXEC dbo.Question_Post @Title=@Title, @Content=@Contentm @UserId=@UserId,
                    @Username=@Username, @Created=@Created",
                    question
                );
                return GetQuestion(questionId);
            }
        }

        public QuestionGetSingleResponse PutQuestion(int questionId, QuestionPutRequest question)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"EXEC dbo.Question_Put @QuestionId = @QuestionId, @Title = @Title, @Content = @Content",
                    new {QuestionId = questionId, Title = question.Title, Content = question.Content}
                );
                return GetQuestion(questionId);
            }
        }

        public bool QuestionExists(int questionId)
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirst<bool>(
                    "@EXEC db.Question_Exists @QuestionId = @QuestionId",
                    new { QuestionId = questionId }
                );
            } 
        }
    }
}
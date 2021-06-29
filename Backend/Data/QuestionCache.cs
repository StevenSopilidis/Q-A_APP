using Backend.Data.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.Data
{
    public class QuestionCache : IQuestionCache
    {
        private MemoryCache _cashe { get; set;}
        public QuestionCache()
        {
            _cashe = new MemoryCache(new MemoryCacheOptions{
                SizeLimit= 100
            });
        }

        //func for generating the id of the question in cashe
        private string  GetCacheKey (int questionId) => $"Question-{questionId}";

        public QuestionGetSingleResponse Get(int questionId)
        {
            QuestionGetSingleResponse question;
            _cashe.TryGetValue(GetCacheKey(questionId), out question);
            return question;
        }

        public void Remove(int questionid)
        {
            _cashe.Remove(GetCacheKey(questionid));
        }

        public void Set(QuestionGetSingleResponse question)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);
            _cashe.Set(
                GetCacheKey(question.QuestionId),
                question,
                cacheEntryOptions
            );
        }
    }
}
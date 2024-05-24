using thread_api_asp.Entities;

namespace thread_api_asp.Repository
{
    public interface IRefreshTokenRepository
    {
        public RefreshToken? GetRefreshToken(string? accessToken);
        public int Update(RefreshToken input);
        public int Add(RefreshToken input);
    }

    public class RefreshTokenRepository(ThreadsContext context) : IRefreshTokenRepository
    {
        public RefreshToken? GetRefreshToken(string? accessToken)
        {
            var result = (
                from atk in context.RefreshTokens
                where
                    atk.Token == accessToken
                select atk).FirstOrDefault();
            return result;
        }

        public int Update(RefreshToken input)
        {
            context.RefreshTokens.Update(input);
            return context.SaveChanges();
        }

        public int Add(RefreshToken input)
        {
            context.RefreshTokens.Add(input);
            return context.SaveChanges();
        }


    }
}
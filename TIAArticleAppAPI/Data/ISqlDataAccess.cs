
namespace TIAArticleAppAPI.Services
{
    public interface ISqlDataAccess
    {
        List<T> LoadData<T, U>(string sqlStatement, U parameters);
        void SaveData<T>(string sqlStatement, T parameters);
    }
}
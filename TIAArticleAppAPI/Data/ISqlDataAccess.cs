namespace TIAArticleAppAPI.Data
{
    public interface ISqlDataAccess
    {
        List<T> LoadDataList<T, U>(string sqlStatement, U parameters);
        T LoadDataObject<T, U>(string sqlStatement, U parameters);
        async Task SaveData<T>(string sqlStatement, T parameters);
    }
}
﻿namespace TIAArticleAppAPI.Data
{
    public interface ISqlDataAccess
    {
        Task<List<T>> LoadDataList<T, U>(string sqlStatement, U parameters);
        Task<List<T>> LoadDataList<T>(string sqlStatement);
        Task<T> LoadDataObject<T, U>(string sqlStatement, U parameters);
        Task SaveData<T>(string sqlStatement, T parameters);
        Task<IEnumerable<T>> QueryRawSql<T, U>(string sqlStatement, U parameters);
    }
}
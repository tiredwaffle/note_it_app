using SQLite;

namespace ISQLiteBase
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}


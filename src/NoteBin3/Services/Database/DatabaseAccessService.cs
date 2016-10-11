using LiteDB;

namespace NoteBin3.Services.Database
{
    public class DatabaseAccessService
    {
        public static string UsersCollectionDatabaseKey => "Users";

        public LiteDatabase OpenOrCreateDefault()
        {
            //notebin3.lidb
            return new LiteDatabase("notebin3.lidb");
        }
    }
}
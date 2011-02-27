using DataAccessFramework.Querying;

namespace DataAccessFramework.UnitTest.Tables
{
    public class BlogTable : QueryTable
    {
        public BlogTable()
            : base("Blog")
        {
            ID = Field("ID");
            UserID = Field("UserID");
        }

        public readonly FieldReference ID;
        public readonly FieldReference UserID;
    }
}
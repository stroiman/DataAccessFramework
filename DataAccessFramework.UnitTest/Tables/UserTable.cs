using DataAccessFramework.Querying;

namespace DataAccessFramework.UnitTest.Tables
{
    public class UserTable : QueryTable
    {
        public UserTable()
            : base("User")
        {
            ID = Field("ID");
            Name = Field("Name");
        }

        public readonly FieldReference ID;
        public readonly FieldReference Name;
    }
}
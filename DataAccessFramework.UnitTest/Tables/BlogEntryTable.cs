using DataAccessFramework.Querying;

namespace DataAccessFramework.UnitTest.Tables
{
    public class BlogEntryTable : QueryTable
    {
        public BlogEntryTable()
            : base("BlogEntry")
        { }

        public FieldReference ID { get { return new FieldReference(this, "ID"); } }

        public FieldReference BlogID { get { return new FieldReference(this, "BlogID"); } }
    }
}
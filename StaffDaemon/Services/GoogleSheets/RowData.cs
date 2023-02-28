namespace StaffDaemon.Services.GoogleSheets
{
    public class RowData : List<string>
    {
        public RowData()
            : base()
        {
        }

        public RowData(IList<object> objects)
        {
            AddRange(objects.Cast<string>());
        }
    }
}

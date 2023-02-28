namespace StaffDaemon.Services.GoogleSheets
{
    public class RangeData : List<RowData>
    {
        public RangeData()
            : base()
        {
        }

        public RangeData(IList<IList<object>> objects)
        {
            AddRange(objects.Select(x => new RowData(x)));
        }
    }
}

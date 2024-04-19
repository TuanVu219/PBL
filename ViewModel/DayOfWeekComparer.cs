namespace PBL3Hos.ViewModel
{
    public class DayOfWeekComparer:IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var daysOfWeek=new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            return daysOfWeek.IndexOf(x).CompareTo(daysOfWeek.IndexOf(y)); 
        }
    }
}

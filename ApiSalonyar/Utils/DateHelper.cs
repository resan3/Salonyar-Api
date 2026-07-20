namespace ApiSalonyar.Utils
{
    // Helpers/DateHelper.cs
    using System.Globalization;

    public static class DateHelper
    {
        private static readonly PersianCalendar _pc = new PersianCalendar();

        public static string ToJalali(DateTime? date)
        {
            if (date == null) return "";
            return $"{_pc.GetYear(date.Value)}/{_pc.GetMonth(date.Value):00}/{_pc.GetDayOfMonth(date.Value):00}";
        }

        public static DateTime? FromJalali(string jalaliDate)
        {
            if (string.IsNullOrEmpty(jalaliDate)) return null;
            var parts = jalaliDate.Split('/');
            if (parts.Length != 3) return null;
            return _pc.ToDateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), 0, 0, 0, 0);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsBridgeController : ControllerBase
    {
        // 🔑 این رمز رو عوض کن؛ همینو بعداً توی اپ اندروید هم می‌ذاری
        private const string ApiKey = "YEK-RAMZ-TOOLANI-INJA-1234567890";

        // دیتابیس جدا، کنار برنامه — به دیتابیس EF فعلی‌ات کاری ندارد
        private static readonly string ConnStr =
            $"Data Source={Path.Combine(AppContext.BaseDirectory, "smsbridge.db")}";

        static SmsBridgeController()
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Sms(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Address TEXT, Body TEXT, Timestamp INTEGER, Device TEXT);
                CREATE TABLE IF NOT EXISTS Calls(
                    CallId TEXT PRIMARY KEY,
                    Number TEXT, Name TEXT, Type TEXT,
                    Date INTEGER, Duration INTEGER, Device TEXT);";
            cmd.ExecuteNonQuery();
        }

        private bool BadKey() => Request.Headers["X-Api-Key"].ToString() != ApiKey;

        // دریافت یک پیامک از گوشی فرستنده
        [HttpPost("sms")]
        public IActionResult PostSms(SmsDto d)
        {
            if (BadKey()) return Unauthorized();
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Sms(Address,Body,Timestamp,Device) VALUES($a,$b,$t,$d)";
            cmd.Parameters.AddWithValue("$a", d.Address);
            cmd.Parameters.AddWithValue("$b", d.Body);
            cmd.Parameters.AddWithValue("$t", d.Timestamp);
            cmd.Parameters.AddWithValue("$d", d.Device);
            cmd.ExecuteNonQuery();
            return Ok();
        }

        // دادن لیست پیامک‌ها به گوشی گیرنده
        [HttpGet("sms")]
        public IActionResult GetSms(long since = 0)
        {
            if (BadKey()) return Unauthorized();
            var list = new List<SmsDto>();
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Address,Body,Timestamp,Device FROM Sms WHERE Timestamp > $s ORDER BY Timestamp DESC LIMIT 1000";
            cmd.Parameters.AddWithValue("$s", since);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new SmsDto(r.GetString(0), r.GetString(1), r.GetInt64(2), r.GetString(3)));
            return Ok(list);
        }

        // دریافت دسته‌ای لیست تماس
        [HttpPost("calls")]
        public IActionResult PostCalls(List<CallDto> items)
        {
            if (BadKey()) return Unauthorized();
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var tx = conn.BeginTransaction();
            foreach (var d in items)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT OR REPLACE INTO Calls(CallId,Number,Name,Type,Date,Duration,Device) VALUES($id,$n,$nm,$ty,$dt,$du,$dv)";
                cmd.Parameters.AddWithValue("$id", d.CallId);
                cmd.Parameters.AddWithValue("$n", d.Number);
                cmd.Parameters.AddWithValue("$nm", d.Name);
                cmd.Parameters.AddWithValue("$ty", d.Type);
                cmd.Parameters.AddWithValue("$dt", d.Date);
                cmd.Parameters.AddWithValue("$du", d.Duration);
                cmd.Parameters.AddWithValue("$dv", d.Device);
                cmd.ExecuteNonQuery();
            }
            tx.Commit();
            return Ok(new { saved = items.Count });
        }

        // دادن لیست تماس‌ها به گوشی گیرنده
        [HttpGet("calls")]
        public IActionResult GetCalls()
        {
            if (BadKey()) return Unauthorized();
            var list = new List<CallDto>();
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CallId,Number,Name,Type,Date,Duration,Device FROM Calls ORDER BY Date DESC LIMIT 1000";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new CallDto(r.GetString(0), r.GetString(1), r.GetString(2), r.GetString(3), r.GetInt64(4), r.GetInt64(5), r.GetString(6)));
            return Ok(list);
        }

        public record SmsDto(string Address, string Body, long Timestamp, string Device);
        public record CallDto(string CallId, string Number, string Name, string Type, long Date, long Duration, string Device);
    }
}
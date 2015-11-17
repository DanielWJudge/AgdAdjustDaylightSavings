using System;
using ServiceStack.OrmLite;

namespace AgdAdjustDaylightSavings
{
    public class AgdFile
    {
        public string FileName { get; set; }
        public DateTime FirstEpoch { get; set; }
        public DateTime LastEpoch { get; set; }

        public AgdFile(string fileName)
        {
            FileName = fileName;

            using (var db = new OrmLiteConnectionFactory(GetSQLiteConnectionString(), SqliteDialect.Provider).OpenDbConnection())
            {
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = "SELECT dataTimestamp FROM data ORDER BY dataTimestamp limit 1";
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            long firstEpoch = (long)rdr[0];
                            FirstEpoch = new DateTime(firstEpoch);
                        }
                    }
                    
                    cmd.CommandText = "SELECT dataTimestamp FROM data ORDER BY dataTimestamp DESC limit 1";
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            long lastEpoch = (long)rdr[0];
                            LastEpoch = new DateTime(lastEpoch);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return FileName;
        }

        protected bool Equals(AgdFile other)
        {
            return string.Equals(FileName, other.FileName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AgdFile) obj);
        }

        public override int GetHashCode()
        {
            return (FileName != null ? FileName.GetHashCode() : 0);
        }

        public string GetSQLiteConnectionString()
        {
            const string FAIL = "FailIfMissing=True;";

            //we have to add 4 leading backslashes for UNC paths
            try
            {
                var uri = new Uri(FileName);
                if (uri.IsUnc)
                    return @"Data Source=" + "\"\\\\" + FileName + "\"" + "; Synchronous=Off; Cache Size=1048576; " + FAIL + " Legacy Format=False; auto_vacuum=1";
            }
            catch { /* unable to get URI from it, so assume it's not a UNC */ }


            return @"Data Source=" + "\"" + FileName + "\"" + "; Synchronous=Off; Cache Size=1048576; " + FAIL + " Legacy Format=False; auto_vacuum=1";
        }
    }
}
using System.Data.Odbc;
using Gavlar50.Umbraco.ImageTracker.Models;
using Umbraco.Core.Logging;

namespace Gavlar50.Umbraco.ImageTracker.DbProviders
{
    // Example Odbc specific provider if you need one
    public class OdbcProvider : IImageTrackerDbProvider
    {
        public OdbcProvider(string dbConnString)
        {
            DbConnString = dbConnString;
        }

        public string DbConnString { get; set; }

        public void AddImageTrack(ImageTrackerDbModel track)
        {
            var existsSql = string.Format("select * from gavlar50ImageTracker where ImageId={0} and PageId={1} and PropertyId={2}",
               track.ImageId, track.PageId, track.PropertyId);

            using (var conn = new OdbcConnection(DbConnString))
            {
                try
                {
                    conn.Open();
                    var odbcCommand = new OdbcCommand(existsSql, conn);
                    var rdr = odbcCommand.ExecuteReader();
                    var exists = rdr.HasRows;
                    rdr.Close();
                    if (exists) return;
                    var insertSql =
                       string.Format(
                          "insert into gavlar50ImageTracker (ImageId,PageId,PropertyId) values ({0},{1},{2})",
                          track.ImageId, track.PageId, track.PropertyId);
                    var comm = new OdbcCommand(insertSql, conn);
                    comm.ExecuteNonQuery();
                }
                catch (OdbcException e)
                {
                    LogHelper.Error<OdbcProvider>(e.Message, e);
                }
            }
        }

        public void RemoveImageTrack(int pageId)
        {
            using (var conn = new OdbcConnection(DbConnString))
            {
                try
                {
                    conn.Open();
                    var deleteSql =
                       string.Format(
                          "delete from gavlar50ImageTracker where PageId={0}",
                          pageId);
                    var comm = new OdbcCommand(deleteSql, conn);
                    comm.ExecuteNonQuery();
                }
                catch (OdbcException e)
                {
                    LogHelper.Error<OdbcProvider>(e.Message, e);
                }
            }
        }

        public void UpdateProgress(int pageId)
        {
            using (var conn = new OdbcConnection(DbConnString))
            {
                try
                {
                    conn.Open();
                    var insertSql =
                       string.Format(
                          "insert into gavlar50ImageTrackerProgress (PageId) values ({0})", pageId);
                    var comm = new OdbcCommand(insertSql, conn);
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
                catch (OdbcException e)
                {
                    LogHelper.Error<SqlProvider>(e.Message, e);
                }
            }
        }
    }
}

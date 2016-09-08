using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoogleEngine
{
    public class FatabaseWorker
    {
        string connection;

        public FatabaseWorker(string conn) {
            connection = conn;
        }
        public void IndexPage(Page childPage, string link)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertIndexedPage", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("url", link));
                        cmd.Parameters.Add(new SqlParameter("html", (childPage == null ? null : childPage.Html)));
                        cmd.Parameters.Add(new SqlParameter("dateEntry", DateTime.Now));

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Swallow errors for now, error handle in 2.0
            }
        }

        public IEnumerable<DataTable> RetrievePages()
        {
            int increment = 50;
            int startId = 1;
            int endId = 50;

            DataTable dt = new DataTable();
            do
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connection))
                    {
                        using (SqlCommand cmd = new SqlCommand("SearchPages", conn))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("startingId", startId));
                            cmd.Parameters.Add(new SqlParameter("endingId", endId));

                            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                            {
                                adapter.Fill(dt);

                                startId += increment;
                                endId += increment;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Wouldn;t normaly do this, but its a rush to production, will add error handling later
                    dt = null;
                }
                yield return dt;
            } while (dt != null && dt.Rows.Count == 50);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altunbilekler.Service
{
    public class DataProcess
    {
        public SqlConnection conn = new SqlConnection("Data Source=;Initial Catalog=;user ID=;password=;MultipleActiveResultSets=True;");

        public bool BulkCopy(DataTable dt, string targetTableName, string projectName, string connectionString)
        {

            string[] columns = { "StoreName", "Category", "SubCategory", "Brand", "SKU", "SKUCode", "Barcode", "UnitCode", "Supplier", "SupplierMark", "Supplier2", "OldPrice", "Price", "Stock", "IsStock", "CargoDetail", "CargoPrice", "URL", "DateTime", "IsStar" };


            DataTable dtDistinct = dt.Clone();

            try
            {
                dtDistinct = dt.DefaultView.ToTable(true, columns);
            }
            catch (Exception ex)
            {
            }

            while (conn.State == ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception)
                {
                }

            }

            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, transaction))
                {
                    bulkCopy.BatchSize = 10;
                    bulkCopy.DestinationTableName = targetTableName;

                    try
                    {
                        bulkCopy.WriteToServer(dtDistinct);
                        transaction.Commit();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                        transaction.Rollback();
                        conn.Close();

                        if (ex.Message.Contains("instance") || ex.Message.Contains("timeout"))
                        {
                            BulkCopy(dt, targetTableName, projectName, connectionString);
                        }

                        ErrorHelper error = new ErrorHelper();
                        error.ErrorWriteFile(ex, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\", "DbBulkCopyError.txt", projectName);
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                        dt.Clear();
                        dtDistinct.Clear();
                        GC.Collect();
                    }
                    return true;
                }
            }


        }


    }
}
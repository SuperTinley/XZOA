using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Repository.SystemManage;

namespace XZOA.Application.SystemManage
{
   public class PriceApp
    {
        IPriceRepository service = new PriceRepository();

        public List<PriceEntity> GetList()
        {
            var query = service.IQueryable();
            return query.ToList();
        }

        public List<PriceEntity> GetPurchaseList(List<string> priNOs)
        {
            var query = service.IQueryable(t=> priNOs.Contains(t.priNO));
            return query.ToList();
        }
        
        public List<PriceEntity> GetPriceList(Pagination pagination, string txt_keyword, DateTime? BeginDate, DateTime? EndDate, string chkTag)
        {
            var query = service.IQueryable();
            if(!string.IsNullOrEmpty(txt_keyword))
            {
                query = query.Where(q=>q.prdName.Contains(txt_keyword)||q.sup.Contains(txt_keyword)||q.spc.Contains(txt_keyword) || q.priNO.Contains(txt_keyword));
            }
            if(BeginDate!=null&& EndDate!=null)
            {
                EndDate = EndDate.Value.AddDays(1);
                query = query.Where(q => q.startDD>=BeginDate&& q.startDD.Value<EndDate);
            }
            if(!string.IsNullOrEmpty(chkTag))
            {
                query = query.Where(q => q.chkTag==chkTag);
            }
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.startDD).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            return query.ToList();
        }

        public PriceEntity GetFormJson(string keyValue)
        {
            var price = service.IQueryable(t=>t.priNO==keyValue).FirstOrDefault();
            return price;
        }

        public void DeleteForm(string keyValue)
        {
            service.Delete(t => t.priNO == keyValue);
        }

        public object GetTodayMax(string date)
        {
           var prices = service.IQueryable(t=>t.priNO.Contains(date));
           int maxNum = 0;
            foreach (var p in prices)
            {
                var num = Convert.ToInt32(p.priNO.Remove(0,6));
                if(num>maxNum)
                {
                    maxNum = num;
                }
            }
            return maxNum;
        }

        public string ProducePriNO()
        {
            const string po = "BJ";
            var nums = new char[3] { 'A', 'B', 'C' };
            var month = DateTime.Now.Month > 9 ? nums[DateTime.Now.Month - 10].ToString() : DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            string handleDate = DateTime.Now.Year.ToString().Remove(0, 3) + month + day;
            object maxTodayObj = GetTodayMax(handleDate);
            string tiNo = string.Empty;
            if (object.Equals(maxTodayObj, 0))
            {
                tiNo = po + handleDate + "0001";
            }
            else
            {
                int dbNum = int.Parse(maxTodayObj.ToString());
                int tempNum = dbNum + 1;
                string tempVal = string.Empty;
                if (dbNum < 9)
                    tempVal = "000" + tempNum.ToString();
                else if (dbNum == 9) tempVal = "0010";
                else if (dbNum > 9 && dbNum < 99)
                    tempVal = "00" + tempNum.ToString();
                else if (dbNum == 99) tempVal = "0100";
                else
                    tempVal = "0" + tempNum.ToString();
                tiNo = po + handleDate + tempVal;
            }

            return tiNo;
        }

        public List<PriceEntity> GetSearchList(string keyWord)
        {
            var endDate = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            var query = service.IQueryable(t => !(t.endDD.HasValue && t.endDD < endDate));
            if (!string.IsNullOrEmpty(keyWord))
            {
                query = query.Where(q => q.prdName.Contains(keyWord));
            }
            if (string.IsNullOrEmpty(keyWord))
            {
                query = query.Take(20);
            }
            return query.ToList();
        }

        public List<PriceEntity> GetSearchPrice(Pagination pagination, string keyWord)
        {
            var endDate = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            var query = service.IQueryable(t => !(t.endDD.HasValue && t.endDD<endDate));
            if (!string.IsNullOrEmpty(keyWord))
            {
                query = query.Where(q => q.prdName.Contains(keyWord)|| q.sup.Contains(keyWord) || q.spc.Contains(keyWord));
            }
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.startDD).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            return query.ToList();
        }

        /// <summary>
        /// 导入excel
        /// </summary>
        /// <returns></returns>
        public bool ImportExcel(string path, string ConnectionString)
        {
            try
            {
                string strConn;
                OleDbConnection conn;
                string sheetName;
                DataSet ds;

                FileInfo file = new FileInfo(path);
                //获取后缀名
                string fileExt = System.IO.Path.GetExtension(file.FullName);
                if (fileExt != ".xls" && fileExt != ".xlsx")
                {
                    return false;
                }
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + file.FullName + ";" + "Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";

                conn = new OleDbConnection(strConn);

                conn.Open();

                //获取所有的 sheet 表
                DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });

                ds = new DataSet();

                for (int i = 0; i < 1; i++)
                {
                    DataTable dt = new DataTable();
                    dt.TableName = "table" + i.ToString();
                    //获取表名
                    sheetName = dtSheetName.Rows[i]["TABLE_NAME"].ToString();

                    OleDbDataAdapter oleda = new OleDbDataAdapter("select * from [" + sheetName + "]", conn);

                    oleda.Fill(dt);
                    SqlConnection sqlConn = new SqlConnection(ConnectionString);
                    sqlConn.Open();
                    string sql = "";
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        var dr = dt.Rows[j];
                        //导入数据
                        string priNo = ProducePriNO();
                        string sup = dr["供应商"].ToString();
                        string prdName = dr["名称"].ToString();
                        decimal price; string unit="";
                        if (!string.IsNullOrEmpty(dr["单价"].ToString()))
                        {
                            price = Convert.ToDecimal(dr["单价"].ToString().Trim());
                        }
                        else {
                            price = 0;
                        }
                        if (!string.IsNullOrEmpty(dr["单位"].ToString()))
                        {
                            unit = dr["单位"].ToString().Trim().ToUpper();
                        }
                        string spc = dr["规格"].ToString();
                        string orderNo = dr["牌号"].ToString();
                        string conMan = dr["联系人"].ToString();
                        string tel = dr["电话"].ToString();
                        string fax = dr["传真"].ToString();
                        string rem = dr["备注"].ToString();
                        
                        string quoter = dr["报价员"].ToString();
                        decimal principal;
                        if (!string.IsNullOrEmpty(dr["税率"].ToString()))
                        {
                            principal = Convert.ToDecimal(dr["税率"].ToString()) / 100;
                        }
                        else {
                            principal = 0;
                        }
                        
                        sql += "INSERT INTO [XZOA].[dbo].[Sys_Price] ([priNO], [sup],[orderNo],[prdName],[price],[startDD],[rem],[spc],[conMan], [tel], [fax], [unit], [principal],[currency],[quoter]) VALUES ";
                        sql += "('" + priNo + "','" + sup + "','"+ orderNo + "','" + prdName + "'," + price + ",'"+ DateTime.Now + "','"+ rem + "','"+ spc + "','" + conMan + "','" + tel + "','" + fax + "','" + unit + "'," + principal + ",0,'" + quoter + "');";
                        SqlCommand cmd = new SqlCommand(sql, sqlConn);
                        if (cmd.ExecuteNonQuery()!=1)
                        {
                            return false;
                        }
                        sql = "";
                    }
                    sqlConn.Close();
                }

                //关闭连接，释放资源
                conn.Close();
                conn.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                //ErrorLogEntity error = new ErrorLogEntity();
                //error.Message = ex.Message;
                //error.Source = ex.Source;
                //error.StackTrace = ex.StackTrace;
                //error.TargetSite = ex.TargetSite.ToString();
                new ErrorLogApp().SubmitForm(ex);
                return false;
            }
        }
        public void SubmitForm(PriceEntity priceEntity, string keyValue)
        {
            
            try
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    priceEntity.priNO = keyValue;
                    service.Update(priceEntity);
                }
                else
                {
                    priceEntity.chkTag = "T";
                    service.Insert(priceEntity);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}

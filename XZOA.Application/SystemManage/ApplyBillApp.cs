/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using XZOA.Domain.Entity.SystemManage;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using XZOA.Code;

namespace XZOA.Application.SystemManage
{
    public class ApplyBillApp
    {
        private IApplyBillRepository service = new ApplyBillRepository();

        
        public List<ApplyBillEntity> GetApplyList(Pagination pagination,string departName,string keyword, DateTime? dateBeginTime, DateTime? dateEndTime, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            var query = service.IQueryable(t=>t.appDep.Equals(departName)&&t.caseTag=="F");
            if(!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.appMan.Contains(keyword) || q.prdName.Contains(keyword) || q.purMan.Contains(keyword));
            }
            if(dateBeginTime != null && dateEndTime!= null)
            {
                var endDate = DateTime.Parse(hopeDDEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                query = query.Where(q=>q.date.Value>=dateBeginTime && q.date < endDate);
            }
            if (hopeDDBeginTime != null && hopeDDEndTime != null)
            {
                var endDate = DateTime.Parse(hopeDDEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                query = query.Where(q => q.hopeDD.Value >= hopeDDBeginTime && q.hopeDD < endDate);
            }
            pagination.records = query.Count();
            query = query.OrderByDescending(q=>q.date).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            return query.ToList();
        }

        public List<ApplyBillEntity> GetApplyHistoryList(Pagination pagination, string keyword, DateTime? dateBeginTime, DateTime? dateEndTime)
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = service.IQueryable(t=>t.caseTag=="T"&&t.appMan.Equals(userName));
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.appMan.Contains(keyword) || q.prdName.Contains(keyword) || q.purMan.Contains(keyword));
            }
            if (dateBeginTime != null && dateEndTime != null)
            {
                var endDate = DateTime.Parse(dateEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                query = query.Where(q => q.date.Value >= dateBeginTime && q.date < endDate);
            }
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.date).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            return query.ToList();
        }
         
        public List<ApplyBillEntity> GetPriceList(string keyValue)
        {
            var query = service.IQueryable(t=>t.priNO.Equals(keyValue));
            return query.ToList();
        }
        
        public List<ApplyBillEntity> GetInspectionList()
        {
            var query = service.IQueryable(t=>t.purExaTag == "T"&&t.purAuthTag=="T" && t.appNum > (t.inAddNum ?? 0) && t.caseTag != "T");//
            return query.ToList();
        }

        public List<ApplyBillEntity> GetTransferList()
        {
            var query = service.IQueryable(t => t.purExaTag == "T" && t.purAuthTag == "T" && t.appNum == (t.inAddNum ?? 0) && t.caseTag != "T");
            return query.ToList();
        }

        public List<ApplyBillEntity> GetList()
        {
            var query = service.IQueryable();
            return query.ToList();
        }

        public List<ApplyBillEntity> GetPointList(string userName)
        {
            var query = service.IQueryable(t=>(t.appAuthMan.Equals(userName)&&(t.appAuthTag !="T")) 
            || (t.appExaMan.Equals(userName) && (t.appExaTag != "T")) || 
            (t.purAuthMan.Equals(userName) && (t.purAuthTag != "T")) || 
            (t.purExaMan.Equals(userName) && (t.purExaTag != "T")));
            return query.ToList();
        }


        public List<ApplyBillEntity> GetPickList()
        {
            var query = service.IQueryable(t=>t.takeNum!=null);
            return query.ToList();
        }

        public List<ApplyBillEntity> GetApplyCheckList(string userName,Pagination pagination)
        {
            var query = service.IQueryable(t => t.appExaMan.Equals(userName) && (t.appExaTag == "D"||t.appExaTag=="M") && t.caseTag!="T");
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.date).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            return query.ToList();
        }

        public List<ApplyBillEntity> GetApplyFirstCheckList()
        {
            var query = service.IQueryable(t =>t.appExaTag == "A");
            return query.ToList();
        }
        
        public int GetApplyCheckCount()
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = service.IQueryable(t => t.appExaMan.Equals(userName) && (t.appExaTag == "D" || t.appExaTag == "M") && t.caseTag != "T");
            return query.Count();
        }

        public int GetApplyApprovalCount()
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = service.IQueryable(t => t.appAuthMan.Equals(userName) && t.appExaTag == "T" && (t.appAuthTag == "D" || t.appAuthTag == "M") && t.caseTag != "T");
            return query.Count();
        }

        public int GetPurchaseCheckCount()
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = service.IQueryable(t => t.purExaMan.Equals(userName) && (t.purExaTag == "D" || t.purExaTag == "M") && !string.IsNullOrEmpty(t.purNo) && t.caseTag != "T");
            return query.Count();
        }

        public int GetPurchaseApprovalCount()
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = service.IQueryable(t => t.purAuthMan.Equals(userName) && (t.purExaTag == "T") && (t.purAuthTag == "D" || t.purAuthTag == "M") && !string.IsNullOrEmpty(t.purNo) && t.caseTag != "T");
            return query.Count();
        }

        public List<ApplyBillEntity> GetPurchaseCheckList(string userName)
        {
            var query = service.IQueryable(t => t.purExaMan.Equals(userName) && (t.purExaTag == "D"||t.purExaTag=="M") && !string.IsNullOrEmpty(t.purNo) && t.caseTag != "T");
            return query.ToList();
        }

        public List<ApplyBillEntity> GetPurchasePrintList()
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = service.IQueryable(t => !string.IsNullOrEmpty(t.purNo)&& t.purAuthTag=="T"&&t.caseTag=="F"&&t.purMan.Equals(userName));
            return query.ToList();
        }


        public List<ApplyBillEntity> GetPurchaseApprovalList(string userName)
        {
            var query = service.IQueryable(t=>t.purAuthMan.Equals(userName) && (t.purExaTag =="T") && (t.purAuthTag== "D"|| t.purAuthTag == "M") && !string.IsNullOrEmpty(t.purNo) && t.caseTag!="T");
            return query.ToList();
        }

        public List<ApplyBillEntity> GetPurchaseDetailsList()
        {
            var query = service.IQueryable(t=>t.purAuthTag=="T");//
            return query.ToList();
        }
        
        public List<ApplyBillEntity> GetApplyApprovalList(string userName, Pagination pagination)
        {
            var query = service.IQueryable(t => t.appAuthMan.Equals(userName) && t.appExaTag=="T" && (t.appAuthTag == "D"|| t.appAuthTag=="M") && t.caseTag != "T");
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.date).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            return query.ToList();
        }

        public List<ApplyBillEntity> GetApplyResultList(Pagination pagination, string keyword, string purIsTem, int? purWay,string status,int? TypeID)
        {
            var query = service.IQueryable(t => t.appAuthTag=="T"&&t.appExaTag=="T"&&t.caseTag!="T"&&t.purAuthTag!="T");
            if(!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q=>q.purSup.Contains(keyword)||q.prdName.Contains(keyword) ||q.purMan.Contains(keyword)||q.appMan.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(purIsTem))
            {
                query = query.Where(q => q.purIsTem==purIsTem);
            }
            if (TypeID != null)
            {
                query = query.Where(q => q.TypeID == TypeID);
            }
            if (purWay != null)
            {
                query = query.Where(q => q.purWay == purWay);
            }
            if (!string.IsNullOrEmpty(status)&&!status.Equals("ALL"))
            {
                if (status == "F")
                    query = query.Where(t => string.IsNullOrEmpty(t.purNo));
                else if (status == "T")
                    query = query.Where(t => !string.IsNullOrEmpty(t.purNo));
                else if(status=="N")
                    query = query.Where(t=>t.purExaTag=="F"|| t.purAuthTag == "F");
            }
            if(string.IsNullOrEmpty(keyword)&& string.IsNullOrEmpty(purIsTem)&& purWay == null&& string.IsNullOrEmpty(status))
            {
                query = query.Where(t => string.IsNullOrEmpty(t.purNo));
            }
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.date).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            return query.ToList();
        }
        
        public ApplyBillEntity GetForm(int keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void DeleteForm(int keyValue)
        {
            service.Delete(t => t.ID == keyValue);
        }
        public void SubmitForm(ApplyBillEntity applyEntity, int? keyValue=null)
        {
            try
            {
                if (keyValue != null)
                {
                    applyEntity.ID = keyValue.Value;
                    service.Update(applyEntity);
                }
                else
                {
                    service.Insert(applyEntity);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 导入excel
        /// </summary>
        /// <returns></returns>
        public bool ImportExcel(string path,string ConnectionString,string appMan, string appDep, string appExaMan, string appAuthMan)
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
                var flag = false;
                //for (int i = 0; i < dtSheetName.Rows.Count; i++)
                //{
                DataTable dt = new DataTable();
                dt.TableName = "table0";
                //获取表名
                sheetName = dtSheetName.Rows[0]["TABLE_NAME"].ToString();

                OleDbDataAdapter oleda = new OleDbDataAdapter("select * from [" + sheetName + "]", conn);

                oleda.Fill(dt);
                SqlConnection sqlConn = new SqlConnection(ConnectionString);
                sqlConn.Open();
                string sql = "";
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    var dr = dt.Rows[j];
                    //导入数据
                    string prdNo = dr["物料编码"].ToString();
                    if (string.IsNullOrEmpty(dr["名称"].ToString())||string.IsNullOrEmpty(dr["申购数量"].ToString()) || string.IsNullOrEmpty(dr["主单位"].ToString()) || string.IsNullOrEmpty(dr["期望交期"].ToString()))
                    {
                        break;
                    }
                    string prdName = dr["名称"].ToString();
                    string orderNo = dr["牌号"].ToString();
                    string spc = dr["规格"].ToString();
                    int typeID = 1;
                    if (dt.Columns.Contains("中类") && !string.IsNullOrEmpty(dr["中类"].ToString()))
                    {
                        typeID= Convert.ToInt32(dr["中类"].ToString());
                    }
                    
                    string mat = dr["材料要求"].ToString();
                    decimal appNum = Convert.ToDecimal(dr["申购数量"].ToString());
                    string appUnit = dr["主单位"].ToString();
                    decimal viceNum = Convert.ToDecimal((dr["副数量"].ToString()==""?"0": dr["副数量"].ToString()));
                    string viceUnit = dr["副单位"].ToString();
                    DateTime hopeDD = Convert.ToDateTime(dr["期望交期"].ToString());
                    string rem = dr["用途"].ToString();
                    int WAREWAY = Convert.ToInt32(dr["仓库类型"].ToString());
                sql += "INSERT INTO [XZOA].[dbo].[Sys_ApplyBill] (date,prdNo,prdName,orderNo,spc,mat,appNum,appUnit,viceNum,viceUnit,hopeDD,rem,WAREWAY,appMan,appDep,appExaMan,appAuthMan,TypeID) VALUES ";
                    sql += "(GETDATE(),'" + prdNo+"','"+ prdName+"','"+ orderNo+ "','" + spc + "','" + mat + "'," + appNum + ",'" + appUnit + "'," + viceNum + ",'" + viceUnit + "','" + hopeDD + "','" + rem + "'," + WAREWAY + ",'" + appMan + "','" + appDep + "','" + appExaMan + "','" + appAuthMan + "',"+ typeID +");";
                }
                SqlCommand cmd = new SqlCommand(sql, sqlConn);
                if (cmd.ExecuteNonQuery() == dt.Rows.Count)
                {
                    flag = true;
                }
                    sqlConn.Close();
                //}
                
                //关闭连接，释放资源
                conn.Close();
                conn.Dispose();

                return flag;
            }
            catch (Exception ex)
            {
                 new ErrorLogApp().SubmitForm(ex);
                return false;
            }
        }

        /// <summary>
        /// 产生采购单号
        /// </summary>
        /// <returns></returns>
        public string ProducePurNO(string sup,int currency)
        {
            const string po = "CG";
            var nums = new char[3] { 'A', 'B', 'C' };
            var month = DateTime.Now.Month > 9 ? nums[DateTime.Now.Month - 10].ToString() : DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            string handleDate = DateTime.Now.Year.ToString().Remove(0, 3) + month + day;
            object maxTodayObj = GetTodayMax(handleDate, currency, sup);//当天最大单号
            string tiNo = string.Empty;
            if (object.Equals(maxTodayObj, 0000))
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
        /// <summary>
        /// 获取当天最大单号
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public object GetTodayMax(string date, int currency,string sup=null)
        {
            var prices = service.IQueryable(t => t.purNo.Contains(date)).OrderByDescending(t=>t.purNo);
            int maxNum = 0000;
            int todayNum=0,supNum=0;
            string purNo="";
            if (prices.Count() > 0)
            {
                foreach (var p in prices)
                {
                    var num = Convert.ToInt32(p.purNo.Remove(0, 6));
                    if (num > todayNum)
                    {
                        todayNum = num;//当天最大单号
                    }
                }
                var purSup = prices.Where(s => s.purSup.Equals(sup));
                if (purSup.Count() > 0)
                {
                    foreach (var p in purSup)
                    {
                        var num = Convert.ToInt32(p.purNo.Remove(0, 6));
                        if (num > supNum)
                        {
                            supNum = num;//同一供应商当天最大单号
                            purNo = p.purNo;
                        }
                    }
                    var purList = service.IQueryable(s => s.purSup.Equals(sup) && s.purNo.Equals(purNo)).ToList();
                    if (purList.Count < 5)
                    {
                        maxNum = supNum - 1;
                    }
                    else
                    {
                        maxNum = todayNum;
                    }
                    foreach (var pur in purList)
                    {
                        var price = new PriceApp().GetFormJson(pur.priNO);
                        if(price.currency.Value!= currency)
                        {
                            maxNum = todayNum;
                        }
                        if (pur.prtTag == "T")//一旦有打印过  采购单号加一
                        {
                            maxNum = todayNum;
                        }
                    }
                }
                else { maxNum = todayNum; }

            }

           return maxNum;
            
        }
    }
}

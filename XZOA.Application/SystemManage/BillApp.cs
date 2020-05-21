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
    public class BillApp
    {
        private IBillRepository service = new BillRepository();
        private IPRDTRepository prdt = new PRDTRepository();
        private IPRDT1Repository prdt1 = new PRDT1Repository();
        private ITF_PSSRepository tf_pss= new TF_PSSRepository();
        private IMF_PSSRepository mf_pss = new MF_PSSRepository();
        XZOA.Data.XZOADbContext dbContext = new Data.XZOADbContext();

        public BillEntity GetForm(int keyValue)
        {
            var billEntity = service.FindEntity(t=>t.id==keyValue);
            return billEntity;
        }

        public BillEntity GetFormByBillNo(string keyValue)
        {
            var billEntity = service.FindEntity(t => t.billNo == keyValue);
            return billEntity;
        }

        public PRDTEntity CheckPrdNoAndUnit(string prdNo,string unit,int value)
        {
            PRDTEntity pRDTEntity;
            if (value == 0)
            {
                pRDTEntity = prdt.TOOLIQueryable(t=>t.PRD_NO==prdNo&&t.UT==unit).FirstOrDefault();
            }
            else {
                pRDTEntity = prdt.G_WSIQueryable(t => t.PRD_NO == prdNo && t.UT == unit).FirstOrDefault();
            }
            return pRDTEntity;
        }

        public TF_PSSEntity CheckBillNoAndErpItm(string billNo, int erpItem, int value)
        {
            TF_PSSEntity tF_PSSEntity;
            if (value == 0)
            {
                tF_PSSEntity = tf_pss.TOOLIQueryable(t => t.PRE_ITM == erpItem && t.PS_NO==billNo).FirstOrDefault();
            }
            else
            {
                tF_PSSEntity = tf_pss.G_WSIQueryable(t => t.PRE_ITM == erpItem && t.PS_NO == billNo).FirstOrDefault();
            }
            return tF_PSSEntity;
        }

        public PRDT1Entity CheckPRDT1(string prdNo, string WH, decimal num, int value)
        {
            PRDT1Entity tF_PSSEntity;
            if (value == 0)
            {
                tF_PSSEntity = prdt1.TOOLIQueryable(t => t.PRD_NO == prdNo && t.WH==WH && t.QTY >= num).FirstOrDefault();
            }
            else
            {
                tF_PSSEntity = prdt1.G_WSIQueryable(t => t.PRD_NO == prdNo && t.WH == WH && t.QTY >= num).FirstOrDefault();
            }
            return tF_PSSEntity;
        }

        public PRDT1Entity CheckPRDT1Num(string prdNo, string WH,int value)
        {
            PRDT1Entity tF_PSSEntity;
            if (value == 0)
            {
                tF_PSSEntity = prdt1.TOOLIQueryable(t => t.PRD_NO == prdNo && t.WH == WH).FirstOrDefault();
            }
            else
            {
                tF_PSSEntity = prdt1.G_WSIQueryable(t => t.PRD_NO == prdNo && t.WH == WH).FirstOrDefault();
            }
            return tF_PSSEntity;
        }


        public void SubmitForm(BillEntity applyEntity, int? keyValue=null)
        {
            if (keyValue!=null)
            {
                applyEntity.id = keyValue.Value;
                service.Update(applyEntity);
            }
            else
            {
                service.Insert(applyEntity);
            }
        }

        public List<int> GetPrintBills(string billNo)
        {
            var bills = service.IQueryable(t=> billNo.Equals(t.billNo)).Select(t=>t.id).ToList();
            return bills;
        }

        public List<BillEntity> GetPrintBillList(string billNo)
        {
            var bills = service.IQueryable(t => billNo.Equals(t.billNo)).ToList();
            return bills;
        }

        public List<BillEntity> GetBillDetailsList(string billNo)
        {
            var query = service.IQueryable(t => t.billNo == billNo);
            return query.ToList();
        }

        public List<BillEntity> GetList()
        {
            var query = service.IQueryable();
            return query.ToList();
        }

        public List<BillEntity> GetPickList()
        {
            var query = service.IQueryable(t=>t.billType=="D");
            return query.ToList();
        }

        /// <summary>
        /// 获取送检单
        /// </summary>
        /// <returns></returns>
        public List<BillEntity> GetExaList()
        {
            var query = service.IQueryable(t=>t.billType=="A"&&t.caseTag=="F");
            return query.ToList();
        }

        /// <summary>
        /// 获取进货单
        /// </summary>
        /// <returns></returns>
        public List<BillEntity> GetInList()
        {
            var query = service.IQueryable(t => t.billType == "C" && t.caseTag == "F");
            return query.ToList();
        }

        /// <summary>
        /// 获取打印单
        /// </summary>
        /// <returns></returns>
        public List<BillEntity> GetBillList()
        {
            var billLists = service.IQueryable(t => t.caseTag == "F" && t.prtTag == "F");
            
            return billLists.ToList();
        }
        /// <summary>
        /// 进货登记
        /// </summary>
        /// <returns></returns>
        public List<BillEntity> GetRegistrationList()
        {
            var billLists = service.IQueryable(t => (t.billType.Equals("C")|| t.billType.Equals("E")));

            return billLists.ToList();
        }

        /// <summary>
        /// 获取打印单
        /// </summary>
        /// <returns></returns>
        public List<BillEntity> GetPrintList()
        {
            var billLists = service.IQueryable(t => t.billType!="A");
            return billLists.ToList();
        }

        /// <summary>
        /// 获取副数量
        /// </summary>
        /// <returns></returns>
        public decimal GetViceNum(int fromID)
        {
            var inViceNum = service.IQueryable(t => t.billType == "C" && t.fromID == fromID).Sum(t => t.viceNum);
            var outViceNum = service.IQueryable(t => t.billType == "D" && t.fromID == fromID).Sum(t => t.viceNum);
            return (inViceNum??0)-(outViceNum??0);
        }
        

        /// <summary>
        /// 产生单号
        /// </summary>
        /// <returns></returns>
        public string ProduceBillNO(string sup,string title, int currency,string dep=null)
        {
            string po = "TI";
            switch(title)
            {
                case "in":
                    po = "PI";
                    break;
                case "erp":
                    po = "PI";
                    break;
                case "erp1":
                    po = "PP";
                    break;
                case "erp2":
                    po = "PP";
                    break;
                case "out":
                    po = "YB";
                    break;
                case "return":
                    po = "PP";
                    break;
                case "pick":
                    po = "ML";
                    break;
                default:
                    po = "TI";
                    break;
            }
            var nums = new char[3] { 'A', 'B', 'C' };
            var month = DateTime.Now.Month > 9 ? nums[DateTime.Now.Month - 10].ToString() : DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            string handleDate = DateTime.Now.Year.ToString().Remove(0, 3) + month + day;
            object maxTodayObj = GetTodayMax(handleDate, po,currency,sup,dep);
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
        public object GetTodayMax(string date,string po, int currency, string sup=null,string dep=null)
        {
            try
            {

                var prices = service.IQueryable(t => t.billNo.Contains(po) && t.billNo.Contains(date));
                int maxNum = 0000;
                int todayNum = 0, supNum = 0;
                string billNo = "";
                if (prices.Count() > 0)
                {
                    foreach (var p in prices)
                    {
                        var num = Convert.ToInt32(p.billNo.Remove(0, 6));
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
                            var num = Convert.ToInt32(p.billNo.Remove(0, 6));
                            if (num > supNum)
                            {
                                supNum = num;//同一供应商当天最大单号
                                billNo = p.billNo;
                            }
                        }
                        var purList = service.IQueryable(s => s.purSup.Equals(sup) && s.billNo.Equals(billNo)).ToList();
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
                            var apply = new ApplyBillApp().GetForm(pur.fromID);
                            if(pur.billNo.Contains("PP"))
                            {
                                var bill = new BillApp().GetForm(pur.fromID);
                                apply= new ApplyBillApp().GetForm(bill.fromID);
                            }
                            var price = new PriceApp().GetFormJson(apply.priNO);
                            if (price.currency.Value != currency)//币别不同 单号加一
                            {
                                maxNum = todayNum;
                            }
                            if (pur.prtTag == "T")//一旦有打印过  采购单号加一
                            {
                                maxNum = todayNum;
                            }
                            if (!string.IsNullOrEmpty(dep))
                            {
                                if (!apply.appDep.Equals(dep))
                                {
                                    maxNum = todayNum;//领用单部门不同  单号加一
                                }
                            }
                        }
                    }
                    else { maxNum = todayNum; }

                }

                return maxNum;

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}

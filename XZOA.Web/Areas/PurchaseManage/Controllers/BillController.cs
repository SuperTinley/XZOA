using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Code.Excel;
using XZOA.Domain.Entity;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Web.Areas.PurchaseManage.Controllers
{
    [HandlerLogin]
    public class BillController : ControllerBase
    {
        ApplyBillApp applyBillApp = new ApplyBillApp();
        PriceApp priceApp = new PriceApp();
        BillApp billApp = new BillApp();
        MF_PSSApp MF_PSSApp = new MF_PSSApp();
        TF_PSSApp TF_PSSApp = new TF_PSSApp();
        PRDT1App pRDT1App = new PRDT1App();
        UserApp userApp = new UserApp();
        MailHelper mHelper = new MailHelper();

        public ActionResult DeliveryIndex()
        {
            return View();
        }

        public ActionResult TransferIndex()
        {
            return View();
        }

        public ActionResult Print(string keyValue)
        {
            string[] BillNos = keyValue.Split(',');
            List<ApplyBill> billList = new List<ApplyBill>();
            foreach (var billNo in BillNos)
            {
                var Ids = billApp.GetPrintBills(billNo);
                ApplyBill applyBill = new ApplyBill();
                List<Bill> list = new List<Bill>();
                BillEntity bill = billApp.GetFormByBillNo(billNo);
                ApplyBillEntity applyBillEntity = applyBillApp.GetForm(bill.fromID);
                BillEntity inBill = null;
                if (billNo.Contains("PP"))//进货退回
                {
                    inBill=billApp.GetForm(bill.fromID);//进货单
                    applyBillEntity = applyBillApp.GetForm(inBill.fromID);
                }
                PriceEntity priceEntity = priceApp.GetFormJson(applyBillEntity.priNO);
                applyBill.billNo = bill.billNo;
                applyBill.billType = bill.billType;
                applyBill.purSup = bill.purSup;
                applyBill.ware = applyBillEntity.WAREWAY.Value;
                if(priceEntity!=null)
                {
                    applyBill.principal = Math.Round((priceEntity.principal ?? 0) * 100, 0);
                }
                applyBill.purMan = applyBillEntity.purMan ?? "";
                applyBill.PrintMan = OperatorProvider.Provider.GetCurrent().UserName;
                applyBill.appExaMan = applyBillEntity.appExaMan;
                applyBill.appMan = applyBillEntity.appMan;
                applyBill.chkDate = bill.chkDate.ToChineseDateString();
                applyBill.appDep = applyBillEntity.appDep;
                applyBill.purNo = applyBillEntity.purNo;
                if (priceEntity!=null&&priceEntity.currency.HasValue)
                {
                    var ItemId = new ItemsApp().GetItemFormByName("币别").F_Id;
                    applyBill.currency = new ItemsDetailApp().GetItemByItemIdAndCode(ItemId, priceEntity.currency.Value.ToString()).F_Description;
                }
                else
                {
                    applyBill.currency = "￥";
                }
                var applys = applyBillApp.GetList();
                var prices = priceApp.GetList();
                var bills = billApp.GetPrintBillList(billNo);
                foreach (var id in Ids)
                {
                    var bi = billApp.GetForm(id);
                    bi.prtTag = "T";
                    billApp.SubmitForm(bi,id);
                    if (billNo.Contains("PP"))//进货退回
                    {
                        var inBills = billApp.GetList();
                        list = (from b in bills
                                join i in inBills
                                on b.fromID equals i.id
                                join a in applys
                                on i.fromID equals a.ID
                                join p in prices
                                on a.priNO equals p.priNO
                                select new Bill
                                {
                                    billNo = b.billNo,
                                    useGroup = a.useGroup,
                                    priNO=p.priNO,
                                    purNo = a.purNo,
                                    prdName = p.prdName,
                                    spc = p.spc,
                                    num = Math.Round(GetNum(p.priNO, a.ID, b.id).Value, 2),
                                    unit = p.unit,
                                    viceNum = Math.Round(GetViceNum(p.priNO, a.ID, b.id).Value, 2),
                                    viceUnit = GetViceUnit(p.priNO, a.ID, b.id),
                                    price = Math.Round(p.price.Value, 4),
                                    TotalMoney = Math.Round(GetSumMoney(p.priNO, a.ID, b.id).Value, 4),
                                    appDep = a.appDep,
                                    appMan = a.appMan,
                                    remark = b.remark
                                }).ToList();
                    }
                    else {
                        list = (from b in bills
                                join a in applys
                                on b.fromID equals a.ID
                                join p in prices
                                on a.priNO equals p.priNO
                                select new Bill
                                {
                                    billNo = b.billNo,
                                    useGroup = a.useGroup,
                                    priNO = p.priNO,
                                    purNo = a.purNo,
                                    prdName = p.prdName,
                                    spc = p.spc,
                                    num = Math.Round(GetNum(p.priNO, a.ID, b.id).Value, 2),
                                    unit = p.unit,
                                    viceNum = Math.Round(GetViceNum(p.priNO, a.ID, b.id).Value, 2),
                                    viceUnit = GetViceUnit(p.priNO, a.ID, b.id),
                                    price = Math.Round(p.price??0, 4),
                                    TotalMoney = Math.Round(GetSumMoney(p.priNO, a.ID, b.id).Value, 4),
                                    appDep = a.appDep,
                                    appMan = a.appMan,
                                    remark = b.remark
                                }).ToList();
                    }
                    var SumMoney = list.Sum(l=>l.TotalMoney);
                    applyBill.TotalMoney = Math.Round(SumMoney.Value,4);
                    applyBill.bills = list;
                }
                billList.Add(applyBill);
            }
            return View(billList);
        }
        

        private decimal? GetNum(string priNO,int applyID,int billID)
        {
            var apply = applyBillApp.GetForm(applyID);
            var price = priceApp.GetFormJson(priNO);
            var bill = billApp.GetForm(billID);
            if (apply.appUnit == price.unit)
            {
                return Math.Abs(bill.num ?? 0);
            }
            else if (apply.viceUnit == price.unit)
            {
                return Math.Abs(bill.viceNum ?? 0);
            }
            else {
                return 0;
            }
        }

        private decimal? GetViceNum(string priNO, int applyID, int billID)
        {
            try
            {
                var apply = applyBillApp.GetForm(applyID);
                var price = priceApp.GetFormJson(priNO);
                var bill = billApp.GetForm(billID);
                if (apply.appUnit == price.unit)
                {
                    return Math.Abs(bill.viceNum ?? 0);
                }
                else if (apply.viceUnit == price.unit)
                {
                    return Math.Abs(bill.num ?? 0);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private decimal? GetSumMoney(string priNO, int applyID, int billID)
        {
            var apply = applyBillApp.GetForm(applyID);
            var price = priceApp.GetFormJson(priNO);
            var bill = billApp.GetForm(billID);
            if (apply.appUnit == price.unit)
            {
                return Math.Round((price.price??0) * Math.Abs((bill.num??0)), 2);
            }
            else if (apply.viceUnit == price.unit)
            {
                return Math.Round((price.price ?? 0) * Math.Abs((bill.viceNum ?? 0)), 2);
            }
            else
            {
                return 0;
            }
        }

        private string GetViceUnit(string priNO, int applyID, int billID)
        {
            var apply = applyBillApp.GetForm(applyID);
            var price = priceApp.GetFormJson(priNO);
            var bill = billApp.GetForm(billID);
            if (apply.appUnit == price.unit)
            {
                if(bill.viceNum==null||bill.viceNum==0)
                {
                    return "";
                }
                return apply.viceUnit;
            }
            else if (apply.viceUnit == price.unit)
            {
                return apply.appUnit;
            }
            else
            {
                return "";
            }
        }

        public ActionResult ReturnIndex()
        {
            return View();
        }

        public ActionResult PrintIndex()
        {
            return View();
        }

        public ActionResult RegistrationIndex()
        {
            return View();
        }

        public ActionResult PickingIndex()
        {
            return View();
        }
                 

         [HttpGet]
        public ActionResult InspectionForm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult TransferForm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ReturnForm()
        {
            return View();
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult SubmitDeliveryForm(BillEntity billEntity,string prdNo)
        {
            if (billEntity.remark == "&nbsp;")
            {
                billEntity.remark = null;
            }
            ApplyBillEntity applyBillEntity = applyBillApp.GetForm(billEntity.fromID);
            var price = priceApp.GetFormJson(applyBillEntity.priNO);
            billEntity.billNo = billApp.ProduceBillNO(billEntity.purSup, "t", price.currency.Value);
            billEntity.billType = "A";
            billEntity.chkDate = DateTime.Now;
            applyBillEntity.prdNo = prdNo;
            applyBillEntity.inNum = (applyBillEntity.inNum??0) + billEntity.num;
            applyBillEntity.inAddNum = (applyBillEntity.inAddNum??0) + billEntity.num;
            billApp.SubmitForm(billEntity);
            applyBillApp.SubmitForm(applyBillEntity,applyBillEntity.ID);
            return Success("操作成功。");
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetDeliveryList(Pagination pagination, string keyword)
        {
            var applyBills = applyBillApp.GetInspectionList();
            var prices = priceApp.GetList();
            var query = from a in applyBills
                       join p in prices
                       on a.priNO equals p.priNO
                       select new {
                           ID=a.ID,
                           hopeDD=a.hopeDD,
                           prdName=a.prdName,
                           purMan=a.purMan,
                           purSup=a.purSup,
                           purPrice=a.purPrice,
                           purNo=a.purNo,
                           orderNo=a.orderNo,
                           purOrderNo=p.orderNo,
                           spc=p.spc,
                           unit = p.unit,
                           purNO=a.purNo,
                           appNum=a.appNum,
                           inAddNum=a.inAddNum,
                           viceNum=a.viceNum,
                           viceUnit=a.viceUnit
                       };
            if(!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q=>q.prdName.Contains(keyword)||q.purSup.Contains(keyword)||q.purMan.Contains(keyword) ||q.purNO.Contains(keyword));
            }

            pagination.records = query.Count();

            query = query.OrderByDescending(q => q.hopeDD).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);

            var data = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };

            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetDeliveryTransferList(Pagination pagination, string keyword)
        {
            var bills = billApp.GetExaList();
            var applybill = applyBillApp.GetList();
            var query = from a in bills
                        join p in applybill
                        on a.fromID equals p.ID
                        select new
                        {
                            id = a.id,
                            purNo = p.purNo,
                            prdName = p.prdName,
                            spc = p.spc,
                            mat = p.mat,
                            hopeDD=p.hopeDD,
                            num=a.num,
                            convertNum=(a.inNum??0)+Math.Abs(a.outNum??0),
                            unit = p.appUnit,
                            purMan = p.purMan,
                            purSup = a.purSup,
                            billNo = a.billNo,
                            prdNo = p.prdNo,
                            WAREWAY=p.WAREWAY
                        };
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.prdName.Contains(keyword) || q.purSup.Contains(keyword) || q.purMan.Contains(keyword) || q.billNo.Contains(keyword));
            }

            pagination.records = query.Count();

            query = query.OrderByDescending(q => q.hopeDD).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);

            var data = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };

            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetPickingList(Pagination pagination, string keyword, DateTime? BeginDate, DateTime? EndDate)
        {
            var departId = OperatorProvider.Provider.GetCurrent().DepartmentId;
            var departName = new OrganizeApp().GetForm(departId).F_FullName;
            var applys = applyBillApp.GetPickList();
            var bills = billApp.GetPickList();
            var prices = priceApp.GetList();
            var query = from b in bills
                        join a in applys
                        on b.fromID equals a.ID
                        join p in prices
                        on a.priNO equals p.priNO
                        where a.appDep.Equals(departName)
                        select new {
                            id = b.id,
                            chkDate = b.chkDate.Value.Date,
                            billNo = b.billNo,
                            prdName = p.prdName,
                            spc = p.spc,
                            mat = a.mat,
                            num = b.num,
                            appUnit = a.appUnit,
                            viceNum = a.viceNum,
                            viceUnit = a.viceUnit,
                            price = p.price,
                            unit = p.unit,
                            money = GetSumMoney(p.priNO, a.ID, b.id),
                            appDep = a.appDep,
                            appMan = a.appMan,
                            useGroup = a.useGroup,
                            remark = b.remark
                        };
           
            if(!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t=> t.appMan.Contains(keyword)|| t.prdName.Contains(keyword));
            }

            if(BeginDate!=null&&EndDate!=null)
            {
                query = query.Where(t=>t.chkDate>=BeginDate.Value.Date&&t.chkDate < EndDate.Value.Date.AddDays(1));
            }

            pagination.records = query.Count();

            query = query.OrderByDescending(q => q.chkDate).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);

            var data = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };

            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetReturnList(Pagination pagination, string keyword)
        {
            var bills = billApp.GetInList();
            var applybill = applyBillApp.GetList();
            var query = from a in bills
                        join p in applybill
                        on a.fromID equals p.ID
                        where (p.yiJiaoNum??0)>0
                        select new
                        {
                            id = a.id,
                            billNo=a.billNo,
                            purNo = p.purNo,
                            prdName = p.prdName,
                            spc = p.spc,
                            mat = p.mat,
                            hopeDD = p.hopeDD,
                            num = a.num,
                            returnNum = (a.outNum ?? 0),
                            unit = p.appUnit,
                            appMan = p.appMan,
                            purSup = a.purSup,
                            appDep = p.appDep,
                            WAREWAY = p.WAREWAY,
                            yiJiaoNum=p.yiJiaoNum,
                            takeNum=p.takeNum
                        };
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => q.prdName.Contains(keyword)||q.appMan.Contains(keyword) || q.purSup.Contains(keyword) || q.billNo.Contains(keyword));
            }
            query = query.Where(t=>(t.yiJiaoNum??0) >= (t.takeNum??0));

            pagination.records = query.Count();

            query = query.OrderByDescending(q => q.hopeDD).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);

            var data = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };

            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetBillList(Pagination pagination, string keyword)
        {
            var query = billApp.GetBillList();
          
            pagination.records = query.Count();

            query = query.OrderByDescending(q => q.chkDate).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();

            var data = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };

            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetRegistrationList(Pagination pagination, string keyword, DateTime? dateBeginDate, DateTime? dateEndDate)
        {
            
            try
            {
                var bills = billApp.GetRegistrationList();
                var applys = applyBillApp.GetList();
                var prices = priceApp.GetList();
                var C = from b in bills
                        join a in applys
                        on b.fromID equals a.ID
                        join p in prices
                        on a.priNO equals p.priNO
                        where b.billType == "C"
                        select new
                        {
                            id = b.id,
                            billType = b.billType,
                            chkDate = b.chkDate,
                            priNO = a.priNO,
                            prdName = a.prdName,
                            orderNo = a.orderNo,
                            spc = a.spc,
                            purSup = a.purSup,
                            purName = p.prdName,
                            purOrderNo = p.orderNo,
                            purSpc = p.spc,
                            num = Math.Abs(b.num??0),
                            unit = a.appUnit,
                            purPrice = p.price,
                            priceRem = p.rem,
                            billRem = b.remark,
                            purMan = a.purMan,
                            billNo = b.billNo,
                            appDep = a.appDep,
                            appRemark = a.rem,
                            prtTag = b.prtTag,
                            prtDate = a.prtDate,
                            viceNum = b.viceNum,
                            viceUnit = a.viceUnit,
                            priceUnit = p.unit,
                            useGroup = a.useGroup,
                            chkTag = p.chkTag,
                            isPreMoney = a.isPreMoney,
                            FINBILLDATE = b.FINBILLDATE
                        };
                var E = from b in bills
                        join c in bills
                        on b.fromID equals c.id
                        join a in applys
                        on c.fromID equals a.ID
                        join p in prices
                        on a.priNO equals p.priNO
                        where b.billType == "E"
                        select new
                        {
                            id = b.id,
                            billType = b.billType,
                            chkDate = b.chkDate,
                            priNO = a.priNO,
                            prdName = a.prdName,
                            orderNo = a.orderNo,
                            spc = a.spc,
                            purSup = a.purSup,
                            purName = p.prdName,
                            purOrderNo = p.orderNo,
                            purSpc = p.spc,
                            num = Math.Abs(b.num ?? 0),
                            unit = a.appUnit,
                            purPrice = p.price,
                            priceRem = p.rem,
                            billRem = b.remark,
                            purMan = a.purMan,
                            billNo = b.billNo,
                            appDep = a.appDep,
                            appRemark = a.rem,
                            prtTag = b.prtTag,
                            prtDate = a.prtDate,
                            viceNum = b.viceNum,
                            viceUnit = a.viceUnit,
                            priceUnit = p.unit,
                            useGroup = a.useGroup,
                            chkTag = p.chkTag,
                            isPreMoney = a.isPreMoney,
                            FINBILLDATE = b.FINBILLDATE
                        };
                var query = C.Union(E);
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(t => t.billNo.Contains(keyword) || t.purMan.Contains(keyword) || t.purSup.Contains(keyword));
                }
                if (dateBeginDate != null && dateEndDate != null)
                {
                    var endDate = DateTime.Parse(dateEndDate.Value.AddDays(1).ToString("yyyy-MM-dd"));
                    query = query.Where(q => q.chkDate >= dateBeginDate && q.chkDate < endDate);
                }
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.chkDate).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
                var data = new
                {
                    rows = query,
                    total = pagination.total,
                    page = pagination.page,
                    records = pagination.records
                };
                return Content(data.ToJson());
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
        }
        
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetPrintList(Pagination pagination, string prtTag,string billType,string purSup,DateTime? dateBeginDate,DateTime? dateEndDate)
        {
            var bills = billApp.GetList();
            var billList = (from b in bills
                            group b by new { b.billNo, b.billType, b.prtTag, b.purSup, chkDate = new DateTime(b.chkDate.Value.Year, b.chkDate.Value.Month, b.chkDate.Value.Day) } into g
                            where g.Key.billType != "A"
                            select new
                            {
                                prtTag = g.Key.prtTag,
                                purSup = g.Key.purSup,
                                billNo = g.Key.billNo,
                                billType = g.Key.billType,
                                chkDate=g.Key.chkDate
                            }).ToList();

            if(!string.IsNullOrEmpty(prtTag)&&prtTag=="F")
            {
                billList = billList.Where(t => t.prtTag == prtTag).ToList();
            }
            if (!string.IsNullOrEmpty(prtTag) && prtTag.Equals("T"))
            {
                billList = billList.Where(t => t.prtTag == prtTag || t.prtTag == "D").ToList();
            }
            if (!string.IsNullOrEmpty(prtTag) && prtTag.Equals("ALL"))
            {
                billList = billList.Where(t => t.prtTag == "T" || t.prtTag == "F" || t.prtTag == "D").ToList();
            }
            if (!string.IsNullOrEmpty(billType))
            {
                billList = billList.Where(t => t.billType == billType).ToList();
            }

            if (dateBeginDate != null && dateEndDate != null)
            {
                var endDate = DateTime.Parse(dateEndDate.Value.AddDays(1).ToString("yyyy-MM-dd"));
                billList = billList.Where(q => q.chkDate >= dateBeginDate && q.chkDate < endDate).ToList();
            }

            if (!string.IsNullOrEmpty(purSup))
            {
                billList = billList.Where(t => t.purSup.Contains(purSup)||t.billNo.Contains(purSup)).ToList();
            }

            if (string.IsNullOrEmpty(purSup) && string.IsNullOrEmpty(prtTag) && dateBeginDate == null && dateEndDate == null)
            {
                billList = billList.Where(t => t.prtTag == "F").ToList();
            }

            pagination.records = billList.Count;

            billList = billList.OrderByDescending(q => q.chkDate).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();

            var data = new
            {
                rows = billList,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };

            return Content(data.ToJson());
        }
        
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetBillDetailsList(string billNo)
        {
            try
            {
                var applys = applyBillApp.GetList();
                var bills = billApp.GetBillDetailsList(billNo);
                var bill = billApp.GetList();
                var prices = priceApp.GetList();
                if (billNo.Contains("PP"))
                {
                    var query = (from b in bills
                                 join bi in bill
                                 on b.fromID equals bi.id
                                 join a in applys
                                 on bi.fromID equals a.ID
                                 join p in prices
                                 on a.priNO equals p.priNO
                                 select new
                                 {
                                     billNo = b.billNo,
                                     prdName = a.prdName,
                                     spc = p.spc,
                                     num = Math.Abs(b.num??0),
                                     unit = a.appUnit,
                                     viceNum = Math.Abs(b.viceNum ?? 0),
                                     viceUnit = a.viceUnit,
                                     appDep = a.appDep,
                                     appMan = a.appMan,
                                     remark = b.remark
                                 });

                    return Content(query.ToJson());
                }
                else
                {
                    var query = (from b in bills
                                 join a in applys
                                 on b.fromID equals a.ID
                                 join p in prices
                                 on a.priNO equals p.priNO
                                 select new
                                 {
                                     billNo = b.billNo,
                                     prdName = a.prdName,
                                     spc = p.spc,
                                     num = Math.Abs(b.num ?? 0),
                                     unit = a.appUnit,
                                     viceNum = Math.Abs(b.viceNum ?? 0),
                                     viceUnit = a.viceUnit,
                                     appDep = a.appDep,
                                     appMan = a.appMan,
                                     remark = b.remark
                                 });

                    return Content(query.ToJson());
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetConvertForm(int keyValue)
        {
            BillEntity billEntity = billApp.GetForm(keyValue);
            ApplyBillEntity applyBillEntity = applyBillApp.GetForm(billEntity.fromID);
            PriceEntity priceEntity = priceApp.GetFormJson(applyBillEntity.priNO);
            var inAddNum = applyBillEntity.inAddNum == null ? 0 : applyBillEntity.inAddNum;
            var data = new
            {
                ID = applyBillEntity.ID,
                purNo = applyBillEntity.purNo,
                priNO = applyBillEntity.priNO,
                prdName = applyBillEntity.prdName,
                spc = applyBillEntity.spc,
                mat = applyBillEntity.mat,
                hopeDD = applyBillEntity.hopeDD,
                purNum = applyBillEntity.appNum,
                sup = applyBillEntity.purSup,
                purMan = applyBillEntity.purMan,
                appUnit = applyBillEntity.appUnit,
                num = (billEntity.num??0),
                viceUnit = applyBillEntity.viceUnit,
                convertNum = (billEntity.num ?? 0) - Math.Abs((billEntity.outNum ?? 0)) - (billEntity.inNum ?? 0),
                inNum = (billEntity.num ?? 0) - Math.Abs((billEntity.outNum ?? 0)) - (billEntity.inNum ?? 0),
                viceNum = billEntity.viceNum,
                priceUnit = priceEntity.unit,
                prdNo = applyBillEntity.prdNo
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetReturnForm(int keyValue)
        {
            BillEntity billEntity = billApp.GetForm(keyValue);
            ApplyBillEntity applyBillEntity = applyBillApp.GetForm(billEntity.fromID);
            PriceEntity priceEntity = priceApp.GetFormJson(applyBillEntity.priNO);
            var inAddNum = applyBillEntity.inAddNum == null ? 0 : applyBillEntity.inAddNum;
            var data = new
            {
                ID = billEntity.id,
                fromID = applyBillEntity.ID,
                billNo = billEntity.billNo,
                priNO = applyBillEntity.priNO,
                prdName = applyBillEntity.prdName,
                spc = applyBillEntity.spc,
                mat = applyBillEntity.mat,
                hopeDD = applyBillEntity.hopeDD.ToDateString(),
                purNum = applyBillEntity.appNum,
                sup = applyBillEntity.purSup,
                purMan = applyBillEntity.purMan,
                appUnit = applyBillEntity.appUnit,
                num = billEntity.num,
                viceUnit = applyBillEntity.viceUnit,
                viceNum = billEntity.viceNum,
                priceUnit = priceEntity.unit,
                prdNo = applyBillEntity.prdNo,
                retNum=(billEntity.num??0)-Math.Abs((billEntity.outNum ?? 0)) - (billEntity.inNum ?? 0)
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(int keyValue)
        {
            ApplyBillEntity applyBillEntity = applyBillApp.GetForm(keyValue);
            PriceEntity priceEntity = priceApp.GetFormJson(applyBillEntity.priNO);
            var data = new
            {
                ID = applyBillEntity.ID,
                purNo = applyBillEntity.purNo,
                priNO = applyBillEntity.priNO,
                prdName = applyBillEntity.prdName,
                spc = applyBillEntity.spc,
                mat = applyBillEntity.mat,
                hopeDD = applyBillEntity.hopeDD.Value.Date,
                appNum = (applyBillEntity.appNum ?? 0) - (applyBillEntity.inAddNum ?? 0),
                purNum = applyBillEntity.appNum,
                sup = applyBillEntity.purSup,
                purMan = applyBillEntity.purMan,
                appUnit = applyBillEntity.appUnit,
                num = (applyBillEntity.appNum??0) - (applyBillEntity.inAddNum??0),
                viceUnit = applyBillEntity.viceUnit,
                viceNum = (applyBillEntity.viceNum??0),
                priceUnit = priceEntity.unit,
                prdNo = applyBillEntity.prdNo
            };
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult RegistrationBill(string keyValue)
        {
            try
            {
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                var Ids = keyValue.Split(',');
                int[] keys= Array.ConvertAll<string, int>(Ids, s => int.Parse(s));
                foreach (var key in keys)
                {
                    var bill = billApp.GetForm(key);
                    if(bill.prtTag != "D")
                    {
                        bill.prtTag = "D";
                        bill.FINBILLDATE = userName + DateTime.Now.ToString();
                        billApp.SubmitForm(bill, key);
                    }
                }
                return Success("操作成功。");
            }
            catch (Exception ex)
            {

                return Error("操作失败。");
            }
        }

        [HttpGet]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        public ActionResult ExportPickExcel(string keyValue, string keyword, DateTime? BeginDate, DateTime? EndDate, int? purWay, string purIsTem)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {
                    var departId = OperatorProvider.Provider.GetCurrent().DepartmentId;
                    var departName = new OrganizeApp().GetForm(departId).F_FullName;
                    string sql = string.Format(@"SELECT ROW_NUMBER() OVER (ORDER BY b.id ASC) AS '序号',
                                                CONVERT(varchar(100), b.chkDate, 23) '领料日期',
                                                b.billNo '领料单号',p.prdName '物料名称',
                                                p.spc '规格',a.mat '材料要求',b.num '数量',
                                                a.appUnit '申购单位',b.viceNum '副数量',
                                                a.viceNum '副单位',p.price '单价',p.unit '报价单位',
                                                (CASE
                                                WHEN a.appUnit=p.unit THEN Convert(decimal(18,2),b.num*p.price)
                                                ELSE Convert(decimal(18,2),b.viceNum*p.price)
                                                END) '金额',
                                                a.appDep '领料部门',a.appMan '申购人',
                                                a.useGroup '使用组别',
                                                b.remark '备注'
                                                FROM Sys_Bill b join Sys_ApplyBill a
                                                on b.fromID = a.ID JOIN Sys_Price p
                                                on a.priNO = p.priNO WHERE b.billType = 'D'  AND a.takeNum IS NOT NULL AND a.appDep='{0}'",departName);
                    StringBuilder sb = new StringBuilder(sql);
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var ap = string.Format(" AND ( p.prdName LIKE '%{0}%' OR a.appDep LIKE '%{0}%' OR a.appMan LIKE '%{0}%')", keyword);
                        sb.Append(ap);
                    }
                    if (BeginDate != null && EndDate != null)
                    {
                        var ap = string.Format(" AND b.chkDate >= '{0}' AND b.chkDate <= '{1}'", BeginDate.Value.Date, EndDate.Value.Date.AddDays(1));
                        sb.Append(ap);
                    }
                    command.CommandText = sb.ToString();
                    using (SqlDataAdapter adp = new SqlDataAdapter(command))
                    {
                        adp.Fill(dt);
                    }
                }
            }
            string path = HttpContext.Server.MapPath("/Excel/");

            string fileName = "";

            if (!string.IsNullOrEmpty(keyValue))
            {
                fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + keyValue;
            }
            else
            {
                fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            }


            //设置新建文件路径及名称
            string savePath = path + fileName;

            new NPOIExcel().Export(dt, fileName, savePath);

            return Content(savePath);
        }


        [HttpPost]
        [HandlerAjaxOnly]
      //  [HandlerAuthorize]
        public ActionResult SubmitConvertForm(BillEntity billEntity,string prdNo,int keyValue,string title)
        {
            
            try
            {
                ApplyBillEntity applyBillEntity = applyBillApp.GetForm(billEntity.fromID);
                PriceEntity priceEntity = priceApp.GetFormJson(applyBillEntity.priNO);
                BillEntity bill = billApp.GetForm(keyValue);
                PRDTEntity pRDTEntity = null;
                if (billEntity.remark == "&nbsp;")
                {
                    billEntity.remark = null;
                }
                if (title == "erp")
                {

                    pRDTEntity = billApp.CheckPrdNoAndUnit(prdNo, applyBillEntity.appUnit, applyBillEntity.WAREWAY.Value);
                    if (pRDTEntity == null)
                    {
                        return Error("该物料编码不存在或者单位不匹配。");
                    }
                    if(string.IsNullOrEmpty(applyBillEntity.prdNo))
                    {
                        applyBillEntity.prdNo = prdNo;
                    }
                    if (string.IsNullOrEmpty(pRDTEntity.WH))
                    {
                        pRDTEntity.WH = "0000";
                    }
                }
                var price = priceApp.GetFormJson(applyBillEntity.priNO);
                billEntity.billNo = billApp.ProduceBillNO(billEntity.purSup, title, price.currency.Value);
                switch (title)
                {
                    case "in":
                        billEntity.billType = "C";
                        applyBillEntity.retDate = DateTime.Now;
                        billEntity.num = billEntity.inNum;
                        billEntity.inNum = null;
                        bill.inNum = (bill.inNum ?? 0) + billEntity.num;
                        bill.viceNum = (bill.viceNum ?? 0) - (billEntity.viceNum ?? 0);
                        if (applyBillEntity.yiJiaoNum == null)
                            applyBillEntity.yiJiaoNum = billEntity.num;
                        else
                            applyBillEntity.yiJiaoNum += billEntity.num;
                        break;
                    case "out":
                        billEntity.billType = "B";
                        billEntity.num = -(billEntity.inNum);
                        billEntity.inNum = null;
                        bill.outNum = (bill.outNum ?? 0) + billEntity.num;
                        bill.viceNum = (bill.viceNum ?? 0) - (billEntity.viceNum ?? 0);
                        applyBillEntity.inAddNum = (applyBillEntity.inAddNum ?? 0) + (billEntity.num ?? 0);
                        break;
                    case "erp":
                        string PS_ID = "PC";
                        billEntity.billType = "C";
                        applyBillEntity.retDate = DateTime.Now;
                        billEntity.num = billEntity.inNum;
                        billEntity.inNum = null;
                        bill.inNum = (bill.inNum ?? 0) + billEntity.num;
                        bill.viceNum = (bill.viceNum ?? 0) - (billEntity.viceNum ?? 0);
                        if (applyBillEntity.yiJiaoNum == null)
                            applyBillEntity.yiJiaoNum = billEntity.num;
                        else
                            applyBillEntity.yiJiaoNum += billEntity.num;
                        applyBillEntity.takeNum = (applyBillEntity.takeNum ?? 0) + billEntity.num;
                        var mf = MF_PSSApp.GetForm(billEntity.billNo, applyBillEntity.WAREWAY.Value);
                        if (mf == null)
                        {
                            MF_PSSApp.SubmitForm(PS_ID, billEntity.billNo, applyBillEntity.purSup, applyBillEntity.WAREWAY.Value);
                        }
                        var TF_PSSEntity = TF_PSSApp.SubmitForm(PS_ID, billEntity.billNo, pRDTEntity.WH, bill, pRDTEntity.NAME,applyBillEntity, billEntity, priceEntity);
                        billEntity.erpItm = TF_PSSEntity.PRE_ITM;
                        PRDT1Entity pRDT1Entity = billApp.CheckPRDT1Num(prdNo, pRDTEntity.WH, applyBillEntity.WAREWAY.Value);
                        if (pRDT1Entity != null)
                        {
                            pRDT1Entity.QTY = (pRDT1Entity.QTY ?? 0) + (billEntity.num ?? 0);
                            pRDT1Entity.LST_IND = DateTime.Now.Date;
                            pRDT1Entity.AMT_CST = (pRDT1Entity.AMT_CST ?? 0) + (TF_PSSEntity.AMTN_NET ?? 0);
                            pRDT1App.Update(pRDT1Entity, applyBillEntity.WAREWAY.Value);
                        }
                        else
                        {
                            pRDT1Entity = new PRDT1Entity();
                            pRDT1Entity.WH = pRDTEntity.WH;
                            pRDT1Entity.PRD_NO = pRDTEntity.PRD_NO;
                            pRDT1Entity.QTY = billEntity.num ?? 0;
                            pRDT1Entity.LST_IND = DateTime.Now.Date;
                            pRDT1Entity.PRD_MARK = "";
                            pRDT1Entity.AMT_CST = (TF_PSSEntity.AMTN_NET ?? 0);
                            pRDT1App.Insert(pRDT1Entity, applyBillEntity.WAREWAY.Value);
                        }
                        if (applyBillEntity.yiJiaoNum == applyBillEntity.appNum)
                        {
                            applyBillEntity.caseTag = "T";
                        }
                        break;
                }
                if (bill.num == (Math.Abs(bill.outNum ?? 0) + (bill.inNum ?? 0)))
                {
                    bill.caseTag = "T";
                }
                billEntity.chkDate = DateTime.Now;
                billApp.SubmitForm(billEntity);
                billApp.SubmitForm(bill, keyValue);
                applyBillApp.SubmitForm(applyBillEntity, applyBillEntity.ID);
                if (title != "out")
                {
                    var appExaMan = userApp.GetFormByName(applyBillEntity.appMan);
                    if (!string.IsNullOrEmpty(appExaMan.F_Email))
                    {
                        mHelper.MailServer = "10.110.120.2";
                        if (title == "in")
                        {
                            mHelper.Send(appExaMan.F_Email, "进货", "你好,您申购的" + applyBillEntity.prdName + "已进货，请点击链接<a>http://10.110.120.6:8090/</a>");
                        }
                        else { mHelper.Send(appExaMan.F_Email, "入仓", "你好,您申购的" + applyBillEntity.prdName + "已入仓，请点击链接<a>http://10.110.120.6:8090/</a>"); }

                    }
                }
                return Success("操作成功。");
            }
            catch (Exception ex)
            {
                new ErrorLogApp().SubmitForm(ex);
                return Error("操作失败。");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
       // [HandlerAuthorize]
        public ActionResult SubmitReturnForm(BillEntity billEntity,int keyValue,string prdNo,string title)
        {
            
            try
            {
                BillEntity bill = billApp.GetForm(keyValue);
                ApplyBillEntity applyBillEntity = applyBillApp.GetForm(bill.fromID);
                PriceEntity priceEntity = priceApp.GetFormJson(applyBillEntity.priNO);
                PRDTEntity pRDTEntity = null;
                TF_PSSEntity tF_PSS = null;
                var price = priceApp.GetFormJson(applyBillEntity.priNO);
                billEntity.billNo = billApp.ProduceBillNO(billEntity.purSup, title, price.currency.Value);
                if (billEntity.remark == "&nbsp;")
                {
                    billEntity.remark = null;
                }
                if (title.Contains("erp"))
                {
                    title = "erp";
                    pRDTEntity = billApp.CheckPrdNoAndUnit(prdNo, applyBillEntity.appUnit, applyBillEntity.WAREWAY.Value);
                    if (pRDTEntity == null)
                    {
                        return Error("该物料编码不存在。");
                    }
                    tF_PSS = billApp.CheckBillNoAndErpItm(bill.billNo, bill.erpItm.Value, applyBillEntity.WAREWAY.Value);
                    if (tF_PSS == null)
                    {
                        return Error("该进货单号不存在。");
                    }
                    PRDT1Entity pRDT1Entity = billApp.CheckPRDT1(prdNo, pRDTEntity.WH, billEntity.inNum.Value, applyBillEntity.WAREWAY.Value);
                    if (pRDT1Entity == null)
                    {
                        return Error("库存量不足。");
                    }
                }
                switch (title)
                {
                    case "return":
                        billEntity.billType = "E";
                        billEntity.num = -(billEntity.inNum);
                        billEntity.inNum = null;
                        bill.outNum = (bill.outNum ?? 0) + billEntity.num;
                        applyBillEntity.inAddNum = (applyBillEntity.inAddNum ?? 0) + (billEntity.num ?? 0);
                        applyBillEntity.yiJiaoNum = (applyBillEntity.yiJiaoNum ?? 0) + (billEntity.num ?? 0);
                        if (applyBillEntity.takeNum >= Math.Abs(billEntity.num.Value))
                        {
                            applyBillEntity.takeNum = (applyBillEntity.takeNum ?? 0) + (billEntity.num ?? 0);
                        }
                        break;
                    case "erp":
                        string PS_ID = "PB";
                        billEntity.billType = "E";
                        billEntity.num = -(billEntity.inNum);
                        billEntity.inNum = null;
                        bill.outNum = (bill.outNum ?? 0) + billEntity.num;
                        applyBillEntity.inAddNum = (applyBillEntity.inAddNum ?? 0) + (billEntity.num ?? 0);
                        applyBillEntity.yiJiaoNum = (applyBillEntity.yiJiaoNum ?? 0) + (billEntity.num ?? 0);
                        if (applyBillEntity.takeNum >= Math.Abs(billEntity.num.Value))
                        {
                            applyBillEntity.takeNum = (applyBillEntity.takeNum ?? 0) + (billEntity.num ?? 0);
                        }
                        var mf = MF_PSSApp.GetForm(billEntity.billNo, applyBillEntity.WAREWAY.Value);
                        if (mf == null)
                        {
                            MF_PSSApp.SubmitForm(PS_ID, billEntity.billNo, applyBillEntity.purSup, applyBillEntity.WAREWAY.Value, bill);
                        }
                        var TF_PSSEntity = TF_PSSApp.SubmitForm(PS_ID, billEntity.billNo, pRDTEntity.WH, bill, pRDTEntity.NAME, applyBillEntity, billEntity, priceEntity);
                        billEntity.erpItm = TF_PSSEntity.PRE_ITM;
                        PRDT1Entity pRDT1Entity = billApp.CheckPRDT1Num(prdNo, pRDTEntity.WH, applyBillEntity.WAREWAY.Value);
                        if (pRDT1Entity != null)
                        {
                            pRDT1Entity.QTY = (pRDT1Entity.QTY ?? 0) + (billEntity.num ?? 0);
                            pRDT1Entity.LST_OTD = DateTime.Now.Date;
                            pRDT1Entity.AMT_CST = (pRDT1Entity.AMT_CST ?? 0) + (TF_PSSEntity.AMTN_NET ?? 0);
                            pRDT1App.Update(pRDT1Entity, applyBillEntity.WAREWAY.Value);
                        }
                        else
                        {
                            pRDT1Entity = new PRDT1Entity();
                            pRDT1Entity.WH = pRDTEntity.WH;
                            pRDT1Entity.PRD_NO = pRDTEntity.PRD_NO;
                            pRDT1Entity.QTY = billEntity.num ?? 0;
                            pRDT1Entity.LST_OTD = DateTime.Now.Date;
                            pRDT1Entity.PRD_MARK = "";
                            pRDT1Entity.AMT_CST = (TF_PSSEntity.AMTN_NET ?? 0);
                            pRDT1App.Insert(pRDT1Entity, applyBillEntity.WAREWAY.Value);
                        }
                        tF_PSS.QTY_RTN = (tF_PSS.QTY_RTN ?? 0) + Math.Abs((billEntity.num ?? 0));
                        TF_PSSApp.Update(tF_PSS, applyBillEntity.WAREWAY.Value);
                        break;
                }
                if (bill.num == (Math.Abs(bill.outNum ?? 0)))
                {
                    bill.caseTag = "T";
                }
                if (applyBillEntity.caseTag == "T")
                {
                    applyBillEntity.caseTag = "F";
                }
                billEntity.fromID = bill.id;
                billEntity.chkDate = DateTime.Now;
                billApp.SubmitForm(billEntity);
                billApp.SubmitForm(bill, keyValue);
                applyBillApp.SubmitForm(applyBillEntity, applyBillEntity.ID);
                return Success("操作成功。");
            }
            catch (Exception ex)
            {

                new ErrorLogApp().SubmitForm(ex);
                return Error(ex.Message);
            }
        }
    }
}
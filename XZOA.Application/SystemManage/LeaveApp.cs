using XZOA.Domain.Entity.SystemManage;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Repository.SystemManage;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using XZOA.Code;
using XZOA.Domain.Enum;
using Microsoft.Office.Interop.Word;

namespace XZOA.Application.SystemManage
{
   public class LeaveApp
    {
      
        private ILeaveRepository service = new LeaveRepository();

        public List<LeaveEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        public List<LeaveEntity> GetFormByUserID(string userId)
        {
            return service.IQueryable(t=>t.F_UserId.Equals(userId)).ToList();
        }

        public int GetCheckCount()
        {
            var UserId = OperatorProvider.Provider.GetCurrent().UserId;
            var UserName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = service.IQueryable(t=>t.F_CheckLeaderId.Equals(UserId) && t.F_CheckUserName.Equals(UserName) && t.F_LeaveStatus == (int)LeaveStatusEnum.UnChecked);
            return query.Count();
        }

        public int GetApprovalCount()
        {
            var UserId = OperatorProvider.Provider.GetCurrent().UserId;
            var UserName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = service.IQueryable(t => t.F_ApprovalLeaderId == UserId &&t.F_ApprovalUserName.Equals(UserName) && t.F_LeaveStatus == (int)LeaveStatusEnum.UnApproved);
            return query.Count();
        }

        public LeaveEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }

        public void SubmitForm(LeaveEntity leaveEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                leaveEntity.Modify(keyValue);
                service.Update(leaveEntity);
            }
            else
            {
                leaveEntity.Create();
                service.Insert(leaveEntity);
            }
        }

        public void DeleteForm(string keyValue)
        {
            service.Delete(t => t.F_Id == keyValue);
        }

        public bool ConvertToWord(LeaveEntity leave, UserEntity user, UserEntity checkLeader, UserEntity approvalLeader, OrganizeEntity organ, RoleEntity role, string printName)
        {
            object missing = System.Reflection.Missing.Value;

            //创建一个Word应用程序实例
            Microsoft.Office.Interop.Word._Application oWord = new Microsoft.Office.Interop.Word.Application();

            oWord.ActivePrinter = printName;
            //设置为不可见
            oWord.Visible = false;

            //模板文件地址，这里假设在X盘根目录
            object oTemplate = "E:\\Temp\\template.doc";

            object filename = "E:\\Temp\\" + new Guid().ToString() + ".doc";

            //以模板为基础生成文档
            Microsoft.Office.Interop.Word._Document oDoc = oWord.Documents.Add(ref oTemplate, ref missing, ref missing, ref missing);

            oDoc.Activate(); //当前文档置前

            try
            {
                //声明书签数组
                object[] oBookMark = new object[12];
                //赋值书签名
                oBookMark[0] = "F_CreateTime";
                oBookMark[1] = "F_Account";
                oBookMark[2] = "F_Department";
                oBookMark[3] = "F_ApprovalLeader";
                oBookMark[4] = "F_CheckLeader";
                oBookMark[5] = "F_Group";
                oBookMark[6] = "F_Position";
                oBookMark[7] = "F_StartTime";
                oBookMark[8] = "F_LeaveReason";
                oBookMark[9] = "F_UserName";
                oBookMark[10] = "F_BeginEndTime";
                oBookMark[11] = "F_Name";
                //赋值任意数据到书签的位置
                oDoc.Bookmarks.get_Item(ref oBookMark[0]).Range.Text = leave.F_CreateTime.ToString();
                oDoc.Bookmarks.get_Item(ref oBookMark[1]).Range.Text = user.F_Account;
                oDoc.Bookmarks.get_Item(ref oBookMark[2]).Range.Text = organ.F_FullName;
                oDoc.Bookmarks.get_Item(ref oBookMark[3]).Range.Text = approvalLeader.F_RealName;
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = checkLeader.F_RealName;
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = "";
                oDoc.Bookmarks.get_Item(ref oBookMark[6]).Range.Text = role.F_FullName;
                oDoc.Bookmarks.get_Item(ref oBookMark[7]).Range.Text = user.F_CreatorTime.ToString();
                oDoc.Bookmarks.get_Item(ref oBookMark[8]).Range.Text = leave.F_LeaveReason;
                oDoc.Bookmarks.get_Item(ref oBookMark[9]).Range.Text = user.F_RealName;
                oDoc.Bookmarks.get_Item(ref oBookMark[10]).Range.Text = leave.F_BeginTime.ToString() + "至" + leave.F_EndTime.ToString();
                oDoc.Bookmarks.get_Item(ref oBookMark[11]).Range.Text = user.F_RealName;

                //oDoc.SaveAs(ref filename, ref missing, ref missing, ref missing,
                //ref missing, ref missing, ref missing, ref missing, ref missing,
                //ref missing, ref missing, ref missing, ref missing, ref missing,
                //ref missing, ref missing);

                //  return filename.ToString();
                //打印
                oDoc.PrintOut(ref missing, ref missing, ref missing, ref missing,

                   ref missing, ref missing, ref missing, ref missing,

                   ref missing, ref missing, ref missing, ref missing,

                   ref missing, ref missing, ref missing, ref missing,

                   ref missing, ref missing);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                object saveChange = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
                if (oDoc != null)
                    oDoc.Close(ref saveChange, ref missing, ref missing);
                if (oDoc != null)
                    oWord.Quit(ref missing, ref missing, ref missing);
            }
        }


    }
}

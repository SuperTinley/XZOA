/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XZOA.Application.SystemManage
{
    public class UserApp
    {
        private IUserRepository service = new UserRepository();
        private UserLogOnApp userLogOnApp = new UserLogOnApp();
        private LeaveApp leaveApp = new LeaveApp();
        private IAppointManRepository appService = new AppointManRepository();
        public List<UserEntity> GetList(Pagination pagination, string keyword, string DepartmentId, List<string> list=null)
        {
            var expression = ExtLinq.True<UserEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_Account.Contains(keyword));
                expression = expression.Or(t => t.F_RealName.Contains(keyword));
                expression = expression.Or(t => t.F_MobilePhone.Contains(keyword));
                if(list!=null)
                {
                    expression = expression.Or(t => list.Contains(t.F_DepartmentId));
                }
            }
            if (!string.IsNullOrEmpty(DepartmentId))
            {
                expression = expression.And(t => t.F_DepartmentId == DepartmentId);
            }
            expression = expression.And(t => t.F_Account != "admin");
            var query = service.IQueryable(expression);
            pagination.records = query.Count();
            return query.ToList();
        }

        public List<UserEntity> GetUserList()
        {
            return service.IQueryable().OrderBy(t => t.F_Id).ToList();
        }

        public List<UserEntity> GetUserList(List<string> roleIds)
        {
            return service.IQueryable(t=>roleIds.Contains(t.F_RoleId)).OrderBy(t => t.F_Id).ToList();
        }

        public List<UserEntity> GetUserCheckList(List<string> departIds,List<string> checkRoleIds, List<string> approvalRoleIds=null)
        {
            var expression = ExtLinq.True<UserEntity>();
            expression = expression.And(t => departIds.Contains(t.F_DepartmentId) && checkRoleIds.Contains(t.F_RoleId)); 
            if(approvalRoleIds!=null)
            {
                expression = expression.Or(t => approvalRoleIds.Contains(t.F_RoleId));
            }
            return service.IQueryable(expression).ToList();
        }

        public UserEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }
        
        public UserEntity GetFormByName(string keyValue)
        {
            var expression = ExtLinq.True<UserEntity>();
            expression = expression.And(t => t.F_RealName.Equals(keyValue));
            return service.FindEntity(expression);
        }

        public void DeleteForm(string keyValue)
        {
            service.DeleteForm(keyValue);
        }
        public int SubmitForm(UserEntity userEntity, UserLogOnEntity userLogOnEntity, string keyValue)
        {
            var Code = 200;
            var expression = ExtLinq.True<UserEntity>();
            expression = expression.And(t => t.F_Account.Equals(userEntity.F_Account));
            var UserEntity=service.FindEntity(expression);
            if (!string.IsNullOrEmpty(keyValue)&&UserEntity!=null)
            {
                UserEntity.F_Account = userEntity.F_Account;
                UserEntity.F_DepartmentId = userEntity.F_DepartmentId;
                UserEntity.F_Birthday = userEntity.F_Birthday;
                UserEntity.F_Email = userEntity.F_Email;
                UserEntity.F_RoleId = userEntity.F_RoleId;
                UserEntity.F_WeChat = userEntity.F_WeChat;
                UserEntity.F_DutyId = userEntity.F_DutyId;
                UserEntity.F_RealName=userEntity.F_RealName;
                UserEntity.F_Gender = userEntity.F_Gender;
                UserEntity.F_ManagerId = userEntity.F_ManagerId;
                UserEntity.F_MobilePhone = userEntity.F_MobilePhone;
                UserEntity.F_IsAdministrator = userEntity.F_IsAdministrator;
                UserEntity.F_Description = userEntity.F_Description;
                UserEntity.F_EnabledMark = userEntity.F_EnabledMark;
                var leave = leaveApp.GetFormByUserID(UserEntity.F_Id);
                var depart = new OrganizeApp().GetForm(UserEntity.F_DepartmentId);
                foreach (var item in leave)
                {
                    item.F_Department = depart.F_FullName;
                    leaveApp.SubmitForm(item,item.F_Id);
                }
                service.SubmitForm(UserEntity, userLogOnEntity, keyValue);
            }
            else {
                if (UserEntity != null)
                {
                    return Code = 100;
                }
                else
                {
                    userEntity.Create();
                }

                service.SubmitForm(userEntity, userLogOnEntity, keyValue);
            }
            
            
            return Code;
        }

        public bool EqualOldPwd(string OldPwd,string userId)
        {
            var rest = true;
            var UserLogOnEntity = userLogOnApp.GetForm(userId);
            if(UserLogOnEntity != null)
            {
                if(UserLogOnEntity.F_UserPassword.Equals(OldPwd))
                {
                    return rest;
                }
            }
            return rest = false;
        }
        

        public bool ChangePwd(string newPwd, string userId)
        {
            try
            {
                var UserLogOnEntity = userLogOnApp.GetForm(userId);
                UserLogOnEntity.F_UserPassword = newPwd;
                userLogOnApp.UpdateForm(UserLogOnEntity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public void UpdateForm(UserEntity userEntity)
        {
            service.Update(userEntity);
        }
        public UserEntity CheckLogin(string username, string password)
        {
            UserEntity userEntity = service.FindEntity(t => t.F_Account == username);
            if (userEntity != null)
            {
                if (userEntity.F_EnabledMark == true)
                {
                    UserLogOnEntity userLogOnEntity = userLogOnApp.GetForm(userEntity.F_Id);
                    if (password == userLogOnEntity.F_UserPassword)
                    {
                        DateTime lastVisitTime = DateTime.Now;
                        int LogOnCount = (userLogOnEntity.F_LogOnCount).ToInt() + 1;
                        if (userLogOnEntity.F_LastVisitTime != null)
                        {
                            userLogOnEntity.F_PreviousVisitTime = userLogOnEntity.F_LastVisitTime.ToDate();
                        }
                        userLogOnEntity.F_LastVisitTime = lastVisitTime;
                        userLogOnEntity.F_LogOnCount = LogOnCount;
                        userLogOnApp.UpdateForm(userLogOnEntity);
                        return userEntity;
                    }
                    else
                    {
                        throw new Exception("密码不正确，请重新输入");
                    }
                }
                else
                {
                    throw new Exception("账户被系统锁定,请联系管理员");
                }
            }
            else
            {
                throw new Exception("账户不存在，请重新输入");
            }
        }
    }
}

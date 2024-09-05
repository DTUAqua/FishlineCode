using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Babelfisk.Entities.SprattusSecurity;
using System.ServiceModel;
using Babelfisk.Entities;
using System.Data;
using System.Runtime.Serialization;
using Anchor.Core;

namespace Babelfisk.Service
{
    public partial class BabelfiskService : ISecurity
    {
        public Entities.SprattusSecurity.Users LogonUser()
        {
            using (var ctx = new SprattusSecurityContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                return ctx.Users
                          .Include("Role")
                          .Include("Role.FishLineTasks")
                          .Where(x => x.UserName.ToLower() == ServiceSecurityContext.Current.PrimaryIdentity.Name.ToLower()).FirstOrDefault();
            }
        }


        public Users GetUser(string strUserName)
        {
            using (var ctx = new SprattusSecurityContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                Users usr = ctx.Users
                               .Include("Role")
                               .Include("Role.FishLineTasks")
                               .Where(x => x.UserName == strUserName).FirstOrDefault();
                
                return usr;
            }
        }


        public List<Users> GetUsers()
        {
            using (var ctx = new SprattusSecurityContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var lstUsers = ctx.Users
                                  .Include("Role")
                                  .Include("Role.FishLineTasks")
                                  .ToList();

                return lstUsers;
            }
        }


        public Users GetUserById(int intId)
        {
            using (var ctx = new SprattusSecurityContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                Users usr = ctx.Users
                               .Include("Role")
                               .Include("Role.FishLineTasks")
                               .Where(x => x.UserId == intId).FirstOrDefault();

                return usr;
            }
        }


        public List<Role> GetRoles()
        {
            using (var ctx = new SprattusSecurityContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                return ctx.Role
                          .Include("FishLineTasks")
                          .ToList();
            }
        }

        public List<FishLineTasks> GetTasks()
        {
            using (var ctx = new SprattusSecurityContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                return ctx.FishLineTasks.ToList();
            }
        }


        public bool CanDeleteUser(Users user)
        {
            bool blnCanDelete = true;

            using (var ctx = new SprattusSecurityContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                //if (SOme condition)
                //    blnCanDelete = false;
            }

            return blnCanDelete;
        }


        private DatabaseOperationResult ValidateInsertUser(SprattusSecurityContainer ctx, Users usr)
        {
            string strUserName = usr.UserName.ToLower();
            if (ctx.Users.Where(x => x.UserId != usr.UserId && x.UserName.ToLower().Equals(strUserName)).Count() > 0)
                return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "UserNameExists");

            return DatabaseOperationResult.CreateSuccessResult();
        }


        public Entities.DatabaseOperationResult UpdateUser(Users user)
        {
            bool blnActiveStateChanged = false;
            try
            {
                var state = user.ChangeTracker.State.ToString();

                using (var ctx = new SprattusSecurityContainer())
                {
                    Users usr = user;
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    DatabaseOperationResult datRes = ValidateInsertUser(ctx, usr);

                    if (datRes.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                        return datRes;

                    if (usr.ChangeTracker.State == ObjectState.Modified)
                    {
                        usr = ctx.Users.Include("Role").Where(x => x.UserId == user.UserId).FirstOrDefault();
                        ctx.ApplyOverwritingMethod(usr, OverwritingMethod.ClientWins);

                        if (usr == null)
                            throw new ApplicationException("Could not find user to update in database.");

                        usr.UserName = user.UserName;
                        usr.Password = user.Password;
                        usr.FirstName = user.FirstName;
                        usr.LastName = user.LastName;
                        usr.email = user.email;

                        if (usr.IsActive != user.IsActive)
                            blnActiveStateChanged = true;

                        usr.IsActive = user.IsActive;

                        if (usr.IsActive && usr.LoginAttempts >= UserNamePassValidator.MaxLoginAttempts)
                            usr.LoginAttempts = 0;
                       
                        //Remove old roles
                        foreach (var r in usr.Role.ToList())
                            if (!user.Role.Where(x => x.RoleId_PK == r.RoleId_PK).Any())
                                usr.Role.Remove(r);

                        //Add new roles
                        foreach(var r in user.Role.ToList())
                            if (!usr.Role.Where(x => x.RoleId_PK == r.RoleId_PK).Any())
                            {
                                var role = ctx.Role.Where(x => x.RoleId_PK == r.RoleId_PK).FirstOrDefault();

                                if (role != null)
                                    usr.Role.Add(role);
                            }

                        //Handle roles
                       // var vNewRole = ctx.Role.Where(x => x.RoleId_PK == user.Role.RoleId_PK).FirstOrDefault();

                      //  usr.Role = vNewRole;
                    }


                    if (user.ChangeTracker.State == ObjectState.Deleted)
                    {
                        usr = ctx.Users.Include("Role").Where(x => x.UserId == user.UserId).FirstOrDefault();
                        
                        if (usr == null)
                            throw new ApplicationException("User has already been deleted.");
                        
                        usr.MarkAsDeleted();
                        ctx.Users.DeleteObject(usr);
                    }

                    ctx.Users.ApplyChanges(usr);

                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                }

                try
                {
                    if (blnActiveStateChanged)
                    {
                        string strAddition = null;
                        if (ServiceSecurityContext.Current != null && ServiceSecurityContext.Current.PrimaryIdentity != null && ServiceSecurityContext.Current.PrimaryIdentity.Name != null)
                            strAddition = ServiceSecurityContext.Current.PrimaryIdentity.Name;
                        Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Info, string.Format("User with login '{0}' had its active state changed to: {1}. This was performed by user: {2}", user.UserName, user.IsActive, strAddition ?? "NULL"));
                    }
                }
                catch { }
                
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in UpdateUser with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in UpdateUser with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in UpdateUser with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }



        #region Role methods


        private Role ValidateRoleDuplicateKey(Role r, SprattusSecurityContainer ctx)
        {
            bool blnIsNew = r.ChangeTracker.State == ObjectState.Added;

            var v = from ro in ctx.Role
                    where ro.Role1.Equals(r.Role1, StringComparison.InvariantCultureIgnoreCase) &&
                         (ro.RoleId_PK != r.RoleId_PK || blnIsNew)
                    select ro;

            return v.FirstOrDefault();
        }


        public DatabaseOperationResult UpdateRole(ref Role r)
        {
            if (r.ChangeTracker.State == ObjectState.Added)
                return SaveNewRole(ref r);

            try
            {
                using (SprattusSecurityContainer ctx = new SprattusSecurityContainer())
                {
                    ctx.Connection.Open();

                    int intRoleId = r.RoleId_PK;
                    var rEdit = ctx.Role.Include("FishLineTasks").Where(x => x.RoleId_PK == intRoleId).FirstOrDefault();

                    if (r.ChangeTracker.State == ObjectState.Deleted)
                        ctx.DeleteObject(rEdit);
                    else
                    {
                        if (ValidateRoleDuplicateKey(r, ctx) != null)
                            return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

                        if (rEdit == null)
                            return new Entities.DatabaseOperationResult(Entities.DatabaseOperationStatus.UnexpectedException, "Could not find role to edit.");

                        CopyRoleToRoleEntity(ctx, r, ref rEdit);

                        ctx.Role.ApplyChanges(rEdit);
                    }

                    ctx.SaveChangesAndHandleOptimisticConcurrency(Entities.OverwritingMethod.ClientWins);

                    rEdit.AcceptChanges();
                    r = rEdit;
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in UpdateRole with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in UpdateRole with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in UpdateRole with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        private void CopyRoleToRoleEntity(SprattusSecurityContainer ctx, Role rFrom, ref Role rTo)
        {
            rFrom.CopyEntityValueTypesTo(rTo);

            //Remove any old tasks not in rFrom
            if (rTo.FishLineTasks.Count > 0)
            {
                var lstTasks = rFrom.FishLineTasks.Select(x => x.FishLineTaskId).ToList();
                var lstRemove = rTo.FishLineTasks.Where(x => !lstTasks.Contains(x.FishLineTaskId)).ToList();

                foreach (var t in lstRemove)
                    rTo.FishLineTasks.Remove(t);
            }

            //Add tasks from rFrom not i rTo
            if (rFrom.FishLineTasks.Count > 0)
            {
                var lstTasks = rTo.FishLineTasks.Select(x => x.FishLineTaskId).ToList();
                var lstAddIds = rFrom.FishLineTasks.Where(x => !lstTasks.Contains(x.FishLineTaskId)).Select(x => x.FishLineTaskId).ToList();

                var lstAdd = ctx.FishLineTasks.Where(t => lstAddIds.Contains(t.FishLineTaskId)).ToList();
                rTo.FishLineTasks.AddRange(lstAdd);
            }
        }



        private DatabaseOperationResult SaveNewRole(ref Role r)
        {
            try
            {
                using (SprattusSecurityContainer ctx = new SprattusSecurityContainer())
                {
                    ctx.Connection.Open();

                    if (ValidateRoleDuplicateKey(r, ctx) != null)
                        return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

                    Role rNew = new Role();
                    CopyRoleToRoleEntity(ctx, r, ref rNew);

                    int intId = 0;
                    if(ctx.Role.Any())
                        intId = ctx.Role.Max(x => x.RoleId_PK) + 1;

                    rNew.RoleId_PK = intId;

                    ctx.Role.ApplyChanges(rNew);

                    ctx.SaveChangesAndHandleOptimisticConcurrency(Entities.OverwritingMethod.ClientWins);

                    rNew.AcceptChanges();
                    r = rNew;
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveNewRole with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveNewRole with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveNewRole with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        #endregion

    }
}
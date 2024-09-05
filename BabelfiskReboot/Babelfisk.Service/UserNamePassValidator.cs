using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Babelfisk.Entities.SprattusSecurity;

namespace Babelfisk.Service
{
    public class UserNamePassValidator : UserNamePasswordValidator
    {
        public static int MaxLoginAttempts = 5;

        public override void Validate(string userName, string token)
        {

            Users usr = null;
            string pwdLog = null;
            try
            {
                if (token != null)
                {
                    string strPwdHash = token;
                    string strPwd = token;

                    string[] str = token.Split(new string[] {"¤¤-¤¤"}, StringSplitOptions.None);

                    if (str.Length == 2)
                    {
                        strPwdHash = str[0];
                        strPwd = str[1];
                    }

                    pwdLog = strPwdHash;

                    using (var ctx = new SprattusSecurityContainer())
                    {
                        ctx.Connection.Open();
                        ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                        usr = (from u in ctx.Users
                               where u.UserName.ToLower() == userName.ToLower() && u.Password == strPwdHash
                               select u).SingleOrDefault();
                    }

                    //If usr is NULL, try with old password and if that works, save the hash instead.
                    if (usr == null)
                    {
                        using (var ctx = new SprattusSecurityContainer())
                        {
                            ctx.Connection.Open();
                            ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                            usr = (from u in ctx.Users
                                   where u.UserName.ToLower() == userName.ToLower() && u.Password == strPwd
                                   select u).SingleOrDefault();
                           
                            //Save hash
                            if (usr != null)
                            {
                                usr.Password = strPwdHash;
                                ctx.Users.ApplyChanges(usr);
                                ctx.SaveChangesAndHandleOptimisticConcurrency(Entities.OverwritingMethod.ClientWins);
                                usr.AcceptChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ThrowAndLogException(new FaultException("Failed to retrieve user from database.", new FaultCode(ex.Message + (ex.InnerException != null ? (". " + ex.InnerException.Message) : ""))), string.Format("userName: {0}, pwdhash: {1}", (userName ?? "NULL"), pwdLog ?? "NULL"), ex);
            }

            //If no user was found with current username and password, handle a failed login.
            if (usr == null)
            {
                //If user is found, but wrong password, increment login attempts.
                usr = HandleUserLoginFail(userName);

                //If user failed to login (username or password wrong), throw exception
                //If username was found, but password was wrong and the user is still active, throw exception.
                if (usr == null || usr.IsActive)
                    ThrowAndLogException(new FaultException("InvalidUsernameOrPassword", new FaultCode("InvalidUsernameOrPassword;" + (usr != null ? ("LoginAttemptsLeft:" + (MaxLoginAttempts - usr.LoginAttempts)) : ""))), string.Format("userName: {0}, pwdhash: {1}", (userName ?? "NULL"), pwdLog ?? "NULL"));
            }

            //If user is not active, throw deactivated exception.
            if (!usr.IsActive)
                ThrowAndLogException(new FaultException("Deactivated", new FaultCode("Deactivated")), string.Format("userName: {0}, pwdhash: {1}", (userName ?? "NULL"), pwdLog ?? "NULL"));

            //If user logged in successfully, reset attempts counter.
            if (usr.LoginAttempts > 0)
                ResetLoginAttempts(usr.UserName);
        }


        private static void ThrowAndLogException(Exception e, string extraMessage = null, Exception inner = null)
        {
            try
            {
                string strMessage = "";
                if (extraMessage != null)
                    strMessage += extraMessage;
                if (e is FaultException && (e as FaultException).Code != null && (e as FaultException).Code.Name != null)
                    strMessage += string.Format(", FaultException code: {0}", (e as FaultException).Code.Name);
                if (inner != null)
                    strMessage += string.Format(", original exception message: {0}, original stacktrace: {1}", inner.Message ?? "NULL", inner.StackTrace ?? "NULL");

                Anchor.Core.Loggers.Logger.LogError(e, strMessage);
            }
            catch { }
            throw e;
        }



        public static void ResetLoginAttempts(string strUserName)
        {
            try
            {
                using (var ctx = new SprattusSecurityContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    //Selet only user by username
                    var user = (from u in ctx.Users
                                where u.UserName.ToLower() == strUserName.ToLower()
                                select u).SingleOrDefault();

                    //If user found, increment 
                    if (user != null)
                    {
                        user.LoginAttempts = 0;

                        ctx.Users.ApplyChanges(user);
                        ctx.SaveChangesAndHandleOptimisticConcurrency(Entities.OverwritingMethod.ClientWins);

                        user.AcceptChanges();
                    }
                }
            }
            catch(Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e, string.Format("ResetLoginAttempts() failed. strUserName = {0}.", strUserName ?? "NULL"));
            }
        }


        private Users HandleUserLoginFail(string userName)
        {
            Users usr = null;

            try
            {
                using (var ctx = new SprattusSecurityContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    //Selet only user by username
                    var user = (from u in ctx.Users
                                where u.UserName.ToLower() == userName.ToLower()
                                select u).SingleOrDefault();

                    //If user found, increment 
                    if (user != null)
                    {
                        usr = user;

                        if (usr.IsActive)
                        {
                            usr.LoginAttempts = Math.Max(usr.LoginAttempts + 1, 0);

                            if (usr.LoginAttempts >= MaxLoginAttempts)
                                usr.IsActive = false;

                            ctx.Users.ApplyChanges(usr);
                            ctx.SaveChangesAndHandleOptimisticConcurrency(Entities.OverwritingMethod.ClientWins);

                            usr.AcceptChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ThrowAndLogException(new FaultException("Failed to increment login attempts.", new FaultCode(ex.Message)), "username: " + (userName ?? "NULL"), ex);
            }


            return usr;
        }

    }
}
using SmartDots.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Babelfisk.Service;
using Anchor.Core.Encryption;

namespace SmartDots.Service
{
    public partial class AquaDotsService
    {
        private static byte[] _entropy = Guid.Parse("4444C928-5C76-4076-9C4A-906A288390C1").ToByteArray();

        private static string _tokenInvalidErrorMessage = "Security token is invalid, please log out and in again.";
        private static string _unknownErrorMessage = _unknownErrorMessage;


        /// <summary>
        /// Authenticate a user at login.
        /// </summary>
        public WebApiResult Authenticate(UserAuthentication usr)
        {
            try
            {
                string userName = (usr.Username ?? "").Trim();
                string password = usr.Password;
                string passwordHashed = Babelfisk.Entities.Hash.ComputeHash(password ?? "");

                Babelfisk.Entities.SprattusSecurity.Users user;
            

                using (var ctx = new Babelfisk.Entities.SprattusSecurity.SprattusSecurityContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    user = (from u in ctx.Users
                            where u.UserName.ToLower() == userName.ToLower() && (u.Password == passwordHashed || u.Password == password)
                            select u).SingleOrDefault();
                }

                if (user == null)
                    throw new ApplicationException("Unknown user or password.");

                Babelfisk.Entities.Sprattus.DFUPerson dfuUser;
                using (var ctx = new Babelfisk.Entities.Sprattus.SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    dfuUser = (from u in ctx.DFUPerson
                               where u.initials != null && u.initials.ToLower() == userName.ToLower()
                               select u).SingleOrDefault();
                }

                if(dfuUser == null)
                    throw new ApplicationException("No aqua person 'Personer (Aqua)' was found that mactches the login username. Please ensure an aqua person exist in the Fishline codes list with initials matching the login username and try again.");


                return new WebApiResult() { Result = new User() { Id = new Guid(user.UserId, 0, 0, new byte[8]), AccountName = user.UserName, Token = GetTokenFromUser(user, dfuUser) } };

            }
            catch (Exception e)
            {
                return new WebApiResult() { ErrorMessage = e.Message };
            }
        }


        public Babelfisk.Entities.SprattusSecurity.Users GetUserFromId(int id)
        {
            Babelfisk.Entities.SprattusSecurity.Users user;
            using (var ctx = new Babelfisk.Entities.SprattusSecurity.SprattusSecurityContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                user = (from u in ctx.Users
                                     .Include("Role")
                                     .Include("Role.FishLineTasks")
                        where u.UserId == id
                        select u).SingleOrDefault();
            }

            return user;
        }



        #region Token methods


        public string GetTokenFromUser(Babelfisk.Entities.SprattusSecurity.Users u, Babelfisk.Entities.Sprattus.DFUPerson dfuUser)
        {
            string s = string.Format("{0}|{1}|{2}|{3}|{4}", u.UserName, u.UserId.ToString(), dfuUser.Id, "AquaDotsToken", DateTime.UtcNow.ToString("ddMMyyyyHHmmssfff", System.Globalization.CultureInfo.InvariantCulture));

            var senc = SimpleStringEncryption.EncryptString(s, _entropy);

            var he = System.Web.HttpUtility.UrlEncode(senc);
            
            return he;
        }


        /// <summary>
        /// Returns whether the token is valid or not.
        /// </summary>
        public bool ValidateToken(string token)
        {
            try
            {
                string userName = null;
                int userId = -1, dfuPersonId = -1;

                return ValidateToken(token, ref userName, ref userId, ref dfuPersonId);
            }
            catch { }

            return false;
        }


        /// <summary>
        /// Validates a token and returns the encrypted username and userid part of it.
        /// </summary>
        public bool ValidateToken(string token, ref string userName, ref int userId, ref int dfuPersonId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return false;

                var ueToken = SimpleStringEncryption.DecryptString(token, _entropy);

                if (ueToken == null)
                    return false;

                var unencryptedToken = ueToken;

                int tmpUserId, tmpDfuUserId;
                DateTime dt;
                string[] arr = null;

                if (unencryptedToken == null || (arr = unencryptedToken.Split('|')) == null || arr.Length != 5 ||
                   !int.TryParse(arr[1], out tmpUserId) ||
                   !int.TryParse(arr[2], out tmpDfuUserId) ||
                   !"AquaDotsToken".Equals(arr[3], StringComparison.InvariantCultureIgnoreCase) ||
                   !DateTime.TryParseExact(arr[4], "ddMMyyyyHHmmssfff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt) ||
                   dt < DateTime.UtcNow.AddMonths(-6) //Make sure token is not more than a half year old
                   )
                    return false;

                userName = arr[0];
                userId = tmpUserId;
                dfuPersonId = tmpDfuUserId;

                return true;
            }
            catch { }

            return false;
        }


        #endregion


        /// <summary>
        /// Get User SmartDots settings.
        /// </summary>
        public WebApiResult GetSettings()
        {
            var req = HttpContext.Current.Request.Params["token"];

            int userId = -1, dfuPersonId = -1;
            string userName = null;
            if (!ValidateToken(req, ref userName, ref userId, ref dfuPersonId))
                return new WebApiResult() { ErrorMessage = _tokenInvalidErrorMessage };

            return new WebApiResult() { Result = new UserSettings() { CanApproveAnnotation = true, UseSampleStatus = true, CanAttachDetachSample = false, ScanFolder = false} };
        }



        /// <summary>
        /// Get readability qualities (lookup?)
        /// </summary>
        public WebApiResult GetReadabilityQualities()
        {
            try
            {
                var req = HttpContext.Current.Request.Params["token"];

                if (!ValidateToken(req))
                    return new WebApiResult() { ErrorMessage = _tokenInvalidErrorMessage };

                string[] arrColors = new string[] { "#00b300", "#0000b3", "#b30000", "#f30000" };

                List<Babelfisk.Entities.Sprattus.L_OtolithReadingRemark> lst = null;
                using (var ctx = new Babelfisk.Entities.Sprattus.SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    lst = ctx.L_OtolithReadingRemark.OrderBy(x => x.num).ToList();
                }

                List<ReadabilityQuality> lstQualities = new List<ReadabilityQuality>();
                if (lst != null && lst.Count > 0)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        var itm = lst[i];
                        var clr = arrColors[Math.Min(i, arrColors.Length - 1)];

                        lstQualities.Add(new ReadabilityQuality()
                        {
                            Id = IntToGuid(itm.L_OtolithReadingRemarkID),
                            Code = itm.otolithReadingRemark,
                            Description = itm.description,
                            Color = clr
                        });
                    }
                }

                /*List<ReadabilityQuality> lstQualities = new List<ReadabilityQuality>()
                {
                     new ReadabilityQuality()
                     {
                          Id = Guid.Parse("942ABE62-0152-4852-80F5-A161BD46147E"),
                          Code = "AQ1",
                          Description = "Rings can be counted with certainty",
                          Color = "#00b300"
                     },

                     new ReadabilityQuality()
                     {
                          Id = Guid.Parse("C32E0EC9-15C6-47C0-8DC5-EFA587BE82AF"),
                          Code = "AQ2",
                          Description = "Rings can be counted with difficulty and some doubt",
                          Color = "#0000b3"
                     },

                      new ReadabilityQuality()
                     {
                          Id = Guid.Parse("FA0651BE-0FE6-4E34-ABF2-2A6C9FE3F410"),
                          Code = "AQ3",
                          Description = "Rings cannot be counted, the calcified structure is considered unreadable (no age is assigned)",
                          Color = "#b30000"
                     },

                     new ReadabilityQuality()
                     {
                          Id = Guid.Parse("4D7FBD36-32C0-4872-9568-6C6DBDF4E384"),
                          Code = "AQ3_QA",
                          Description = "Unreadable or very difficult to age with acceptable precision",
                          Color = "#f30000"
                     }
                };*/

                return new WebApiResult() { Result = lstQualities };
            }
            catch (Exception e)
            {
                return new WebApiResult() { ErrorMessage = e.Message };
            }
        }


        #region Guid conversion methods


        protected static Guid IntToGuid(int g)
        {
            return new Guid(g, 0, 0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        }

        protected static int? GuidToInt(Guid g)
        {
            if (g == null)
                return null;

            var arr = g.ToByteArray();

            if (arr == null || arr.Length < 4)
                return null;

            return BitConverter.ToInt32(arr, 0);
        }


        #endregion

    }
}
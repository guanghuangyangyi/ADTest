//文件名称（File Name）                 StringConverTool.cs
//作者(Author)                          yjq
//日期（Create Date）                   2017.5.5
//修改记录(Revision History)
//    R1:
//        修改作者:
//        修改日期:
//        修改原因:
//    R2:
//        修改作者:
//        修改日期:
//        修改原因:
//**************************************************************
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
AD的全称是Active Directory
 域（Domain）:
       1)域是Windows网络中独立运行的单位，域之间相互访问则需要建立信任关系(即Trust Relation)。信任关系是连接在域与域之间的桥梁。当一个域与其他域建立了信任关系后
       2)两个域之间不但可以按需要相互进行管理，还可以跨网分配文件和打印机等设备资源，使不同的域之间实现网络资源的共享与管理，以及相互通信和数据传输
 域控制器（DC）：域控制器就是一台服务器，负责每一台联入网络的电脑和用户的验证工作。
 组织单元（OU）
 用户名服务器名（CN）

先按照域


*/
namespace ADTest
{
    /// <summary>
    /// 域控认证工具
    /// </summary>
    public class ADTool
    {

        /// <summary>
        /// 未完成
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsPass(string userAccount, string password)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(userAccount))
                {
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        string DomainName = "DC = SYQUEYRY,DC = COM,DC = CN";
                        string ADPath = "LDAP://SYQUEYRY.COM.CN";
                        string ADDomain = "SYQUEYRY";
                        //获得当前域中的路径
                        string _ADPath = ADPath + "/" + ADDomain;
                        string domainAndUsername;
                        bool hasDomain = false;
                        if (userAccount.StartsWith(DomainName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            hasDomain = true;
                        }
                        if (hasDomain)
                        {
                            domainAndUsername = userAccount;
                        }
                        else
                        {
                            domainAndUsername = DomainName + @"\" + userAccount;
                        }
                        DirectoryEntry entry = new DirectoryEntry(_ADPath, domainAndUsername, password);
                        DirectorySearcher search = new DirectorySearcher(entry);
                        if (hasDomain)
                        {
                            userAccount = userAccount.Substring(DomainName.Length + 1);
                        }
                        search.Filter = "(sAMAccountName=" + userAccount + ")";
                        search.PropertiesToLoad.Add("displayName");
                        SearchResult adUser = null;
                        try
                        {
                            adUser = search.FindOne();
                            if (adUser == null)

                            {
                                //_error = "域认证失败";
                            }
                            else
                            {
                                if (Convert.ToInt32(adUser.Properties["userAccountControl"][0]) == 2)
                                {
                                    //_myUser = new MyUser(userAccount, password, adUser.Properties["displayName"].ToString());
                                }
                                else
                                {
                                    //_error = "此用户已禁用";

                                }

                                adUser = null;

                            }

                        }

                        catch (Exception ex)

                        {
                            //_error = ex.Message;
                            adUser = null;
                        }
                        finally
                        {
                            entry.Close();
                            entry = null;
                            search.Dispose();
                            search = null;
                        }
                    }
                    else
                    {
                        throw new Exception("密码为空");
                    }
                }
                else
                {
                    throw new Exception("用户名为空");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        /*
        /// <summary>
        /// LDAP登录
        /// </summary>
        /// <param name="domain">域</param>
        /// <param name="lDAP">LDAP</param>
        /// <param name="userName">域用户名</param>
        /// <param name="password">域密码</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="persistCookie">是否保存密码</param>
        /// <param name="formsAuthentication">表单验证,是否需要重新定位</param>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        //public static BaseUserInfo LogOnByLDAP(string domain, string lDAP, string userName, string password, string permissionCode, bool persistCookie, bool formsAuthentication, out string statusCode, out string statusMessage)
        //{
        //    DirectoryEntry dirEntry = new DirectoryEntry();
        //    dirEntry.Path = lDAP;
        //    dirEntry.Username = domain + "\\" + userName;
        //    dirEntry.Password = password;
        //    dirEntry.AuthenticationType = AuthenticationTypes.Secure;
        //    try
        //    {
        //        DirectorySearcher dirSearcher = new DirectorySearcher(dirEntry);
        //        dirSearcher.Filter = String.Format("(&(objectClass=user)(samAccountName={0}))", userName);
        //        System.DirectoryServices.SearchResult result = dirSearcher.FindOne();
        //        if (result != null)
        //        {
        //            // 统一的登录服务
        //            DotNetService dotNetService = new DotNetService();
        //            BaseUserInfo userInfo = dotNetService.LogOnService.LogOnByUserName(Utilities.GetUserInfo(), userName, out statusCode, out statusMessage);
        //            // 检查身份
        //            if (statusCode.Equals(Status.OK.ToString()))
        //            {
        //                userInfo.IPAddress = GetIPAddressId();

        //                bool isAuthorized = true;
        //                // 用户是否有哪个相应的权限
        //                if (!string.IsNullOrEmpty(permissionCode))
        //                {
        //                    isAuthorized = dotNetService.PermissionService.IsAuthorized(userInfo, permissionCode, null);
        //                }
        //                // 有相应的权限才可以登录
        //                if (isAuthorized)
        //                {
        //                    if (persistCookie)
        //                    {
        //                        // 相对安全的方式保存登录状态
        //                        // SaveCookie(userName, password);
        //                        // 内部单点登录方式
        //                        SaveCookie(userInfo);
        //                    }
        //                    else
        //                    {
        //                        RemoveUserCookie();
        //                    }
        //                    LogOn(userInfo, formsAuthentication);
        //                }
        //                else
        //                {
        //                    statusCode = Status.LogOnDeny.ToString();
        //                    statusMessage = "访问被拒绝、您的账户没有后台管理访问权限。";
        //                }
        //            }

        //            return userInfo;
        //        }
        //        else
        //        {
        //            statusCode = Status.LogOnDeny.ToString();
        //            statusMessage = "应用系统用户不存在，请联系管理员。";
        //            return null;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //Logon failure: unknown user name or bad password.
        //        statusCode = Status.LogOnDeny.ToString();
        //        statusMessage = "域服务器返回信息" + e.Message.Replace("\r\n", "");
        //        return null;
        //    }
        //}
        */

        /// <summary>
        /// 功能：是否连接到域
        /// 作者：yjq
        /// 时间：2017-05-05
        /// </summary>
        /// <param name="domainName">域名或IP</param>
        /// <param name="userName">用户名</param>
        /// <param name="userPwd">密码</param>
        /// <param name="entry">域</param>
        /// <returns></returns>
        public static bool IsConnected(string domainName, string userName, string userPwd, out DirectoryEntry domain)
        {
            bool result = false;
            try
            {
                domain = new DirectoryEntry();
                domain.Path = string.Format("LDAP://{0}", domainName);
                domain.Username = userName;
                domain.Password = userPwd;
                domain.AuthenticationType = AuthenticationTypes.Secure;

                domain.RefreshCache();

                result= true;
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        /// <summary>
        /// 域中是否存在组织单位
        /// </summary>
        /// <param name="entry">域</param>
        /// <param name="name">组织单位名称</param>
        /// <param name="ou"></param>
        /// <returns></returns>
        public static bool IsExistOU(DirectoryEntry entry,string name, out DirectoryEntry ou)
        {
            ou = new DirectoryEntry();
            try
            {
                ou = entry.Children.Find("OU=" + name);

                return (ou != null);
            }
            catch (Exception ex)
            {
              throw ex;
            }
        }
    }

    /// <summary>
    /// 类型
    /// </summary>
    public enum TypeEnum:int
    {
        /// <summary>
        /// 组织单位
        /// </summary>
        OU=1,
        /// <summary>
        /// 用户
        /// </summary>
        USER=2,
    }
    public class AdModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string ParentId { get; set; }
        public AdModel(string id,string name,int typeId,string parentId)
        {
            Id = id;
            Name = name;
            TypeId = typeId;
            ParentId = parentId;
        }
    }

}
/*
AD 域账号登录 域服务数据读写，有俩种模式
1、轻量级的数据读取
获取连接PrincipalContext pc = new PrincipalContext(ContextType.Domain, _domain)
pc.ValidateCredentials(jobNumber, password)


2、DectoryEntry 
可以获取整个服务器的数据，也可以修改其中的信息


*/

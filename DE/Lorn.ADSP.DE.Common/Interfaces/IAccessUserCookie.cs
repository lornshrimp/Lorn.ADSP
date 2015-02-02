using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Interfaces
{
    /// <summary>
    /// 用户Cookie访问接口
    /// </summary>
    public interface IAccessUserCookie
    {
        /// <summary>
        /// 获取用户Cookie
        /// </summary>
        /// <param name="cookieId"></param>
        /// <returns></returns>
        UserMediaCookie GetUserCookie(Guid cookieId,Guid mediaId);
        /// <summary>
        /// 储存用户Cookie
        /// </summary>
        /// <param name="cookie"></param>
        void SetUserCookie(Guid mediaId,UserMediaCookie cookie);
    }
}

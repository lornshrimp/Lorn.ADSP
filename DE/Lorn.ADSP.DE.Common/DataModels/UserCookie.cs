using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Lorn.ADSP.Common.Interfaces;

namespace Lorn.ADSP.DE.DataModels
{
    public class UserCookie
    {
        public Guid CookieId { get; set; }
        [Import(typeof(ITime))]
        protected ITime time;
        public DateTime LastAccessTime { get; set; }
        public IDictionary<Guid, UserMediaCookie> MediaCookies { get; set; }
        public UserMediaCookie this[Guid mediaId]
        {
            get
            {
                this.LastAccessTime = time.Now;
                if (this.MediaCookies != null && this.MediaCookies.ContainsKey(mediaId))
                {
                    return this.MediaCookies[mediaId];
                }
                return null;
            }
            set
            {
                this.LastAccessTime = time.Now;
                if (this.MediaCookies == null)
                {
                    this.MediaCookies = new Dictionary<Guid, UserMediaCookie>();
                }
                this.MediaCookies[mediaId] = value;
            }
        }
    }
    public class UserMediaCookie
    {
        protected IDictionary<string, UserCookieItemBase> items { get; set; }
        public UserCookieItem<T> GetUserCookieItem<T>(string key)
        {
            return items[key] as UserCookieItem<T>;
        }
        public void SetUserCookieItem<T>(string key,UserCookieItem<T> value)
        {
            this.items[key] = value;
        }
    }

    public abstract class UserCookieItemBase
    {
        public string Key { get; set; }
        public DateTime? ExPireDate { get; set; }
    }
    public class UserCookieItem<T>:UserCookieItemBase
    {
        public T Value
        {
            get;
            set;
        }
    }
}

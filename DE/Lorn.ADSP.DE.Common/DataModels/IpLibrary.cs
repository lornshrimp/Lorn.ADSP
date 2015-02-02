using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Lorn.ADSP.DE.DataModels
{
    /// <summary>
    /// Ip库
    /// </summary>
    public class IpLibrary
    {
        public const string STR_IPLIBRARIES = "IpLibraries";
        public SortedSet<IpMap> IpMaps { get; set; }
        public class IpMap
        {
            public BigInteger IpStart { get; set; }
            public BigInteger IpEnd { get; set; }
            public Guid RegionId { get; set; }
        }
        public class ByIpMap : Comparer<IpMap>
        {
            public override int Compare(IpMap x, IpMap y)
            {
                var result = Comparer<BigInteger>.Default.Compare(x.IpStart, y.IpStart);
                if (result != 0 )
                {
                    return result;
                }
                else
                {
                    result = Comparer<BigInteger>.Default.Compare(x.IpEnd, y.IpEnd);
                    if (result != 0)
                    {
                        return result;
                    }
                    else
                        return Comparer<Guid>.Default.Compare(x.RegionId, y.RegionId);
                }
            }
        }
    }
}

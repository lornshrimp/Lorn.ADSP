using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Lorn.Permissions.DataModels
{
    [Serializable]
    public partial class PreLoadPermissionInfo
    {
        [Key]
        public Guid ModuleId { get; set; }
        [Key]
        public Guid BusinessObjectId { get; set; }
        [Key]
        public int BusinessObjectType { get; set; }
        [Key]
        public Guid OperationId { get; set; }
        public bool HavePermission { get; set; }
    }
}

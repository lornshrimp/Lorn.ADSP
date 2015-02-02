using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.ComponentModel.Composition;
using System.Runtime.Caching;
using Lorn.Permissions.DataModels;
using Lorn.Permissions.DAL;
using Lorn.Permissions.Interfaces;

namespace Lorn.Permissions.BL
{
    [Export(typeof(IPermissionController))]
    public class PermissionController
    {
        public const string PermissionCacheMode = "PermissionCacheMode";
        public const string AllPermissions = "AllPermissions";
        public const string RBusinessWorkers = "RBusinessWorkers";
        protected PermissionsEntities dataContainer;
        protected ObjectCache cache;

        public PermissionController(PermissionsEntities dataContainer, ObjectCache cache)
        {
            this.dataContainer = dataContainer;
            this.cache = cache;
        }

        public bool CacheMode
        {
            get
            {
                if (this.cache.Get(PermissionCacheMode) == null)
                {
                    return false;
                }
                else
                {
                    return (bool)this.cache.Get(PermissionCacheMode);
                }
            }
            set
            {
                this.cache.Set(PermissionCacheMode,value,new CacheItemPolicy());
                //if (value == false)
                //{
                //    this.cache.ClearDataSession(AllPermissions);
                //    this.cache.ClearDataSession(RBusinessWorkers);
                //}
            }
        }

        #region IPermissionController Members

        public virtual bool CheckPermission(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId)
        {
            return CheckPermission(moduleId, businessWorkerId, businessObjectType, businessObjectId, null);
        }

        public virtual bool CheckPermission(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, DateTime checktime)
        {
            return CheckPermission(moduleId, businessWorkerId, businessObjectType, businessObjectId, null, DateTime.Now);
        }

        public virtual IEnumerable<Permission> GetPermissions(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId)
        {
            return GetPermissions(moduleId, businessWorkerId, businessObjectType, businessObjectId, null);
        }

        public virtual IEnumerable<Permission> GetPermissions(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, DateTime checktime)
        {
            return GetPermissions(moduleId, businessWorkerId, businessObjectType, businessObjectId, null, checktime);
        }

        public virtual void AssignPermission(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, DateTime startTime, bool IsDeny, Guid? operationId = null, DateTime? endTime = null, string permissionName = null, string remark = null)
        {
            Contract.Requires(dataContainer.BusinessWorkers.AsParallel().Any(o => o.BusinessWorkerId == businessWorkerId));
            //查找是否已经有符合的授权记录
            var permissions = dataContainer.Permissions.AsParallel().Where(o => o.ModuleId == moduleId && o.BusinessWorker_BusinessWorkerId == businessWorkerId && o.ControlBusinessObjectType == businessObjectType && o.ControlBusinessObjectId == businessObjectId && o.OperationId == operationId && o.IsDeny == IsDeny);
            if (endTime == null)
            {
                permissions = permissions.AsParallel().Where(o => o.EndDate == null);
            }
            else
            {
                permissions = permissions.AsParallel().Where(o => o.EndDate == null || o.EndDate >= endTime);
            }
            //如果没有符合的授权记录，则插入该授权记录
            if (permissions.Count() == 0)
            {
                Permission permission = new Permission();
                permission.PermissionId = Guid.NewGuid();
                permission.PermissionName = permissionName;
                permission.ControlBusinessObjectType = businessObjectType;
                permission.ControlBusinessObjectId = businessObjectId;
                permission.OperationId = operationId;
                permission.StartDate = startTime;
                permission.EndDate = endTime;
                permission.IsDeny = IsDeny;
                permission.Remark = remark;
                permission.ModuleId = moduleId;
                permission.BusinessWorker_BusinessWorkerId = businessWorkerId;
                dataContainer.Permissions.Add(permission);
                dataContainer.SaveChanges();
            }
        }
        #endregion
        protected virtual IEnumerable<Permission> GetPermissions(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, Guid? operationId, DateTime checktime, IList<Guid> checkedBusinessWorkIds)
        {
            IList<Permission> allPermissions = new List<Permission>();
            IList<RBusinessWorker> rBusinessWorkers = new List<RBusinessWorker>();
            if (CacheMode == true)
            {
                if (this.cache.Get(AllPermissions) == null)
                {
                    this.cache.Set(AllPermissions, dataContainer.Permissions.ToList(),new CacheItemPolicy());

                }
                if (this.cache.Get(RBusinessWorkers) == null)
                {
                    this.cache.Set(RBusinessWorkers, dataContainer.RBusinessWorkers.Include("ChildBusinessWorker").Include("ParentBusinessWorker").ToList(),new CacheItemPolicy());

                }
                allPermissions = this.cache.Get(AllPermissions) as IList<Permission>;
                rBusinessWorkers = this.cache.Get(RBusinessWorkers) as IList<RBusinessWorker>; ;
            }
            else
            {
                lock (dataContainer)
                {
                    //dataContainer.Connection.Open();
                    allPermissions = dataContainer.Permissions.ToList();
                    rBusinessWorkers = dataContainer.RBusinessWorkers.Include("ChildBusinessWorker").Include("ParentBusinessWorker").ToList();
                    //dataContainer.Connection.Close();
                }
            }
            return GetPermissions(moduleId, businessWorkerId, businessObjectType, businessObjectId, operationId, checktime, checkedBusinessWorkIds, allPermissions, rBusinessWorkers);
        }

        private IEnumerable<Permission> GetPermissions(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, Guid? operationId, DateTime checktime, IList<Guid> checkedBusinessWorkIds, IList<Permission> allPermissions, IList<RBusinessWorker> rBusinessWorkers)
        {
            IEnumerable<Permission> permissions;
            //查询直接赋给该业务工人的权限
            permissions = GetDirectPermissions(allPermissions, moduleId, businessWorkerId, businessObjectType, businessObjectId, operationId, checktime);
            checkedBusinessWorkIds.Add(businessWorkerId);
            //查询该业务工人的非拒绝的已启用的未查询过权限的直接父级业务工人
            var v = GetDirectParentBusinessWorkerRelation(rBusinessWorkers, businessWorkerId, checktime);
            var parentBusinessWorkers = v.GroupBy(o => o.ParentBusinessWorker).AsParallel().Where(o =>
                o.All(p => p.IsDeny == false) && o.Key.Enabled == true && !checkedBusinessWorkIds.Contains(o.Key.BusinessWorkerId)
                ).Select(o => o.Key);
#if DEBUG
            int parentBusinessWorkersCount = parentBusinessWorkers.Count();
#endif
            //查询父级业务工人的权限
            parentBusinessWorkers.ForAll((o) => permissions = permissions.Union(GetPermissions(moduleId, o.BusinessWorkerId, businessObjectType, businessObjectId, operationId, checktime, checkedBusinessWorkIds, allPermissions, rBusinessWorkers)));
            return permissions;
        }

        /// <summary>
        /// 获取业务工人的直接父业务工人关系
        /// </summary>
        /// <param name="childBusinessWorkerId">子业务工人Id</param>
        /// <param name="checkTime">查询时间</param>
        /// <returns>直接父业务工人关系</returns>
        protected virtual IEnumerable<RBusinessWorker> GetDirectParentBusinessWorkerRelation(IList<RBusinessWorker> rBusinessWorkers, Guid childBusinessWorkerId, DateTime checkTime)
        {
            var data = rBusinessWorkers.Where(o =>
                o.ChildBusinessWorker_BusinessWorkerId == childBusinessWorkerId && (o.EndDate == null || o.EndDate > checkTime) && o.StartDate <= checkTime);
#if DEBUG
            int count = data.Count();
#endif
            return data;
        }

        /// <summary>
        /// 获取直接赋给业务工人的权限记录
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="businessWorkerId">业务工人Id</param>
        /// <param name="businessObjectType">业务对象类型</param>
        /// <param name="businessObjectId">业务对象Id</param>
        /// <param name="checktime">查询时间</param>
        /// <returns>是否拥有权限</returns>
        protected virtual IEnumerable<Permission> GetDirectPermissions(IList<Permission> allPermissions, Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, Guid? operationId, DateTime checktime)
        {

            var permissions = allPermissions.Where(o =>
                o.ModuleId == moduleId && o.ControlBusinessObjectType == businessObjectType && o.ControlBusinessObjectId == businessObjectId && o.OperationId == operationId && o.BusinessWorker_BusinessWorkerId == businessWorkerId && (o.EndDate == null || o.EndDate > checktime) && o.StartDate <= checktime);
#if DEBUG
            int permissionsCount = permissions.Count();
#endif
            return permissions;
        }


        public bool CheckPermission(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, Guid? operationId)
        {
            return CheckPermission(moduleId, businessWorkerId, businessObjectType, businessObjectId, operationId, DateTime.Now);
        }

        public bool CheckPermission(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, Guid? operationId, DateTime checktime)
        {
            var permissions = GetPermissions(moduleId, businessWorkerId, businessObjectType, businessObjectId, operationId, checktime);
            if (permissions.AsParallel().Any(o => o.IsDeny == true))
            {
                return false;
            }
            else if (permissions.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<Permission> GetPermissions(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, Guid? operationId)
        {
            return GetPermissions(moduleId, businessWorkerId, businessObjectType, businessObjectId, operationId, DateTime.Now);
        }

        public IEnumerable<Permission> GetPermissions(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, Guid? operationId, DateTime checktime)
        {
            return GetPermissions(moduleId, businessWorkerId, businessObjectType, businessObjectId, operationId, checktime, new List<Guid>());
        }


        public IList<PreLoadPermissionInfo> PreLoadPermissions(Guid businessWorkerId)
        {
            List<PreLoadPermissionInfo> preLoadPermissions = new List<PreLoadPermissionInfo>();
            foreach (var permission in dataContainer.PreLoadPermissions)
            {
                preLoadPermissions.Add(new PreLoadPermissionInfo() { BusinessObjectId = permission.ControlBusinessObjectId, ModuleId = permission.ModuleId, BusinessObjectType = permission.ControlBusinessObjectType, OperationId = permission.OperationId.HasValue ? permission.OperationId.Value : Guid.Empty, HavePermission = CheckPermission(permission.ModuleId, businessWorkerId, permission.ControlBusinessObjectType, permission.ControlBusinessObjectId, permission.OperationId) });
            }
            return preLoadPermissions;
        }
    }
}

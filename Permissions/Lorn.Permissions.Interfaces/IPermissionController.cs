using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.Permissions.DataModels;

namespace Lorn.Permissions.Interfaces
{
    public interface IPermissionController
    {
        /// <summary>
        /// 检查用户权限
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="businessWorkerId">用户Id</param>
        /// <param name="businessObjectType">业务对象类型</param>
        /// <param name="businessObjectId">业务对象Id</param>
        /// <returns>是否拥有权限</returns>
        bool CheckPermission(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId);
        /// <summary>
        /// 检查用户权限
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="businessWorkerId">用户Id</param>
        /// <param name="businessObjectType">业务对象类型</param>
        /// <param name="businessObjectId">业务对象Id</param>
        /// <param name="operationId">操作Id</param>
        /// <returns>是否拥有权限</returns>
        bool CheckPermission(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, Guid? operationId);
        /// <summary>
        /// 检查指定时间点的用户权限
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="businessObjectType">业务对象类型</param>
        /// <param name="businessObjectId">业务对象Id</param>
        /// <param name="checktime">检查的时间点</param>
        /// <returns>是否拥有权限</returns>
        bool CheckPermission(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, DateTime checktime);
        /// <summary>
        /// 检查指定时间点的用户权限
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="businessObjectType">业务对象类型</param>
        /// <param name="businessObjectId">业务对象Id</param>
        /// <param name="operationId">操作Id</param>
        /// <param name="checktime">检查的时间点</param>
        /// <returns>是否拥有权限</returns>
        bool CheckPermission(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, Guid? operationId, DateTime checktime);
        /// <summary>
        /// 获取权限信息
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="businessWorkerType">业务工人类型</param>
        /// <param name="businessWorkerId">业务工人Id</param>
        /// <param name="businessObjectType">业务对象类型</param>
        /// <param name="businessObjectId">业务对象Id</param>
        /// <returns>权限集合</returns>
        IEnumerable<Permission> GetPermissions(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId);
        /// <summary>
        /// 获取权限信息
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="businessWorkerType">业务工人类型</param>
        /// <param name="businessWorkerId">业务工人Id</param>
        /// <param name="businessObjectType">业务对象类型</param>
        /// <param name="businessObjectId">业务对象Id</param>
        /// <param name="operationId">操作Id</param>
        /// <returns>权限集合</returns>
        IEnumerable<Permission> GetPermissions(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, Guid? operationId);
        /// <summary>
        /// 获取指定时间点权限信息
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="businessWorkerType">业务工人类型</param>
        /// <param name="businessWorkerId">业务工人Id</param>
        /// <param name="businessObjectType">业务对象类型</param>
        /// <param name="businessObjectId">业务对象Id</param>
        /// <param name="checktime">检查的时间点</param>
        /// <returns>权限集合</returns>
        IEnumerable<Permission> GetPermissions(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, DateTime checktime);
        /// <summary>
        /// 获取指定时间点权限信息
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="businessWorkerType">业务工人类型</param>
        /// <param name="businessWorkerId">业务工人Id</param>
        /// <param name="businessObjectType">业务对象类型</param>
        /// <param name="businessObjectId">业务对象Id</param>
        /// <param name="operationId">操作Id</param>
        /// <param name="checktime">检查的时间点</param>
        /// <returns>权限集合</returns>
        IEnumerable<Permission> GetPermissions(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, Guid? operationId, DateTime checktime);
        /// <summary>
        /// 配置权限
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="permissionName">权限名称（可选）</param>
        /// <param name="businessWorkerType">业务工人类型</param>
        /// <param name="businessWorkerId">业务工人Id</param>
        /// <param name="businessObjectType">业务对象类型</param>
        /// <param name="businessObjectId">业务对象Id</param>
        /// <param name="operationId">操作Id</param>
        /// <param name="startTime">生效时间</param>
        /// <param name="endTime">失效时间</param>\
        /// <param name="IsDeny">是否为拒绝权限</param>
        /// <param name="remark">备注（可选）</param>
        void AssignPermission(Guid moduleId, Guid businessWorkerId, int businessObjectType, Guid businessObjectId, DateTime startTime, bool IsDeny, Guid? operationId = null, DateTime? endTime = null, string permissionName = null, string remark = null);

        /// <summary>
        /// 预加载权限
        /// </summary>
        /// <returns></returns>
        IList<PreLoadPermissionInfo> PreLoadPermissions(Guid businessWorkerId);

        /// <summary>
        /// 获取或设置是否是缓存模式
        /// </summary>
        bool CacheMode
        {
            get;
            set;
        }
    }
}

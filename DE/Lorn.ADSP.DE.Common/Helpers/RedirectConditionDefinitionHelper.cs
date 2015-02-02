using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lorn.ADSP.DE.DataModels;

namespace Lorn.ADSP.DE.Helpers
{
    public static class RedirectConditionDefinitionHelper
    {
        public static RedirectConditionDefinition FindRedirectConditionDefinition(this ICollection<RedirectConditionDefinition> redirectConditionDefinitions, Guid redirectConditionDefinitionId)
        {
            RedirectConditionDefinition difinition = null;
            foreach (var item in redirectConditionDefinitions)
            {
                if (item.RedirectConditionDefinitionId == redirectConditionDefinitionId)
                {
                    difinition = item;
                    return difinition;
                }
                else if (item.ChildRedirectConditionDefinitions != null)
                {
                    difinition = FindRedirectConditionDefinition(item.ChildRedirectConditionDefinitions, redirectConditionDefinitionId);
                    if (difinition != null)
                    {
                        return difinition;
                    }
                }
            }
            return difinition;
        }

        public static RedirectConditionDefinition FindRedirectConditionDefinition(this ICollection<RedirectConditionDefinition> redirectConditionDefinitions, string redirectConditionDefinitionCode)
        {
            RedirectConditionDefinition difinition = null;
            foreach (var item in redirectConditionDefinitions)
            {
                if (item.ConditionCode != null && item.ConditionCode.ToLower().Trim() == redirectConditionDefinitionCode.ToLower().Trim())
                {
                    difinition = item;
                    return difinition;
                }
                else if (item.ChildRedirectConditionDefinitions != null)
                {
                    difinition = FindRedirectConditionDefinition(item.ChildRedirectConditionDefinitions, redirectConditionDefinitionCode);
                    if (difinition != null)
                    {
                        return difinition;
                    }
                }
            }
            return difinition;
        }

        public static bool RetriveRedirectCondition(this ICollection<RedirectConditionDetail> redirectConditionDetails, RedirectDimension redirectDemension, Guid requestConditionId)
        {
            return redirectConditionDetails.AsParallel().Any(o => o.RedirectConditionDefinitionId == requestConditionId || FindRedirectConditionDefinition(FindRedirectConditionDefinition(redirectDemension.RedirectConditionDefinitions, o.RedirectConditionDefinitionId.Value).ChildRedirectConditionDefinitions, requestConditionId) != null);
        }

        public static bool RetriveRedirectCondition(this ICollection<RedirectConditionDetail> redirectConditionDetails, RedirectDimension redirectDemension, string requestConditionDefinitionCode)
        {
            var requestCondition = FindRedirectConditionDefinition(redirectDemension.RedirectConditionDefinitions, requestConditionDefinitionCode);
            if (requestCondition == null)
            {
                return false;
            }
            return redirectConditionDetails.AsParallel().Any(o => o.RedirectConditionDefinitionId == requestCondition.RedirectConditionDefinitionId || FindRedirectConditionDefinition(FindRedirectConditionDefinition(redirectDemension.RedirectConditionDefinitions, o.RedirectConditionDefinitionId.Value).ChildRedirectConditionDefinitions, requestCondition.RedirectConditionDefinitionId) != null);
        }
    }
}

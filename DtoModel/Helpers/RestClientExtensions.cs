using System.Threading.Tasks;
using DtoModel.AccessControl;
using DtoModel.OrgStructure;
using DtoModel.Reporting;

namespace DtoModel.Helpers
{
    public static class RestClientExtensions
    {
        public static async Task<DepartmentSites[]> GetDepartmentSites(this RestClient client)
        {
            return await client.Get<DepartmentSites[]>("api/OrganizationalStructure/GetDepartmnetSites");
        }

        public static async Task<DepartmentStructure[]> GetDepartmentStructure(this RestClient client)
        {
            return await client.Get<DepartmentStructure[]>("api/OrganizationalStructure/GetDepartments");            
        }

        public static async Task<ActiveControlDelegation[]> GetActiveDelegations(this RestClient client)
        {
            return await client.Get<ActiveControlDelegation[]>("api/AccessControl/GetActiveDelegations");
        }

        public static async Task DeleteDelegation(this RestClient client, string receivingUser)
        {
            await client.Delete("api/secured/AccessControl/DisableDelegation/", receivingUser);
        }

        public static async Task<ActiveControlDelegation> CreateDelegation(this RestClient client, ManageDelegationRecord request)
        {
            return await client.Put<ManageDelegationRecord, ActiveControlDelegation>("api/secured/AccessControl/EnableDelegation", request);
        }
        
        public static async Task<DepartmentStructure> GetDepartmentStructure(this RestClient client, string department)
        {
            return await client.Get<DepartmentStructure>("api/OrganizationalStructure/GetDepartment/" + department);
        }

        public static async Task<FacilityAccessPoint[]> GetSiteFacilityAccessPoints(this RestClient client, string site)
        {
            return await client.Get<FacilityAccessPoint[]>("api/secured/AccessControl/GetFacilityAccessPoints/" + site);
        }

        public static async Task<GroupRecord[]> GetGroups(this RestClient client)
        {
            return await client.Get<GroupRecord[]>("api/OrganizationalStructure/GetGroups");
        }

        public static async Task<SecurityRule[]> GetSiteFacilityAccessRules(this RestClient client, string site)
        {
            return await client.Get<SecurityRule[]>("api/secured/AccessControl/GetFacilityAccessRules/" + site);
        }

        public static async Task<SecurityRule> GrantPermissions(this RestClient client, string accessPoint, string receiver)
        {
            return await client.Put<SecurityRule, SecurityRule>("api/secured/AccessControl/GrantPermissions", new SecurityRule { AccessPointName = accessPoint, EffectiveFor = receiver });
        }

        public static async Task RevokePermissions(this RestClient client, string accessPoint, string receiver)
        {
            await client.Delete("api/secured/AccessControl/RevokePermissions/", accessPoint + "/" + receiver);
        }

        public static async Task<bool> RegisterActivityLogs(this RestClient client, ActivityReportBatch logs)
        {
            return await client.Put<ActivityReportBatch, bool>("api/Reporting/RegisterActivityLogs", logs);
        }

        public static async Task<bool> NotifySecurityBreach(this RestClient client, SecurityBreach securityBreach)
        {
            return await client.Put<SecurityBreach, bool>("api/Reporting/NotifySecurityBreach", securityBreach);
        }
    }
}

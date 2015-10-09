/// Orchard Collaboration is a plugin for Orchard CMS that provides an integrated ticketing system for it.
/// Copyright (C) 2014-2015  Siyamand Ayubi
///
/// This file is part of Orchard Collaboration.
///
///    Orchard Collaboration is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License as published by
///    the Free Software Foundation, either version 3 of the License, or
///    (at your option) any later version.
///
///    Orchard Collaboration is distributed in the hope that it will be useful,
///    but WITHOUT ANY WARRANTY; without even the implied warranty of
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
///    GNU General Public License for more details.
///
///    You should have received a copy of the GNU General Public License
///    along with Orchard Collaboration.  If not, see <http://www.gnu.org/licenses/>.

namespace Orchard.CRM.Core.Services
{
    using Orchard.Caching;
    using Orchard.ContentManagement;
    using Orchard.CRM.Core.Models;
    using Orchard.Data;
    using Orchard.Roles.Models;
    using Orchard.Security;
    using Orchard.Users.Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class BasicDataService : IBasicDataService
    {
        private readonly IRepository<ServiceRecord> servicePartRecordRepository;
        private readonly IRepository<TicketTypeRecord> ticketTypeRecordRepository;
        private readonly IRepository<RolesPermissionsRecord> rolesPermissionsRepository;
        private readonly ICacheManager chacheManager;
        private readonly IContentManager contentManager;
        private readonly IRepository<StatusRecord> statusRecordRepository;
        private readonly Object ourLock = new Object();
        private readonly IRepository<PriorityRecord> priorityRecordRepository;
        private readonly IRepository<BusinessUnitMemberPartRecord> businessUnitMemberRepository;
        private readonly IRepository<TeamMemberPartRecord> teamMemberRepository;
        protected IRepository<UserRolesPartRecord> userRolesRepository;

        private static bool RefreshServices = false;
        private static bool RefreshStatusRecords = false;
        private static bool RefreshPriorities = false;
        private static bool RefreshTicketTypes = false;
        private static bool RefreshBusinessUnitMembers = false;
        private static bool RefreshTeamMembers = false;
        private static bool RefreshBusinessUnits = false;
        private static bool RefreshTeams = false;
        private static bool RefreshOperatorUsers = false;
        private static bool RefreshCustomerAndOperatorUsers = false;
        private static int CustomersAndOperatorsCount = 0;

        public BasicDataService(
            ICacheManager chacheManager,
            IContentManager contentManager,
            IRepository<PriorityRecord> priorityRecordRepository,
            IRepository<StatusRecord> statusRecordRepository,
            IRepository<BusinessUnitMemberPartRecord> businessUnitMemberRepository,
            IRepository<TicketTypeRecord> ticketTypeRecordRepository,
            IRepository<TeamMemberPartRecord> teamMemberRepository,
            IRepository<UserRolesPartRecord> userRolesRepository,
            IRepository<RolesPermissionsRecord> rolesPermissionsRepository,
            IRepository<ServiceRecord> servicePartRecordRepository)
        {
            this.rolesPermissionsRepository = rolesPermissionsRepository;
            this.userRolesRepository = userRolesRepository;
            this.teamMemberRepository = teamMemberRepository;
            this.statusRecordRepository = statusRecordRepository;
            this.chacheManager = chacheManager;
            this.priorityRecordRepository = priorityRecordRepository;
            this.ticketTypeRecordRepository = ticketTypeRecordRepository;
            this.servicePartRecordRepository = servicePartRecordRepository;
            this.businessUnitMemberRepository = businessUnitMemberRepository;
            this.contentManager = contentManager;
        }

        public void ClearCache()
        {
            lock (ourLock)
            {
                RefreshServices = true;
                RefreshStatusRecords = true;
                RefreshPriorities = true;
                RefreshTicketTypes = true;
                RefreshBusinessUnitMembers = true;
                RefreshTeamMembers = true;
                RefreshBusinessUnits = true;
                RefreshTeams = true;
                RefreshOperatorUsers = true;
                RefreshCustomerAndOperatorUsers = true;
            }
        }

        public IEnumerable<BusinessUnitMemberPartRecord> GetBusinessUnitMembers()
        {
            return this.chacheManager.Get("BusinessUnitMembers", context =>
            {
                context.Monitor(new Token(() => !RefreshBusinessUnitMembers));
                var temp = this.contentManager.Query().ForType("BusinessUnitMember").ForVersion(VersionOptions.Published).List();

                var returnValue = temp.Select(c => c.As<BusinessUnitMemberPart>().Record);

                lock (ourLock)
                {
                    RefreshBusinessUnitMembers = false;
                }

                return returnValue;
            });
        }

        public IEnumerable<ContentItem> GetBusinessUnits()
        {
            return this.chacheManager.Get("BusinessUnits", context =>
            {
                context.Monitor(new Token(() => !RefreshBusinessUnits));
                var returnValue = this.contentManager.HqlQuery().ForType("BusinessUnit").List();

                lock (ourLock)
                {
                    RefreshBusinessUnits = false;
                }

                return returnValue;
            });
        }

        public IEnumerable<IUser> GetOperators()
        {
            return this.chacheManager.Get("Operators", context =>
            {
                context.Monitor(new Token(() => !RefreshOperatorUsers));

                var roles = this.rolesPermissionsRepository.Table.Where(c =>
                    (c.Permission.Name == Permissions.OperatorPermission.Name ||
                    c.Permission.Name == Permissions.AdvancedOperatorPermission.Name) &&
                    c.Permission.FeatureName == "Orchard.CRM.Core").Select(c => c.Role.Id).ToArray();

                var userRoles = this.userRolesRepository.Table.Where(c =>
                roles.Contains(c.Role.Id)).ToList();

                IEnumerable<int> userIds = userRoles.Select(c => c.UserId).Distinct();
                var users = this.contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty);

                lock (ourLock)
                {
                    RefreshOperatorUsers = false;
                }

                return users;
            });
        }

        public IUser GetOperatorOrCustomerUser(int id)
        {
            if (CustomersAndOperatorsCount < 2000)
            {
                var customerAndOperators = this.GetOperatorAndCustomers();
                if (customerAndOperators != null)
                {
                    return customerAndOperators.FirstOrDefault(c => c.Id == id);
                }
            }

            IUser user = this.contentManager.Get<IUser>(id);
            return user;
        }

        public IUser GetOperatorOrCustomerUser(string email)
        {
            var user = this.contentManager.Query<UserPart, UserPartRecord>().Where(c => c.Email == email).List().FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            var roles = this.rolesPermissionsRepository.Table.Where(c =>
                   (c.Permission.Name == Permissions.OperatorPermission.Name ||
                    c.Permission.Name == Permissions.CustomerPermission.Name ||
                    c.Permission.Name == Permissions.AdvancedOperatorPermission.Name) &&
                    c.Permission.FeatureName == "Orchard.CRM.Core").Select(c => c.Role.Id).ToArray();

            var userRoles = this.userRolesRepository.Table.Where(c =>
              roles.Contains(c.Role.Id) && c.UserId == user.Id).ToList();

            if (userRoles.Count == 0)
            {
                return null;
            }

            return user.As<IUser>();
        }
        private List<IUser> GetOperatorAndCustomers()
        {
            return this.chacheManager.Get("Operators", context =>
            {
                context.Monitor(new Token(() => !RefreshCustomerAndOperatorUsers));

                var roles = this.rolesPermissionsRepository.Table.Where(c =>
                    (c.Permission.Name == Permissions.OperatorPermission.Name ||
                    c.Permission.Name == Permissions.CustomerPermission.Name ||
                    c.Permission.Name == Permissions.AdvancedOperatorPermission.Name) &&
                    c.Permission.FeatureName == "Orchard.CRM.Core").Select(c => c.Role.Id).ToArray();

                CustomersAndOperatorsCount = this.userRolesRepository.Table.Count(c =>
                roles.Contains(c.Role.Id));

                if (CustomersAndOperatorsCount > 2000)
                {
                    return null;
                }

                var userRoles = this.userRolesRepository.Table.Where(c =>
                roles.Contains(c.Role.Id)).ToList();

                IEnumerable<int> userIds = userRoles.Select(c => c.UserId).Distinct();
                var users = this.contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty);

                lock (ourLock)
                {
                    RefreshCustomerAndOperatorUsers = false;
                }

                return users.ToList();
            });
        }

        public IEnumerable<ContentItem> GetTeams()
        {
            return this.chacheManager.Get("Teams", context =>
            {
                context.Monitor(new Token(() => !RefreshTeams));
                var returnValue = this.contentManager.HqlQuery().ForType("Team").List();

                lock (ourLock)
                {
                    RefreshTeams = false;
                }

                return returnValue;
            });
        }

        public IEnumerable<TeamMemberPartRecord> GetTeamMembers()
        {
            return this.chacheManager.Get("TeamMembers", context =>
            {
                context.Monitor(new Token(() => !RefreshTeamMembers));

                var temp = this.contentManager.Query().ForType("TeamMember").ForVersion(VersionOptions.Published).List();

                var returnValue = temp.Select(c => c.As<TeamMemberPart>().Record);

                lock (ourLock)
                {
                    RefreshTeamMembers = false;
                }

                return returnValue;
            });
        }

        public IEnumerable<ServiceRecord> GetServices()
        {
            return this.chacheManager.Get("TicketServices", context =>
            {
                context.Monitor(new Token(() => !RefreshServices));
                var returnValue = this.servicePartRecordRepository.Table.Where(c => c.Deleted == false).ToList();

                lock (ourLock)
                {
                    RefreshServices = false;
                }

                return returnValue;
            });
        }

        public IEnumerable<PriorityRecord> GetPriorities()
        {
            return this.chacheManager.Get("TicketPriorities", context =>
            {
                context.Monitor(new Token(() => !RefreshPriorities));
                var returnValue = this.priorityRecordRepository.Table.ToList();

                lock (ourLock)
                {
                    RefreshPriorities = false;
                }

                return returnValue;
            });
        }

        public IEnumerable<StatusRecord> GetStatusRecords()
        {
            return this.chacheManager.Get("StatusRecords", context =>
            {
                context.Monitor(new Token(() => !RefreshStatusRecords));
                var returnValue = this.statusRecordRepository.Table.Where(c => c.Deleted == false).OrderBy(c => c.OrderId).ToList();

                lock (ourLock)
                {
                    RefreshStatusRecords = false;
                }

                return returnValue;
            });
        }

        public IEnumerable<TicketTypeRecord> GetTicketTypes()
        {
            return this.chacheManager.Get("TicketTypes", context =>
            {
                context.Monitor(new Token(() => !RefreshTicketTypes));
                var returnValue = this.ticketTypeRecordRepository.Table.Where(c => c.Deleted == false).ToList();

                lock (ourLock)
                {
                    RefreshTicketTypes = false;
                }

                return returnValue;
            });
        }

        public class Token : IVolatileToken
        {
            public Token(Func<bool> evaluate)
            {
                this.Evaluate = evaluate;
            }

            public Func<bool> Evaluate { get; private set; }
            public bool IsCurrent
            {
                get
                {
                    return this.Evaluate();
                }
                set
                {

                }
            }
        }
    }
}
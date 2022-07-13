using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Trident.Contracts;
using Trident.Logging;
using Trident.Mapper;
using Trident.Samples.AzureFunctions.Configuration;
using Trident.Samples.AzureFunctions.Functions.Organization;
using Trident.Samples.AzureFunctions.Models.Organization;
using Trident.Samples.Contracts;
using Trident.Samples.Domain.Entities;
using Trident.Testing;
using Trident.Testing.TestScopes;

namespace Trident.Samples.AzureFunctions.Test.Api.Functions
{
    [TestClass()]
    public class OrganizationtFunctionsTests
    {

        [TestMethod(), TestCategory(TestCategory.Unit)]
        public async Task StudentFunctions_GetStudentById_OkResponse()
        {
            // arrange
            var scope = new DefaultScope();
            var mockRequest = scope.CreateHttpRequest(null, null);
            var id = Guid.NewGuid();
            var organization = new OrganizationEntity { Id = id, Name = "Trident" };

            scope.organizationManagerMock.Setup(x => x.GetById(It.IsAny<Guid>(), false)).ReturnsAsync(organization);

            // act
            var response = await scope.InstanceUnderTest.GetOrganizationById(mockRequest, id);
            var actual = response;
            var actualContent = await response.ReadJsonToObject<Organization>();

            // assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actualContent);
        }

        [TestMethod(), TestCategory(TestCategory.Unit)]
        public async Task OrganizationtFunctions_GetOrganizationById_NotFoundResponse()
        {
            // arrange
            var scope = new DefaultScope();
            var mockRequest = scope.CreateHttpRequest(null, null);
            var id = Guid.NewGuid();
            OrganizationEntity organization = null;
            scope.organizationManagerMock.Setup(x => x.GetById(It.IsAny<Guid>(), false)).ReturnsAsync(organization);

            // act
            var response = await scope.InstanceUnderTest.GetOrganizationById(mockRequest, id);
            var actual = response;
            // assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(HttpStatusCode.NotFound, actual.StatusCode);
        }


        [TestMethod(), TestCategory(TestCategory.Unit)]
        public async Task OrganizationtFunctions_CreateOrganizaion_OkResponse()
        {
            // arrange
            var scope = new DefaultScope();
            var id = Guid.NewGuid();
            var json = JsonConvert.SerializeObject(new Organization { Id = id });
            var mockRequest = scope.CreateHttpRequest(null, json);

            var depts = new List<DepartmentEntity>();
            depts.Add(new DepartmentEntity { Id = Guid.NewGuid(), Name = "IT" });
            depts.Add(new DepartmentEntity { Id = Guid.NewGuid(), Name = "Marketing" });

            var entity = new OrganizationEntity { Id = id, Name = "Trident", Age = 10, Departments = depts };

            scope.organizationManagerMock.Setup(x => x.Save(It.IsAny<OrganizationEntity>(), false))
                .ReturnsAsync(entity);

            // act
            var response = await scope.InstanceUnderTest.CreateOrganization(mockRequest);
            var actual = response;
            var actualContent = await response.ReadJsonToObject<Organization>();

            // assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actualContent);
        }


        private class DefaultScope : HttpTestScope<OrganizationFunctions>
        {
            public DefaultScope()
            {
                TestOrganizationEntities = GetTestOrganizationEntities();
                TestOrganizations = GetTestOrganizations();

                var config = new MapperConfiguration(cfg => { cfg.AddProfile<ApiMapperProfile>(); });

                Mapper = new TestMapperRegistry();

                InstanceUnderTest = new OrganizationFunctions(AppLoggerMock.Object, Mapper, organizationManagerMock.Object);
            }

            // Not mocking mapper in order to validate entity/model mappings
            public IMapperRegistry Mapper { get; }
            public Mock<ILog> AppLoggerMock { get; } = new Mock<ILog>();

            public class TestMapperRegistry : AutoMapperRegistry
            {
                public TestMapperRegistry() : base(new MapperConfiguration(cfg =>
                {

                    cfg.AddProfile<ApiMapperProfile>();
                }))
                {

                }

            }
            public Mock<IManager<Guid, OrganizationEntity>> organizationManagerMock { get; } = new Mock<IManager<Guid, OrganizationEntity>>();

            public List<Organization> TestOrganizations { get; }
            public List<OrganizationEntity> TestOrganizationEntities { get; }

            private List<OrganizationEntity> GetTestOrganizationEntities()
            {
                return new List<OrganizationEntity>
                {
                    new OrganizationEntity
                    {
                        Id = Guid.NewGuid(), Name = "Trident", OrgType = OrganisationTypes.Corp
                    },
                    new OrganizationEntity
                    {
                        Id = Guid.NewGuid(), Name = "Microsoft", OrgType = OrganisationTypes.SCorp
                    },
                    new OrganizationEntity
                    {
                        Id = Guid.NewGuid(), Name = "Oracle", OrgType = OrganisationTypes.LLC
                    }
                };
            }

            private List<Organization> GetTestOrganizations()
            {
                return new List<Organization>
                {
                    new Organization {Id = Guid.NewGuid(), Name = "Trident", OrgType = OrganisationTypes.Corp},
                    new Organization {Id = Guid.NewGuid(), Name = "Microsoft", OrgType = OrganisationTypes.SCorp},
                    new Organization {Id = Guid.NewGuid(), Name = "Oracle", OrgType = OrganisationTypes.LLC}
                };
            }
        }
    }
}
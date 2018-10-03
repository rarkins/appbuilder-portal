﻿using System;
using Moq;
using OptimaJet.DWKit.StarterApplication.Data;
using OptimaJet.DWKit.StarterApplication.Models;
using OptimaJet.DWKit.StarterApplication.Services.BuildEngine;
using Project = OptimaJet.DWKit.StarterApplication.Models.Project;
using BuildEngineProject = SIL.AppBuilder.BuildEngineApiClient.Project;
using SIL.AppBuilder.BuildEngineApiClient;
using SIL.AppBuilder.Portal.Backend.Tests.Acceptance.Support;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Hangfire;
using OptimaJet.DWKit.StarterApplication.Repositories;
using JsonApiDotNetCore.Data;

namespace SIL.AppBuilder.Portal.Backend.Tests.Acceptance.BuildEngine
{
    [Collection("BuildEngineCollection")]
    public class BuildEngineProjectServiceTests : BaseTest<BuildEngineStartup>
    {
        // Skipping tests because getting DbUpdateConcurreyExceptions after the first couple of tests for unknown reasons
        // Each test can be run individually successfully
        const string skipAcceptanceTest = "Acceptance Test disabled"; // Set to null to be able to run/debug using Unit Test Runner
        public User CurrentUser { get; set; }
        public User user1 { get; set; }
        public OrganizationMembership CurrentUserMembership { get; set; }
        public OrganizationMembership organizationMembership1 { get; set; }
        public Organization org1 { get; private set; }
        public Group group1 { get; set; }
        public GroupMembership groupMembership1 { get; set; }
        public ApplicationType type1 { get; set; }
        public Project project1 { get; set; }
        public SystemStatus systemStatus1 { get; set; }
        public BuildEngineProjectServiceTests(TestFixture<BuildEngineStartup> fixture) : base(fixture)
        {
        }
        private void BuildTestData()
        {
            CurrentUser = NeedsCurrentUser();
            user1 = AddEntity<AppDbContext, User>(new User
            {
                ExternalId = "test-auth0-id1",
                Email = "test-email1@test.test",
                Name = "Test Testenson1",
                GivenName = "Test1",
                FamilyName = "Testenson1"
            });
            org1 = AddEntity<AppDbContext, Organization>(new Organization
            {
                Name = "TestOrg1",
                WebsiteUrl = "https://testorg1.org",
                BuildEngineUrl = "https://buildengine.testorg1",
                BuildEngineApiAccessToken = "replace",

            });
            CurrentUserMembership = AddEntity<AppDbContext, OrganizationMembership>(new OrganizationMembership
            {
                UserId = CurrentUser.Id,
                OrganizationId = org1.Id
            });
            organizationMembership1 = AddEntity<AppDbContext, OrganizationMembership>(new OrganizationMembership
            {
                UserId = user1.Id,
                OrganizationId = org1.Id
            });
            group1 = AddEntity<AppDbContext, Group>(new Group
            {
                Name = "TestGroup1",
                Abbreviation = "TG1",
                OwnerId = org1.Id
            });
            groupMembership1 = AddEntity<AppDbContext, GroupMembership>(new GroupMembership
            {
                UserId = user1.Id,
                GroupId = group1.Id
            });
            type1 = AddEntity<AppDbContext, ApplicationType>(new ApplicationType
            {
                Name = "scriptureappbuilder",
                Description = "Scripture App Builder"
            });
            project1 = AddEntity<AppDbContext, Project>(new Project
            {
                Name = "Test Project1",
                TypeId = type1.Id,
                Description = "Test Description",
                OwnerId = user1.Id,
                GroupId = group1.Id,
                OrganizationId = org1.Id,
                Language = "eng-US",
                IsPublic = true
            });
            systemStatus1 = AddEntity<AppDbContext, SystemStatus>(new SystemStatus
            {
                BuildEngineUrl = "https://buildengine.testorg1",
                BuildEngineApiAccessToken = "replace"
            });
        }
        [Fact(Skip = skipAcceptanceTest)]
        public void Project_Not_Found()
        {
            BuildTestData();
            var backgroundProjectRepository = _fixture.GetService<IJobRepository<Project>>();
            var systemStatusRepository = _fixture.GetService<IJobRepository<SystemStatus>>();
            var mockBuildEngine = new Mock<IBuildEngineApi>(); // _fixture.GetService<Mock<IBuildEngineApi>>();
            var mockRecurringJobManager = new Mock<IRecurringJobManager>();
            var buildProjectService = new BuildEngineProjectService(mockRecurringJobManager.Object, mockBuildEngine.Object, backgroundProjectRepository, systemStatusRepository);
            buildProjectService.ManageProject(999);
            // TODO: Verify notification
        }
        [Fact(Skip = skipAcceptanceTest)]
        public async Task Project_Connection_UnavailableAsync()
        {
            BuildTestData();
            var backgroundProjectRepository = _fixture.GetService<IJobRepository<Project>>();
            var systemStatusRepository = _fixture.GetService<IJobRepository<SystemStatus>>();
            var mockBuildEngine = new Mock<IBuildEngineApi>(); // _fixture.GetService<Mock<IBuildEngineApi>>();
            var mockRecurringJobManager = new Mock<IRecurringJobManager>();
            var buildProjectService = new BuildEngineProjectService(mockRecurringJobManager.Object, mockBuildEngine.Object, backgroundProjectRepository, systemStatusRepository);
            systemStatus1.SystemAvailable = false;
            var ex = await Assert.ThrowsAsync<Exception>(async () => await buildProjectService.ManageProjectAsync(project1.Id));
            Assert.Equal("Connection not available", ex.Message);

        }
        [Fact(Skip = skipAcceptanceTest)]
        public async Task Project_Connection_Not_Found()
        {
            BuildTestData();
            var backgroundProjectRepository = _fixture.GetService<IJobRepository<Project>>();
            var systemStatusRepository = _fixture.GetService<IJobRepository<SystemStatus>>();
            var mockBuildEngine = new Mock<IBuildEngineApi>(); // _fixture.GetService<Mock<IBuildEngineApi>>();
            var mockRecurringJobManager = new Mock<IRecurringJobManager>();
            var buildProjectService = new BuildEngineProjectService(mockRecurringJobManager.Object, mockBuildEngine.Object, backgroundProjectRepository, systemStatusRepository);
            systemStatus1.SystemAvailable = true;
            org1.BuildEngineApiAccessToken = "4323864";
            var ex = await Assert.ThrowsAsync<Exception>(async () => await buildProjectService.ManageProjectAsync(project1.Id));
            Assert.Equal("SystemStatus record for connection not found", ex.Message);
        }
        [Fact(Skip = skipAcceptanceTest)]
        public void Project_Create_Project()
        {
            BuildTestData();
            var backgroundProjectRepository = _fixture.GetService<IJobRepository<Project>>();
            var systemStatusRepository = _fixture.GetService<IJobRepository<SystemStatus>>();
            var mockBuildEngine = new Mock<IBuildEngineApi>(); // _fixture.GetService<Mock<IBuildEngineApi>>();
            var mockRecurringJobManager = new Mock<IRecurringJobManager>();
            var buildProjectService = new BuildEngineProjectService(mockRecurringJobManager.Object, mockBuildEngine.Object, backgroundProjectRepository, systemStatusRepository);
            mockBuildEngine.Reset();
            var projectResponse = new ProjectResponse
            {
                Id = 1,
                Status = "initialized",
                Result = "",
                Error = "",
                Url = ""
            };
            mockBuildEngine.Setup(x => x.CreateProject(It.IsAny<BuildEngineProject>())).Returns(projectResponse);
            systemStatus1.SystemAvailable = true;
            buildProjectService.ManageProject(project1.Id);
            mockBuildEngine.Verify(x => x.SetEndpoint(
                It.Is<String>(u => u == org1.BuildEngineUrl),
                It.Is<String>(t => t == org1.BuildEngineApiAccessToken)
            ));
            mockBuildEngine.Verify(x => x.CreateProject(
                It.Is<BuildEngineProject>(b => b.UserId == user1.Email)
            ));
            mockBuildEngine.Verify(x => x.CreateProject(
                It.Is<BuildEngineProject>(b => b.GroupId == group1.Abbreviation)
            ));
            mockBuildEngine.Verify(x => x.CreateProject(
                It.Is<BuildEngineProject>(b => b.AppId == type1.Name)
            ));
            mockBuildEngine.Verify(x => x.CreateProject(
                It.Is<BuildEngineProject>(b => b.ProjectName == project1.Name)
            ));
            var projects = ReadTestData<AppDbContext, Project>();
            var modifiedProject = projects.First(p => p.Id == project1.Id);
            Assert.Equal(1, modifiedProject.WorkflowProjectId);
        }
        [Fact(Skip = skipAcceptanceTest)]
        public async Task Project_Create_Project_FailedAsync()
        {
            BuildTestData();
            var backgroundProjectRepository = _fixture.GetService<IJobRepository<Project>>();
            var systemStatusRepository = _fixture.GetService<IJobRepository<SystemStatus>>();
            var mockBuildEngine = new Mock<IBuildEngineApi>(); // _fixture.GetService<Mock<IBuildEngineApi>>();
            var mockRecurringJobManager = new Mock<IRecurringJobManager>();
            var buildProjectService = new BuildEngineProjectService(mockRecurringJobManager.Object, mockBuildEngine.Object, backgroundProjectRepository, systemStatusRepository);
            mockBuildEngine.Reset();
            mockBuildEngine.Setup(x => x.CreateProject(It.IsAny<BuildEngineProject>())).Returns((ProjectResponse)null);
            systemStatus1.SystemAvailable = true;
            var ex = await Assert.ThrowsAsync<Exception>(async () => await buildProjectService.ManageProjectAsync(project1.Id));
            Assert.Equal("Create project failed", ex.Message);
         }
        [Fact(Skip = skipAcceptanceTest)]
        public async Task Project_Project_Completed()
        {
            BuildTestData();
            project1.WorkflowProjectId = 1;
            var backgroundProjectRepository = _fixture.GetService<IJobRepository<Project>>();
            var systemStatusRepository = _fixture.GetService<IJobRepository<SystemStatus>>();
            var mockBuildEngine = new Mock<IBuildEngineApi>(); // _fixture.GetService<Mock<IBuildEngineApi>>();
            var mockRecurringJobManager = new Mock<IRecurringJobManager>();
            var buildProjectService = new BuildEngineProjectService(mockRecurringJobManager.Object, mockBuildEngine.Object, backgroundProjectRepository, systemStatusRepository);
            mockBuildEngine.Reset();
            var projectResponse = new ProjectResponse
            {
                Id = 1,
                Status = "completed",
                Result = "SUCCESS",
                Error = "",
                Url = "ssh://APKAJU5Y3VNN3GHK3LLQ@git-codecommit.us-east-1.amazonaws.com/v1/repos/scriptureappbuilder-DEM-LSDEV-eng-US-Test-Project8"
            };
            mockBuildEngine.Setup(x => x.GetProject(It.IsAny<int>())).Returns(projectResponse);
            systemStatus1.SystemAvailable = true;
            await buildProjectService.ManageProjectAsync(project1.Id);
            mockBuildEngine.Verify(x => x.SetEndpoint(
                It.Is<String>(u => u == org1.BuildEngineUrl),
                It.Is<String>(t => t == org1.BuildEngineApiAccessToken)
            ));
            mockBuildEngine.Verify(x => x.GetProject(
                It.Is<int>(b => b == project1.WorkflowProjectId)
            ));
            var projects = ReadTestData<AppDbContext, Project>();
            var modifiedProject = projects.First(p => p.Id == project1.Id);
            Assert.Equal(projectResponse.Url, modifiedProject.WorkflowProjectUrl);
        }
        [Fact(Skip = skipAcceptanceTest)]
        public async Task Project_Project_Failed()
        {
            BuildTestData();
            project1.WorkflowProjectId = 1;
            var backgroundProjectRepository = _fixture.GetService<IJobRepository<Project>>();
            var systemStatusRepository = _fixture.GetService<IJobRepository<SystemStatus>>();
            var mockBuildEngine = new Mock<IBuildEngineApi>(); // _fixture.GetService<Mock<IBuildEngineApi>>();
            var mockRecurringJobManager = new Mock<IRecurringJobManager>();
            var buildProjectService = new BuildEngineProjectService(mockRecurringJobManager.Object, mockBuildEngine.Object, backgroundProjectRepository, systemStatusRepository);
            mockBuildEngine.Reset();
            var projectResponse = new ProjectResponse
            {
                Id = 1,
                Status = "completed",
                Result = "FAILURE",
                Error = "",
                Url = "ssh://APKAJU5Y3VNN3GHK3LLQ@git-codecommit.us-east-1.amazonaws.com/v1/repos/scriptureappbuilder-DEM-LSDEV-eng-US-Test-Project8"
            };
            mockBuildEngine.Setup(x => x.GetProject(It.IsAny<int>())).Returns(projectResponse);
            systemStatus1.SystemAvailable = true;
            await buildProjectService.ManageProjectAsync(project1.Id);
            mockBuildEngine.Verify(x => x.SetEndpoint(
                It.Is<String>(u => u == org1.BuildEngineUrl),
                It.Is<String>(t => t == org1.BuildEngineApiAccessToken)
            ));
            mockBuildEngine.Verify(x => x.GetProject(
                It.Is<int>(b => b == project1.WorkflowProjectId)
            ));
            var projects = ReadTestData<AppDbContext, Project>();
            var modifiedProject = projects.First(p => p.Id == project1.Id);
            Assert.Null(modifiedProject.WorkflowProjectUrl);
        }

    }
}
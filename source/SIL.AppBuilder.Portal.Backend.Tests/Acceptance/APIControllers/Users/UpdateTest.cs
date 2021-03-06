using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using OptimaJet.DWKit.StarterApplication.Data;
using OptimaJet.DWKit.StarterApplication.Models;
using SIL.AppBuilder.Portal.Backend.Tests.Acceptance.Support;
using SIL.AppBuilder.Portal.Backend.Tests.Support.StartupScenarios;
using Xunit;

namespace SIL.AppBuilder.Portal.Backend.Tests.Acceptance.APIControllers.Users
{
    [Collection("WithoutAuthCollection")]
    public class UpdateTests : BaseTest<NoAuthStartup>
    {
        public UpdateTests(TestFixture<NoAuthStartup> fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Patch_CurrentUser()
        {
            var tuple = NeedsConfiguredCurrentUser();

            var user = tuple.Item1;
            var id = user.Id;
            var oldName = user.GivenName;

            var expectedGivenName = oldName + "-new!";
            var payload = ResourcePatchPayload(
                "users", id, new Dictionary<string, object>()
                {
                    { "given-name", expectedGivenName }
                }
            );


            var response = await Patch("/api/users/" + id, payload);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedUser = await Deserialize<User>(response);

            Assert.Equal(expectedGivenName, updatedUser.GivenName);
        }


        [Fact]
        public async Task Patch_SomeUser()
        {
            var tuple = NeedsConfiguredCurrentUser();
            var user = AddEntity<AppDbContext, User>(new User { ExternalId = "n/a" });

            AddEntity<AppDbContext, OrganizationMembership>(new OrganizationMembership
            {
                UserId = user.Id,
                OrganizationId = tuple.Item2.OrganizationId
            });

            var expectedGivenName = user.GivenName + "-updated!";
            var payload = ResourcePatchPayload(
                "users", user.Id, new Dictionary<string, object>()
                {
                    { "given-name", expectedGivenName }
                });

            var response = await Patch("/api/users/" + user.Id, payload);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedUser = await Deserialize<User>(response);

            Assert.Equal(expectedGivenName, updatedUser.GivenName);
        }

        [Fact]
        public async Task Patch_SomeUser_FromTheWrongOrganization()
        {
            var tuple = NeedsConfiguredCurrentUser();
            var user = AddEntity<AppDbContext, User>(new User { ExternalId = "n/a" });
            var expectedGivenName = user.GivenName + "-updated!";
            var payload = ResourcePatchPayload(
                "users", user.Id, new Dictionary<string, object>()
                {
                    { "given-name", expectedGivenName }
                });

            var response = await Patch("/api/users/" + user.Id, payload);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Fact]
        public async Task Patch_SomeUser_WhenAnOrganizationIsSpecified_AndTheUserIsInThatOrganization()
        {
            var tuple = NeedsConfiguredCurrentUser();
            var user = AddEntity<AppDbContext, User>(new User { ExternalId = "n/a" });

            AddEntity<AppDbContext, OrganizationMembership>(new OrganizationMembership
            {
                UserId = user.Id,
                OrganizationId = tuple.Item2.OrganizationId
            });

            var expectedGivenName = user.GivenName + "-updated!";
            var payload = ResourcePatchPayload(
                "users", user.Id, new Dictionary<string, object>()
                {
                    { "given-name", expectedGivenName }
                });

            var response = await Patch("/api/users/" + user.Id, payload, tuple.Item2.OrganizationId.ToString());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedUser = await Deserialize<User>(response);

            Assert.Equal(expectedGivenName, updatedUser.GivenName);
        }

        [Fact]
        public async Task Patch_SomeUser_Different_Organization_NotFound()
        {
            NeedsConfiguredCurrentUser();
            var user = AddEntity<AppDbContext, User>(new User { ExternalId = "n/a" });

            var expectedGivenName = user.GivenName + "-updated!";
            var payload = ResourcePatchPayload(
                "users", user.Id, new Dictionary<string, object>()
                {
                    { "given-name", expectedGivenName }
                });

            var response = await Patch("/api/users/" + user.Id, payload, addOrgHeader: true);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task Patch_SomeUser_Good_PublishingKey()
        {
            var tuple = NeedsConfiguredCurrentUser();
            var user = AddEntity<AppDbContext, User>(new User { ExternalId = "n/a" });

            AddEntity<AppDbContext, OrganizationMembership>(new OrganizationMembership
            {
                UserId = user.Id,
                OrganizationId = tuple.Item2.OrganizationId
            });

            var publishingKey = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQCTF+wTVdaMDYmgeAZd7voe/b5MEHJWBXQDik14sqqj0aXtwV4+qxPU2ptqcjGpRk3ynmxp9i6Venw1JVf39iDFhWgd7VGBA7QEfApRm1v1FRI0wuN user1@user1MBP.local";
            var expectedGivenName = user.GivenName + "-updated!";
            var payload = ResourcePatchPayload(
                "users", user.Id, new Dictionary<string, object>()
                {
                    { "given-name", expectedGivenName },
                    { "publishing-key", publishingKey}
                });

            var response = await Patch("/api/users/" + user.Id, payload, tuple.Item2.OrganizationId.ToString());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedUser = await Deserialize<User>(response);

            Assert.Equal(expectedGivenName, updatedUser.GivenName);
            Assert.Equal(publishingKey, updatedUser.PublishingKey);
        }
        [Fact]
        public async Task Patch_SomeUser_Bad_PublishingKey()
        {
            var tuple = NeedsConfiguredCurrentUser();
            var user = AddEntity<AppDbContext, User>(new User { ExternalId = "n/a" });

            AddEntity<AppDbContext, OrganizationMembership>(new OrganizationMembership
            {
                UserId = user.Id,
                OrganizationId = tuple.Item2.OrganizationId
            });

            var publishingKey = "This is junk";
            var expectedGivenName = user.GivenName + "-updated!";
            var payload = ResourcePatchPayload(
                "users", user.Id, new Dictionary<string, object>()
                {
                    { "given-name", expectedGivenName },
                    { "publishing-key", publishingKey}
                });

            var response = await Patch("/api/users/" + user.Id, payload, tuple.Item2.OrganizationId.ToString());

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }
    }
}

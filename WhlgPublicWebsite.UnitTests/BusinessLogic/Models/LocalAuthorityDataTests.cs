using System.Linq;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Tests.BusinessLogic.Models.ExpectedLocalAuthorityData;
using static WhlgPublicWebsite.BusinessLogic.Models.LocalAuthorityData;

namespace Tests.BusinessLogic.Models;

/*
 * ⚠ IMPORTANT! ⚠
 *
 * The expected data for these tests is deliberately split into multiple files under ExpectedLocalAuthorityData.
 *
 * Do NOT refactor these tests to combine these expectations! The separation and duplication is by design.
 *
 * These tests were introduced in response to an incident in which some local authorities had their status inadvertently
 * reverted. This was caused by a mishandled merge that changed the whole LocalAuthorityDetailsByCustodianCode block.
 *
 * By separating out the expected data in these tests, we aim to ensure that each piece of information about each local
 * authority is documented independently. Combining these expectations would risk the exact merging problem that we are
 * trying to prevent.
 *
 * The longer term ideal state would be to move this data into the database, but these tests provide a defensive
 * mitigation until that is possible.
 *
 * For full details, see the report attached to: https://beisdigital.atlassian.net/browse/DESNZ-851
 */
[TestFixture]
public class LocalAuthorityDataTests
{
    [Test]
    public void LocalAuthorityDetailsByCustodianCode_EachPropertyHasOwnSeparateExpectedDataClass()
    {
        // Ensure that all properties of a local authority are documented in these tests
        // and that those expectations are NOT combined by later refactoring, as per the class comment.
        var localAuthorityProperties = typeof(LocalAuthorityDetails).GetProperties();
        var expectedDataClasses = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.Namespace == typeof(LocalAuthorityNames).Namespace);
        expectedDataClasses.Should().HaveSameCount(localAuthorityProperties);
    }

    [Test]
    public void LocalAuthorityDetailsByCustodianCode_HasCorrectNameForEveryLocalAuthority()
    {
        // Arrange
        var expectedNamesByCode = LocalAuthorityNames.NamesByCustodianCode();

        // Act & Assert
        const string fieldName = nameof(LocalAuthorityDetails.Name);
        foreach (var (code, name) in expectedNamesByCode)
        {
            using var _ = LocalAuthorityPropertyAssertionScope(fieldName, code);
            var localAuthority = LocalAuthorityDetailsByCustodianCode[code];
            localAuthority.Name.Should().Be(name);
        }

        LocalAuthorityDetailsByCustodianCode.Should().HaveSameCount(expectedNamesByCode);
    }

    [Test]
    public void LocalAuthorityDetailsByCustodianCode_HasCorrectStatusForEveryLocalAuthority()
    {
        // Arrange
        var expectedStatusesByCode = LocalAuthorityStatuses.StatusesByCustodianCode();

        // Act & Assert
        const string fieldName = nameof(LocalAuthorityDetails.Status);
        foreach (var (code, status) in expectedStatusesByCode)
        {
            using var _ = LocalAuthorityPropertyAssertionScope(fieldName, code);
            var localAuthority = LocalAuthorityDetailsByCustodianCode[code];
            localAuthority.Status.Should().Be(status);
        }

        LocalAuthorityDetailsByCustodianCode.Should().HaveSameCount(expectedStatusesByCode);
    }

    [Test]
    public void LocalAuthorityDetailsByCustodianCode_HasCorrectWebsiteUrlForEveryLocalAuthority()
    {
        // Arrange
        var expectedWebsiteUrlsByCustodianCode = LocalAuthorityWebsiteUrls.WebsiteUrlsByCustodianCode();

        // Act & Assert
        const string fieldName = nameof(LocalAuthorityDetails.WebsiteUrl);
        foreach (var (code, websiteUrl) in expectedWebsiteUrlsByCustodianCode)
        {
            using var _ = LocalAuthorityPropertyAssertionScope(fieldName, code);
            var localAuthority = LocalAuthorityDetailsByCustodianCode[code];
            localAuthority.WebsiteUrl.Should().Be(websiteUrl);
        }

        LocalAuthorityDetailsByCustodianCode.Should().HaveSameCount(expectedWebsiteUrlsByCustodianCode);
    }

    [Test]
    public void LocalAuthorityDetailsByCustodianCode_HasCorrectIncomeBandOptionsForEveryLocalAuthority()
    {
        // Arrange
        var expectedIncomeBandOptionsByCustodianCode = LocalAuthorityIncomeBands.IncomeBandOptionsByCustodianCode();

        // Act & Assert
        const string fieldName = nameof(LocalAuthorityDetails.IncomeBandOptions);
        foreach (var (code, incomeBandOptions) in expectedIncomeBandOptionsByCustodianCode)
        {
            using var _ = LocalAuthorityPropertyAssertionScope(fieldName, code);
            var localAuthority = LocalAuthorityDetailsByCustodianCode[code];
            localAuthority.IncomeBandOptions.Should().Equal(incomeBandOptions);
        }

        LocalAuthorityDetailsByCustodianCode.Should().HaveSameCount(expectedIncomeBandOptionsByCustodianCode);
    }

    [Test]
    public void LocalAuthorityDetailsByCustodianCode_HasCorrectConsortiumForEveryLocalAuthority()
    {
        // Arrange
        var expectedConsortiumsByCustodianCode = LocalAuthorityConsortiums.ConsortiumsByCustodianCode();

        // Act & Assert
        const string fieldName = nameof(LocalAuthorityDetails.Consortium);
        foreach (var (code, consortium) in expectedConsortiumsByCustodianCode)
        {
            using var _ = LocalAuthorityPropertyAssertionScope(fieldName, code);
            var localAuthority = LocalAuthorityDetailsByCustodianCode[code];
            localAuthority.Consortium.Should().Be(consortium);
        }

        LocalAuthorityDetailsByCustodianCode.Should().HaveSameCount(expectedConsortiumsByCustodianCode);
    }

    private static AssertionScope LocalAuthorityPropertyAssertionScope(string fieldName, string code)
    {
        return new AssertionScope($"{fieldName} for local authority with code \"{code}\"");
    }
}
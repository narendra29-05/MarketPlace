using Findly.Infrastructure.Security;

namespace Findly.UnitTests.Security;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_then_verify_succeeds()
    {
        var hash = _hasher.Hash("Sup3rSecret!");

        Assert.True(_hasher.Verify("Sup3rSecret!", hash));
    }

    [Fact]
    public void Verify_wrong_password_fails()
    {
        var hash = _hasher.Hash("Sup3rSecret!");

        Assert.False(_hasher.Verify("WrongPass1!", hash));
    }

    [Theory]
    [InlineData("")]
    [InlineData("garbage")]
    [InlineData("100000.not-base64.also-not")]
    [InlineData("abc.RmluZGx5U2VlZFNhbHQxNg==.JtbHMFn40N9ehO2O0B7SQl/peqlcWidoeeeKD3zZaKc=")]
    public void Verify_malformed_stored_hash_returns_false(string stored)
    {
        Assert.False(_hasher.Verify("anything", stored));
    }

    [Fact]
    public void Hash_uses_random_salt_each_time()
    {
        Assert.NotEqual(_hasher.Hash("same-password"), _hasher.Hash("same-password"));
    }

    [Fact]
    public void Seeded_admin_hash_verifies_seed_password()
    {
        // Exact string from db/Findly.Migrations/Scripts/fin/Tables/009_SeedAdminUser.sql —
        // guards against hasher parameter drift breaking the seeded admin login.
        const string seededHash = "100000.RmluZGx5U2VlZFNhbHQxNg==.JtbHMFn40N9ehO2O0B7SQl/peqlcWidoeeeKD3zZaKc=";

        Assert.True(_hasher.Verify("Admin@123!", seededHash));
    }
}

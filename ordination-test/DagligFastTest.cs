using shared.Model;

namespace ordination_test;

[TestClass]
public class DagligFastTest
{
    private static Laegemiddel _lm = new();
    private DagligFast _df = new(new DateTime(2030, 6, 12), new DateTime(2030, 6, 13), _lm, 2, 2, 2, 2);

    [TestMethod]
    public void DagligFastCreated()
    {
        // Ensure the start date matches the expected value
        Assert.AreEqual(new DateTime(2030, 6, 12), _df.startDen);

        // Ensure other properties have expected values
        Assert.AreEqual(2, _df.MorgenDosis.antal);
        Assert.AreEqual(2, _df.MiddagDosis.antal);
        Assert.AreEqual(2, _df.AftenDosis.antal);
        Assert.AreEqual(2, _df.NatDosis.antal);
    }

    [TestMethod]
    public void SamletDosisTest()
    {
        // Ensure samletDosis returns the sum of all doses for the entire period
        Assert.AreEqual(16, _df.samletDosis());
        Assert.AreNotEqual(17, _df.samletDosis());
    }

    [TestMethod]
    public void DoegnDosisTest()
    {
        // Ensure doegnDosis returns the correct daily dose
        Assert.AreEqual(8, _df.doegnDosis());
        Assert.AreNotEqual(9, _df.doegnDosis());
    }

    [TestMethod]
    public void GetDoserTest()
    {
        // Test that getDoser returns the correct doser array
        var doser = _df.getDoser();

        Assert.AreEqual(2, doser[0].antal);
        Assert.AreEqual(2, doser[1].antal);
        Assert.AreEqual(2, doser[2].antal);
        Assert.AreEqual(2, doser[3].antal);
    }

    [TestMethod]
    public void GetTypeTest()
    {
        // Ensure getType returns correct type identifier
        Assert.AreEqual("DagligFast", _df.getType());
    }
}
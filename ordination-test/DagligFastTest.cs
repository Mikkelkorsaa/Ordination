using shared.Model;

namespace ordination_test;

[TestClass]
public class DagligFastTest
{
    private static Laegemiddel _lm = new Laegemiddel();
     private DagligFast _df = new DagligFast(new DateTime(2030, 6, 12), new DateTime(2030, 6, 13), _lm, 2, 2, 2, 2);

    [TestMethod]
    public void DagligFastCreated()
    {
        Assert.AreEqual("12/06/2030 00:00:00", _df.ToString());
        Assert.AreNotEqual("12/07/2030 00:00:00", _df.ToString());

        Assert.AreEqual(2, _df.MorgenDosis);
        Assert.AreNotEqual(1, _df.MorgenDosis);

        Assert.AreEqual(2, _df.MiddagDosis);
        Assert.AreNotEqual(1, _df.MiddagDosis);
    }

    [TestMethod]
    public void SamletDosisTest()
    {
        Assert.AreEqual(16, _df.samletDosis());
        Assert.AreNotEqual(15, _df.samletDosis());
    }

    [TestMethod]
    public void DoegnDosisTest()
    {
        Assert.AreEqual(16, _df.doegnDosis());
        Assert.AreNotEqual(15, _df.doegnDosis());
    }

    [TestMethod]
    public void GetDoserTest()
    {
        var doser = _df.getDoser();
        
        Assert.AreEqual(2, doser[0]);
        Assert.AreEqual(2, doser[1]);
        Assert.AreEqual(2, doser[2]);
        Assert.AreEqual(2, doser[3]);
    }

    [TestMethod]
    public void GetTypeTest()
    {
        Assert.AreEqual("DagligFast", _df.ToString());
    }
}
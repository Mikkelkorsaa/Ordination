using shared.Model;

namespace ordination_test;

[TestClass]
public class DagligFastTest
{
    [TestMethod]
    public void DagligFastCreated()
    {
        Laegemiddel LM = new Laegemiddel();
        DagligFast dagligFast = new DagligFast(new DateTime(2030, 6, 12), new DateTime(2030, 6, 15), LM, 2, 2, 2, 2);

        Assert.AreEqual("12/06/2030 00:00:00", dagligFast.ToString());
        Assert.AreNotEqual("12/07/2030 00:00:00", dagligFast.ToString());

        Assert.AreEqual(2, dagligFast.MorgenDosis);
        Assert.AreNotEqual(1, dagligFast.MorgenDosis);

        Assert.AreEqual(2, dagligFast.MiddagDosis);
        Assert.AreNotEqual(1, dagligFast.MiddagDosis);
    }

    [TestMethod]
    public void SamletDosisTest()
    {
        Laegemiddel LM = new Laegemiddel();
        DagligFast dagligFast = new DagligFast(
            new DateTime(2030, 6, 12),
            new DateTime(2030, 6, 13),
            LM, 2, 2, 2, 2);
        
        Assert.AreEqual(16, dagligFast.samletDosis());
        Assert.AreNotEqual(15, dagligFast.samletDosis());
    }
}
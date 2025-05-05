using shared.Model;
namespace ordination_test;

[TestClass]
public class DagligSkævTest
{
    private static Laegemiddel _lm = new();
    private DagligSkæv _ds = new (new DateTime(2030, 06, 15), new DateTime(2030, 06, 17), _lm);
    
    [TestMethod]
    public void DagligSkævCreatedTest()
    {
        var emptyDagligSkæv = new DagligSkæv();
        Assert.AreNotEqual(emptyDagligSkæv, _ds);
        Assert.IsNotNull(_ds);
    }

    [TestMethod]
    public void OpretDosisTest()
    {
        _ds.doser.Add(new Dosis(new DateTime(2030, 06, 15, 10, 0 ,0), 5));
        Assert.AreEqual(5, _ds.doser[0]);
        Assert.AreNotEqual(4, _ds.doser[0]);
    }

    [TestMethod]
    public void DoegnDosisTest()
    {
        _ds.doser.Add(new Dosis(new DateTime(2030, 06, 15, 10, 0 ,0), 3));
        _ds.doser.Add(new Dosis(new DateTime(2030, 06, 15, 17, 0 ,0), 1));
        _ds.doser.Add(new Dosis(new DateTime(2030, 06, 15, 20, 0 ,0), 2));
        
        Assert.AreEqual(6.00, _ds.doegnDosis());
        Assert.AreNotEqual(5.00, _ds.doegnDosis());
    }

    [TestMethod]
    public void SamletDosisTest()
    {
        _ds.doser.Add(new Dosis(new DateTime(2030, 06, 15, 10, 0 ,0), 3));
        _ds.doser.Add(new Dosis(new DateTime(2030, 06, 15, 17, 0 ,0), 1));
        _ds.doser.Add(new Dosis(new DateTime(2030, 06, 15, 20, 0 ,0), 2));
        
        Assert.AreEqual(18.00, _ds.samletDosis());
        Assert.AreNotEqual(17.00, _ds.samletDosis());
    }

    [TestMethod]
    public void GetTypeTest()
    {
        Assert.AreEqual("DagligSkæv", _ds.GetType());
    }
}
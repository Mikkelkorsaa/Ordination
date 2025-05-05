using shared.Model;

namespace ordination_test;

[TestClass]
public class PNTest
{
    private static Laegemiddel _lm = new Laegemiddel();
    private PN _pn = new PN(new DateTime(2030, 6, 1), new DateTime(2030, 6, 10), 2.0, _lm);

    [TestMethod]
    public void PNCreatedTest()
    {
        Assert.AreEqual(2.0, _pn.antalEnheder);
    }

    [TestMethod]
    public void GivDosisValidDateTest()
    {
        var dato = new Dato { dato = new DateTime(2030, 6, 5) };
        bool result = _pn.givDosis(dato);

        Assert.IsTrue(result);
        Assert.AreEqual(1, _pn.getAntalGangeGivet());
        Assert.AreEqual(dato, _pn.dates[0]);
    }

    [TestMethod]
    public void GivDosisInvalidDateTest()
    {
        var beforeStart = new Dato { dato = new DateTime(2030, 5, 31) };
        var afterEnd = new Dato { dato = new DateTime(2030, 6, 11) };

        bool resultBefore = _pn.givDosis(beforeStart);
        bool resultAfter = _pn.givDosis(afterEnd);

        Assert.IsFalse(resultBefore);
        Assert.IsFalse(resultAfter);
        Assert.AreEqual(0, _pn.getAntalGangeGivet());
    }

    [TestMethod]
    public void SamletDosisTest()
    {
        _pn.givDosis(new Dato { dato = new DateTime(2030, 6, 2) });
        _pn.givDosis(new Dato { dato = new DateTime(2030, 6, 3) });

        Assert.AreEqual(4.0, _pn.samletDosis());
        Assert.AreNotEqual(6.0, _pn.samletDosis());
    }

    [TestMethod]
    public void DoegnDosisTest()
    {
        _pn = new PN(new DateTime(2030, 6, 1), new DateTime(2030, 6, 10), 2.0, _lm);
        _pn.givDosis(new Dato { dato = new DateTime(2030, 6, 2) });
        _pn.givDosis(new Dato { dato = new DateTime(2030, 6, 4) });

        // 2 doser på 2. og 4. => (2 + 2) / (4 - 2 + 1) = 4 / 3
        Assert.AreEqual(4.0 / 3.0, _pn.doegnDosis(), 0.0001);
    }

    [TestMethod]
    public void DoegnDosisNoDosesTest()
    {
        Assert.AreEqual(0.0, _pn.doegnDosis());
    }

    [TestMethod]
    public void GetAntalGangeGivetTest()
    {
        _pn = new PN(new DateTime(2030, 6, 1), new DateTime(2030, 6, 10), 2.0, _lm);
        _pn.givDosis(new Dato { dato = new DateTime(2030, 6, 3) });
        _pn.givDosis(new Dato { dato = new DateTime(2030, 6, 5) });

        Assert.AreEqual(2, _pn.getAntalGangeGivet());
    }

    [TestMethod]
    public void GetTypeTest()
    {
        Assert.AreEqual("PN", _pn.getType());
    }
}

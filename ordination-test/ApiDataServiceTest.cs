using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using shared.Model;
using Service;
using Data;
using static shared.Util;

namespace ordination_test;

[TestClass]
public class DataServiceTest
{
    private DataService _service;
    private OrdinationContext _context;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<OrdinationContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new OrdinationContext(options);
        _context.Database.EnsureCreated();
        _service = new DataService(_context);

        _service.SeedData();
    }

    [TestMethod]
    public void GetPatienterTest()
    {
        var patienter = _service.GetPatienter();
        Assert.IsTrue(patienter.Count > 0);
    }

    [TestMethod]
    public void GetLaegemidlerTest()
    {
        var lm = _service.GetLaegemidler();
        Assert.IsTrue(lm.Count > 0);
    }

    
    //OpretPN
    [TestMethod]
    public void OpretPNValidTest()
    {
        var patient = _service.GetPatienter().First();
        var lm = _service.GetLaegemidler().First();

        var pn = _service.OpretPN(patient.PatientId, lm.LaegemiddelId, 2.5, DateTime.Today, DateTime.Today.AddDays(3));

        Assert.IsNotNull(pn);
        Assert.AreEqual(2.5, pn.antalEnheder);
        Assert.AreEqual(lm.LaegemiddelId, pn.laegemiddel.LaegemiddelId);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void OpretPNInvalidPatientIdThrows()
    {
        var lm = _service.GetLaegemidler().First();
        _service.OpretPN(-1, lm.LaegemiddelId, 1.0, DateTime.Today, DateTime.Today.AddDays(1));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void OpretPNInvalidDatesThrows()
    {
        var patient = _service.GetPatienter().First();
        var lm = _service.GetLaegemidler().First();
        _service.OpretPN(patient.PatientId, lm.LaegemiddelId, 1.0, DateTime.Today.AddDays(5), DateTime.Today);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void OpretPN_AntalIsZero_Throws()
    {
        var patient = _service.GetPatienter().First();
        var laegemiddel = _service.GetLaegemidler().First();
        _service.OpretPN(patient.PatientId, laegemiddel.LaegemiddelId, 0, DateTime.Today, DateTime.Today.AddDays(1));
    }
    
    //OpretDagligFast
    [TestMethod]
    public void OpretDagligFast_ValidMinimalDoses_ShouldCreate()
    {
        var patient = _service.GetPatienter().First();
        var laegemiddel = _service.GetLaegemidler().First();

        var dagligFast = _service.OpretDagligFast(
            patient.PatientId, laegemiddel.LaegemiddelId,
            antalMorgen: 0.0001,
            antalMiddag: 0,
            antalAften: 0,
            antalNat: 0,
            startDato: DateTime.Today,
            slutDato: DateTime.Today.AddDays(1)
        );

        Assert.IsNotNull(dagligFast);
        Assert.AreEqual(0.0001, dagligFast.MorgenDosis);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void OpretDagligFast_InvalidPatientId_Throws()
    {
        var laegemiddel = _service.GetLaegemidler().First();
        _service.OpretDagligFast(-1, laegemiddel.LaegemiddelId, 1, 1, 1, 1, DateTime.Today, DateTime.Today.AddDays(1));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void OpretDagligFast_AllDosesZero_Throws()
    {
        var patient = _service.GetPatienter().First();
        var laegemiddel = _service.GetLaegemidler().First();
        _service.OpretDagligFast(patient.PatientId, laegemiddel.LaegemiddelId, 0, 0, 0, 0, DateTime.Today, DateTime.Today.AddDays(1));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void OpretDagligFast_NegativeDose_Throws()
    {
        var patient = _service.GetPatienter().First();
        var laegemiddel = _service.GetLaegemidler().First();
        _service.OpretDagligFast(patient.PatientId, laegemiddel.LaegemiddelId, -1, 1, 1, 1, DateTime.Today, DateTime.Today.AddDays(1));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void OpretDagligFast_SameStartAndEndDate_Throws()
    {
        var patient = _service.GetPatienter().First();
        var laegemiddel = _service.GetLaegemidler().First();
        var date = DateTime.Today;
        _service.OpretDagligFast(patient.PatientId, laegemiddel.LaegemiddelId, 1, 1, 1, 1, date, date);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void OpretDagligFast_StartDateAfterEndDate_Throws()
    {
        var patient = _service.GetPatienter().First();
        var laegemiddel = _service.GetLaegemidler().First();
        _service.OpretDagligFast(patient.PatientId, laegemiddel.LaegemiddelId, 1, 1, 1, 1, DateTime.Today.AddDays(2), DateTime.Today);
    }

    
    //AnvendOrdination
    [TestMethod]
    public void AnvendOrdination_ValidPNWithinRange_ReturnsSuccess()
    {
        var pn = _service.GetPNs().First();
        var dato = new Dato { dato = pn.startDen.AddDays(1) };

        var result = _service.AnvendOrdination(pn.OrdinationId, dato);

        Assert.AreEqual("PN ordination anvendt", result);
        Assert.IsTrue(pn.dates.Any(d => d.dato == dato.dato));
    }


    [TestMethod]
    public void AnvendOrdination_InvalidOrdinationId_ReturnsNotFound()
    {
        var result = _service.AnvendOrdination(-1, new Dato { dato = DateTime.Today });

        Assert.AreEqual("Ordination ikke fundet", result);
    }

    [TestMethod]
    public void AnvendOrdination_DateBeforeStart_ReturnsOutOfRange()
    {
        var pn = _service.GetPNs().First();
        var dato = new Dato { dato = pn.startDen.AddDays(-1) };

        var result = _service.AnvendOrdination(pn.OrdinationId, dato);

        Assert.AreEqual("Dato er ikke indenfor ordinationens gyldighedsperiode", result);
    }

    [TestMethod]
    public void AnvendOrdination_DateAfterEnd_ReturnsOutOfRange()
    {
        var pn = _service.GetPNs().First();
        var dato = new Dato { dato = pn.slutDen.AddDays(1) };

        var result = _service.AnvendOrdination(pn.OrdinationId, dato);

        Assert.AreEqual("Dato er ikke indenfor ordinationens gyldighedsperiode", result);
    }
    
    [TestMethod]
    public void AnvendOrdination_NotPNOrdination_ReturnsCouldNotApply()
    {
        var df = _service.GetDagligFaste().First();
        var dato = new Dato { dato = df.startDen.AddDays(1) };

        var result = _service.AnvendOrdination(df.OrdinationId, dato);

        Assert.AreEqual("Ordination kunne ikke anvendes", result);
    }



    [TestMethod]
    public void GetAnbefaletDosisPerDøgn_NormalVaegt_Test()
    {
        var patient = _service.GetPatienter().First();
        var laegemiddel = _service.GetLaegemidler().First();

        var dosis = _service.GetAnbefaletDosisPerDøgn(patient.PatientId, laegemiddel.LaegemiddelId);
        var forventet = patient.vaegt * laegemiddel.enhedPrKgPrDoegnNormal;

        Assert.AreEqual(forventet, dosis, 0.001);
    }
    
    [TestMethod]
    public void GetAnbefaletDosis_VaegtUnder25_UsesLet()
    {
        var laegemiddel = _service.GetLaegemidler().First();
        var patient = new Patient("000000-0000", "Let Patient", 24.9);
        _context.Patienter.Add(patient);
        _context.SaveChanges();

        double expected = patient.vaegt * laegemiddel.enhedPrKgPrDoegnLet;
        double actual = _service.GetAnbefaletDosisPerDøgn(patient.PatientId, laegemiddel.LaegemiddelId);

        Assert.AreEqual(expected, actual, 0.0001);
    }
    
    [TestMethod]
    public void GetAnbefaletDosis_VaegtExactly25_UsesNormal()
    {
        var laegemiddel = _service.GetLaegemidler().First();
        var patient = new Patient("000001-0001", "Normal Patient", 25.0);
        _context.Patienter.Add(patient);
        _context.SaveChanges();

        double expected = patient.vaegt * laegemiddel.enhedPrKgPrDoegnNormal;
        double actual = _service.GetAnbefaletDosisPerDøgn(patient.PatientId, laegemiddel.LaegemiddelId);

        Assert.AreEqual(expected, actual, 0.0001);
    }
    
    [TestMethod]
    public void GetAnbefaletDosis_VaegtExactly120_UsesNormal()
    {
        var laegemiddel = _service.GetLaegemidler().First();
        var patient = new Patient("000002-0002", "Max Normal Patient", 120.0);
        _context.Patienter.Add(patient);
        _context.SaveChanges();

        double expected = patient.vaegt * laegemiddel.enhedPrKgPrDoegnNormal;
        double actual = _service.GetAnbefaletDosisPerDøgn(patient.PatientId, laegemiddel.LaegemiddelId);

        Assert.AreEqual(expected, actual, 0.0001);
    }
    
    [TestMethod]
    public void GetAnbefaletDosis_VaegtOver120_UsesTung()
    {
        var laegemiddel = _service.GetLaegemidler().First();
        var patient = new Patient("000003-0003", "Tung Patient", 121.0);
        _context.Patienter.Add(patient);
        _context.SaveChanges();

        double expected = patient.vaegt * laegemiddel.enhedPrKgPrDoegnTung;
        double actual = _service.GetAnbefaletDosisPerDøgn(patient.PatientId, laegemiddel.LaegemiddelId);

        Assert.AreEqual(expected, actual, 0.0001);
    }

    [TestMethod]
    public void GetAnbefaletDosis_InvalidPatientId_ReturnsMinusOne()
    {
        var laegemiddel = _service.GetLaegemidler().First();

        double actual = _service.GetAnbefaletDosisPerDøgn(-1, laegemiddel.LaegemiddelId);

        Assert.AreEqual(-1, actual);
    }
    
    [TestMethod]
    public void GetAnbefaletDosis_InvalidLaegemiddelId_ReturnsMinusOne()
    {
        var patient = _service.GetPatienter().First();

        double actual = _service.GetAnbefaletDosisPerDøgn(patient.PatientId, -1);

        Assert.AreEqual(-1, actual);
    }



    [TestMethod]
    public void GetDagligFastTest()
    {
        var faste = _service.GetDagligFaste();
        Assert.IsTrue(faste.Count > 0);
    }

    [TestMethod]
    public void GetDagligSkaevTest()
    {
        var skaev = _service.GetDagligSkæve();
        Assert.IsTrue(skaev.Count > 0);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

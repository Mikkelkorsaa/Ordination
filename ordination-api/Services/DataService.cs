using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using shared.Model;
using static shared.Util;
using Data;

namespace Service;

public class DataService
{
    private OrdinationContext db { get; }

    public DataService(OrdinationContext db) {
        this.db = db;
    }

    /// <summary>
    /// Seeder noget nyt data i databasen, hvis det er nødvendigt.
    /// </summary>
    public void SeedData() {

        // Patients
        Patient[] patients = new Patient[5];
        patients[0] = db.Patienter.FirstOrDefault()!;

        if (patients[0] == null)
        {
            patients[0] = new Patient("121256-0512", "Jane Jensen", 63.4);
            patients[1] = new Patient("070985-1153", "Finn Madsen", 83.2);
            patients[2] = new Patient("050972-1233", "Hans Jørgensen", 89.4);
            patients[3] = new Patient("011064-1522", "Ulla Nielsen", 59.9);
            patients[4] = new Patient("123456-1234", "Ib Hansen", 87.7);

            db.Patienter.Add(patients[0]);
            db.Patienter.Add(patients[1]);
            db.Patienter.Add(patients[2]);
            db.Patienter.Add(patients[3]);
            db.Patienter.Add(patients[4]);
            db.SaveChanges();
        }

        Laegemiddel[] laegemiddler = new Laegemiddel[5];
        laegemiddler[0] = db.Laegemiddler.FirstOrDefault()!;
        if (laegemiddler[0] == null)
        {
            laegemiddler[0] = new Laegemiddel("Acetylsalicylsyre", 0.1, 0.15, 0.16, "Styk");
            laegemiddler[1] = new Laegemiddel("Paracetamol", 1, 1.5, 2, "Ml");
            laegemiddler[2] = new Laegemiddel("Fucidin", 0.025, 0.025, 0.025, "Styk");
            laegemiddler[3] = new Laegemiddel("Methotrexat", 0.01, 0.015, 0.02, "Styk");
            laegemiddler[4] = new Laegemiddel("Prednisolon", 0.1, 0.15, 0.2, "Styk");

            db.Laegemiddler.Add(laegemiddler[0]);
            db.Laegemiddler.Add(laegemiddler[1]);
            db.Laegemiddler.Add(laegemiddler[2]);
            db.Laegemiddler.Add(laegemiddler[3]);
            db.Laegemiddler.Add(laegemiddler[4]);

            db.SaveChanges();
        }

        Ordination[] ordinationer = new Ordination[6];
        ordinationer[0] = db.Ordinationer.FirstOrDefault()!;
        if (ordinationer[0] == null) {
            Laegemiddel[] lm = db.Laegemiddler.ToArray();
            Patient[] p = db.Patienter.ToArray();

            ordinationer[0] = new PN(new DateTime(2021, 1, 1), new DateTime(2021, 1, 12), 123, lm[1]);    
            ordinationer[1] = new PN(new DateTime(2021, 2, 12), new DateTime(2021, 2, 14), 3, lm[0]);    
            ordinationer[2] = new PN(new DateTime(2021, 1, 20), new DateTime(2021, 1, 25), 5, lm[2]);    
            ordinationer[3] = new PN(new DateTime(2021, 1, 1), new DateTime(2021, 1, 12), 123, lm[1]);
            ordinationer[4] = new DagligFast(new DateTime(2021, 1, 10), new DateTime(2021, 1, 12), lm[1], 2, 0, 1, 0);
            ordinationer[5] = new DagligSkæv(new DateTime(2021, 1, 23), new DateTime(2021, 1, 24), lm[2]);
            
            ((DagligSkæv) ordinationer[5]).doser = new Dosis[] { 
                new Dosis(CreateTimeOnly(12, 0, 0), 0.5),
                new Dosis(CreateTimeOnly(12, 40, 0), 1),
                new Dosis(CreateTimeOnly(16, 0, 0), 2.5),
                new Dosis(CreateTimeOnly(18, 45, 0), 3)        
            }.ToList();
            

            db.Ordinationer.Add(ordinationer[0]);
            db.Ordinationer.Add(ordinationer[1]);
            db.Ordinationer.Add(ordinationer[2]);
            db.Ordinationer.Add(ordinationer[3]);
            db.Ordinationer.Add(ordinationer[4]);
            db.Ordinationer.Add(ordinationer[5]);

            db.SaveChanges();

            p[0].ordinationer.Add(ordinationer[0]);
            p[0].ordinationer.Add(ordinationer[1]);
            p[2].ordinationer.Add(ordinationer[2]);
            p[3].ordinationer.Add(ordinationer[3]);
            p[1].ordinationer.Add(ordinationer[4]);
            p[1].ordinationer.Add(ordinationer[5]);

            db.SaveChanges();
        }
    }

    
    public List<PN> GetPNs() {
        return db.PNs.Include(o => o.laegemiddel).Include(o => o.dates).ToList();
    }

    public List<DagligFast> GetDagligFaste() {
        return db.DagligFaste
            .Include(o => o.laegemiddel)
            .Include(o => o.MorgenDosis)
            .Include(o => o.MiddagDosis)
            .Include(o => o.AftenDosis)            
            .Include(o => o.NatDosis)            
            .ToList();
    }

    public List<DagligSkæv> GetDagligSkæve() {
        return db.DagligSkæve
            .Include(o => o.laegemiddel)
            .Include(o => o.doser)
            .ToList();
    }

    public List<Patient> GetPatienter() {
        return db.Patienter.Include(p => p.ordinationer).ToList();
    }

    public List<Laegemiddel> GetLaegemidler() {
        return db.Laegemiddler.ToList();
    }

    public PN OpretPN(int patientId, int laegemiddelId, double antal, DateTime startDato, DateTime slutDato) {
        if (patientId <= 0 || laegemiddelId <= 0)
            throw new ArgumentException("Patient ID and Laegemiddel ID must be greater than 0");
        
        if (antal <= 0)
            throw new ArgumentException("Antal must be greater than 0");
            
        if (startDato >= slutDato)
            throw new ArgumentException("Start date must be before end date");
        
        var patient = db.Patienter.Find(patientId);
        if (patient == null)
            throw new ArgumentException("Patient not found");
            
        var laegemiddel = db.Laegemiddler.Find(laegemiddelId);
        if (laegemiddel == null)
            throw new ArgumentException("Laegemiddel not found");
            
        var pn = new PN(startDato, slutDato, antal, laegemiddel);
        patient.ordinationer.Add(pn);
        db.PNs.Add(pn);
        db.SaveChanges();
        return pn;
    }

    public DagligFast OpretDagligFast(int patientId, int laegemiddelId, 
        double antalMorgen, double antalMiddag, double antalAften, double antalNat, 
        DateTime startDato, DateTime slutDato) {
        
        if (patientId <= 0 || laegemiddelId <= 0)
            throw new ArgumentException("Patient ID and Laegemiddel ID must be greater than 0");
            
        if (startDato >= slutDato)
            throw new ArgumentException("Start date must be before end date");
    
        if (antalMorgen < 0 || antalMiddag < 0 || antalAften < 0 || antalNat < 0)
            throw new ArgumentException("Antal values cannot be negative");
        
        if (antalMorgen + antalMiddag + antalAften + antalNat == 0)
            throw new ArgumentException("Antal values cannot be 0");
    
        var patient = db.Patienter.Find(patientId);
        if (patient == null)
            throw new ArgumentException("Patient not found");
            
        var laegemiddel = db.Laegemiddler.Find(laegemiddelId);
        if (laegemiddel == null)
            throw new ArgumentException("Laegemiddel not found");
    
        var dagligFast = new DagligFast(startDato, slutDato, laegemiddel, antalMorgen, antalMiddag, antalAften, antalNat);
        patient.ordinationer.Add(dagligFast);
        db.DagligFaste.Add(dagligFast);
        db.SaveChanges();
        return dagligFast;
    }

    public DagligSkæv OpretDagligSkaev(int patientId, int laegemiddelId, Dosis[] doser, DateTime startDato, DateTime slutDato) {
        var patient = db.Patienter.Find(patientId);
        var laegemiddel = db.Laegemiddler.Find(laegemiddelId);
        var dagligSkaev = new DagligSkæv(startDato, slutDato, laegemiddel, doser);
        patient.ordinationer.Add(dagligSkaev);
        db.DagligSkæve.Add(dagligSkaev);
        db.SaveChanges();
        return dagligSkaev;
    }

    public string AnvendOrdination(int id, Dato dato) {
        var ordination = db.Ordinationer.Find(id);
        if (ordination == null) {
            return "Ordination ikke fundet";
        }

        if (dato.dato < ordination.startDen || dato.dato > ordination.slutDen) {
            return "Dato er ikke indenfor ordinationens gyldighedsperiode";
        }

        if (ordination is PN pn) {
            pn.dates.Add(dato);
            db.SaveChanges();
            return "PN ordination anvendt";
        }

        return "Ordination kunne ikke anvendes";
    }

    /// <summary>
    /// Den anbefalede dosis for den pågældende patient, per døgn, hvor der skal tages hensyn til
	/// patientens vægt. Enheden afhænger af lægemidlet. Patient og lægemiddel må ikke være null.
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="laegemiddel"></param>
    /// <returns></returns>
	public double GetAnbefaletDosisPerDøgn(int patientId, int laegemiddelId) {
        var patient = db.Patienter.Find(patientId);
        var laegemiddel = db.Laegemiddler.Find(laegemiddelId);
        
        if(patient == null || laegemiddel == null) {
            return -1;
        }

        if(patient.vaegt < 25) {
            return patient.vaegt * laegemiddel.enhedPrKgPrDoegnLet;
        }
        else if(patient.vaegt <= 120) {
            return patient.vaegt * laegemiddel.enhedPrKgPrDoegnNormal;
        }
        else {
            return patient.vaegt * laegemiddel.enhedPrKgPrDoegnTung;
        }
	}
}
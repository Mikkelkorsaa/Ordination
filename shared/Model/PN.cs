namespace shared.Model;

public class PN : Ordination {
	public double antalEnheder { get; set; }
    public List<Dato> dates { get; set; } = new List<Dato>();

    public PN (DateTime startDen, DateTime slutDen, double antalEnheder, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
		this.antalEnheder = antalEnheder;
	}

    public PN() : base(null!, new DateTime(), new DateTime()) {
    }

    /// <summary>
    /// Registrerer at der er givet en dosis på dagen givesDen
    /// Returnerer true hvis givesDen er inden for ordinationens gyldighedsperiode og datoen huskes
    /// Returner false ellers og datoen givesDen ignoreres
    /// </summary>
    public bool givDosis(Dato givesDen)
    {
	    if (startDen < givesDen.dato && givesDen.dato < slutDen)
	    {
		    dates.Add(givesDen);
		    return true;
	    }
	    return false;
    }

    public override double doegnDosis()
    {
	    if (!dates.Any()) return 0;
	    if (dates.Count == 1) return antalEnheder;

	    // Compute Max and Min directly on the 'dato' property of 'Dato' objects
	    DateTime start = dates.Min(d => d.dato);
	    DateTime end = dates.Max(d => d.dato);

	    // Divide samletDosis by the span (total days), adding one to avoid division by zero
	    return samletDosis() / ((end - start).Days + 1);
    }
    
    public override double samletDosis() {
        return dates.Count() * antalEnheder;
    }

    public int getAntalGangeGivet() {
        return dates.Count();
    }

	public override String getType() {
		return "PN";
	}
}

namespace FlightPanelApp.UseCases;

public class Flight
{
    public TimeOnly StandardTimeDeparture { get; set; }

    public string Destination { get; set; } = string.Empty;

    public string Airline { get; set; } = string.Empty;

    public int Number { get; set; }

    public int Gate { get; set; }

    public TimeOnly EstimatedTimeDeparture { get; set; }

    public string Remarks { get; set; } = string.Empty;
}

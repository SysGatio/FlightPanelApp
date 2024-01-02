using System.Timers;

namespace FlightPanelApp.UseCases;

public class FlightPanel : IDisposable
{
    public DateTime LocalTime { get; set; } = DateTime.Now;

    public List<Flight> Flights { get; set; } = [];

    private System.Timers.Timer? _timer;
    private System.Timers.Timer? _timerLocalTime;
    private const int NumberOfFlights = 12;

    public FlightPanel()
    {
        AddInitialFlights();
        StartLocalTimeTimer();
        StartFlightPanelTimer();
    }

    private void AddInitialFlights()
    {
        for (int i = 1; i <= NumberOfFlights; i++)
        {
            Flights.Add(CreateFlight());
        }
    }

    private void StartLocalTimeTimer()
    {
        _timerLocalTime = new System.Timers.Timer(TimeSpan.FromSeconds(1));
        _timerLocalTime.Elapsed += UpdateLocalTime!;
        _timerLocalTime.Start();
    }

    private void StartFlightPanelTimer()
    {
        _timer = new System.Timers.Timer(TimeSpan.FromSeconds(60));
        _timer.Elapsed += UpdateFlightPanel!;
        _timer.Start();
    }

    private void UpdateLocalTime(Object source, ElapsedEventArgs e)
    {
        LocalTime = DateTime.Now;
    }

    private void UpdateFlightPanel(Object source, ElapsedEventArgs e)
    {
        var flights = Flights.Where(f => f.EstimatedTimeDeparture > TimeOnly.FromTimeSpan(LocalTime.TimeOfDay)).ToList();

        for (int i = flights.Count; i <= NumberOfFlights; i++)
        {
            flights.Add(CreateFlight());
        }

        foreach (var flight in flights)
        {
            flight.Remarks = GetRemarks(flight.EstimatedTimeDeparture);
        }

        Flights = flights;
    }

    private Flight CreateFlight()
    {
        var nextTimeDeparture = GetNextTimeDeparture();

        return new Flight()
        {
            StandardTimeDeparture = nextTimeDeparture,
            Destination = GetDestination(),
            Airline = GetAirline(),
            Number = GetFlightNumber(),
            Gate = GetGateNumber(),
            EstimatedTimeDeparture = nextTimeDeparture,
            Remarks = GetRemarks(nextTimeDeparture)
        };
    }

    private TimeOnly GetNextTimeDeparture()
    {
        if (Flights.Count == 0)
        {
            int minutes = DateTime.Now.Minute / 5;
            return new TimeOnly(DateTime.Now.Hour, minutes * 5, 0).AddMinutes(10);
        }

        var minutesToAdd = new Random().Next(1, 5) * 5;
        var lastTimeDeparture = Flights.LastOrDefault()!.StandardTimeDeparture;
        return lastTimeDeparture.AddMinutes(minutesToAdd);
    }

    private string GetDestination()
    {
        string[] destinations = ["Aracaju", "Brasília", "Porto Alegre", "Belo Horizonte", "Campinas", "Rio de Janeiro", "Manaus", "Salvador", "Goiânia", "Curitiba", "Cuiabá", "São José do Rio Preto", "Fortaleza"];
        return destinations[new Random().Next(0, destinations.Length)];
    }

    private string GetAirline()
    {
        string[] airlines = ["Gol", "LATAM", "Azul", "Avianca"];
        return airlines[new Random().Next(0, airlines.Length)];
    }

    private int GetFlightNumber()
    {
        var exitingFlights = Flights.Select(f => f.Number).ToList();

        while (true)
        {
            var candidate = new Random().Next(1100, 7500);
            if (exitingFlights.Contains(candidate) is false)
                return candidate;
        }
    }

    private int GetGateNumber()
    {
        var occupedGates = Flights.Select(f => f.Gate).ToList();

        while (true)
        {
            var candidate = new Random().Next(10, 45);
            if (occupedGates.Contains(candidate) is false)
                return candidate;
        }
    }

    private static string GetRemarks(TimeOnly standardTimeDeparture)
    {
        var currentTime = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);

        if (standardTimeDeparture < currentTime.AddMinutes(10))
            return "Taking Off";

        if (standardTimeDeparture < currentTime.AddMinutes(20))
            return "Now Boarding";

        if (standardTimeDeparture < currentTime.AddMinutes(30))
            return "Procced to Gate";

        if (standardTimeDeparture < currentTime.AddMinutes(60))
            return "Check-in Closed";

        if (standardTimeDeparture < currentTime.AddMinutes(120))
            return "Check-in Open";

        if (standardTimeDeparture < currentTime.AddMinutes(180))
            return "Confirmed";

        return "Estimated";
    }

    public void Dispose()
    {
        _timer!.Stop();
        _timerLocalTime!.Stop();

        _timer.Dispose();
        _timerLocalTime.Dispose();

        GC.SuppressFinalize(this);
    }
}

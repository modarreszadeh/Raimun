namespace Web.Models.ViewModels
{
    public class WeatherViewModel
    {
        public Location Location { get; set; }
        public Current Current { get; set; }
    }

    public class Current
    {
        public float Temp_C { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
    }
}
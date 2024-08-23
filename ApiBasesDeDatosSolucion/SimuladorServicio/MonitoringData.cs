namespace SimuladorServicio
{
    public class MonitoringData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PaisOrigen { get; set; }
        public string PaisDestino { get; set; }
        public string ClienteOrigen { get; set; }
        public string ClienteDestino { get; set; }
        public double ValorOrigen { get; set; }
        public double ValorDestino { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

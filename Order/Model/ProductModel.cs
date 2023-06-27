using Newtonsoft.Json;

public class ProductModel
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; }

    [JsonProperty("product")]
    public string ProductName { get; set; }

    [JsonProperty("price")]
    public float Price { get; set; }
}
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderModel
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("productId")]
    public string ProductId { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }
}

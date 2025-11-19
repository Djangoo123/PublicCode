
using System.Net.Http.Json;

var data = new {
    Ciphertext = new[]{ "ENC_1", "ENC_2"},
    Proof = "VALID_PROOF"
};

var http = new HttpClient();
var resp = await http.PostAsJsonAsync("http://localhost:5000/compute/sum", data);
Console.WriteLine(await resp.Content.ReadAsStringAsync());

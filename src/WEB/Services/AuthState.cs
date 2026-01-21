using Microsoft.JSInterop;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using System.Text;

namespace WEB.Services;

public class AuthState
{
 private readonly IJSRuntime _js;

 public bool IsAuthenticated { get; private set; }
 public string? UserType { get; private set; }

 public event Action? AuthChanged;

 public AuthState(IJSRuntime js)
 {
 _js = js;
 }

 public async Task InitializeAsync()
 {
 var token = await _js.InvokeAsync<string?>("localStorage.getItem", "jwt");
 var userType = await _js.InvokeAsync<string?>("localStorage.getItem", "userType");

 bool valid = !string.IsNullOrWhiteSpace(token) && IsTokenNotExpired(token);

 IsAuthenticated = valid;
 UserType = userType;
 }

 public async Task RefreshAsync()
 {
 await InitializeAsync();
 AuthChanged?.Invoke();
 }

 private static bool IsTokenNotExpired(string token)
 {
 try
 {
 // token format: header.payload.signature
 var parts = token.Split('.');
 if (parts.Length <2) return false;
 var payload = parts[1];
 string json = Base64UrlDecode(payload);
 using var doc = JsonDocument.Parse(json);
 if (doc.RootElement.TryGetProperty("exp", out var expElem))
 {
 long exp =0;
 if (expElem.ValueKind == JsonValueKind.Number && expElem.TryGetInt64(out exp))
 {
 var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
 return exp > now;
 }
 // some tokens may have string exp
 if (expElem.ValueKind == JsonValueKind.String && long.TryParse(expElem.GetString(), out exp))
 {
 var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
 return exp > now;
 }
 }

 // no exp claim - consider token valid
 return true;
 }
 catch
 {
 return false;
 }
 }

 private static string Base64UrlDecode(string input)
 {
 string s = input;
 s = s.Replace('-', '+').Replace('_', '/');
 switch (s.Length %4)
 {
 case 2: s += "=="; break;
 case 3: s += "="; break;
 case 0: break;
 default:
 // If modulus is1, the input is invalid base64; try to pad to make it valid
 s += "=";
 break;
 }

 byte[] bytes = Convert.FromBase64String(s);
 return Encoding.UTF8.GetString(bytes);
 }
}

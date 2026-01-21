using Microsoft.JSInterop;
using System.Threading.Tasks;

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

 IsAuthenticated = !string.IsNullOrWhiteSpace(token);
 UserType = userType;
 }

 public async Task RefreshAsync()
 {
 await InitializeAsync();
 AuthChanged?.Invoke();
 }
}

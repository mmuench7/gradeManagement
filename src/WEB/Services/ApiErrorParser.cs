using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace WEB.Services;

public static class ApiErrorParser
{
 public static async Task<string> GetFriendlyMessage(HttpResponseMessage res)
 {
 string body = string.Empty;
 try
 {
 body = await res.Content.ReadAsStringAsync();
 }
 catch
 {
 return $"Serverfehler ({(int)res.StatusCode}).";
 }

 if (string.IsNullOrWhiteSpace(body))
 return $"Serverfehler ({(int)res.StatusCode}).";

 // Try parse as JSON and extract known fields
 try
 {
 using var doc = JsonDocument.Parse(body);
 var root = doc.RootElement;

 // Handle ASP.NET ProblemDetails validation response with 'errors' object
 if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("errors", out var errorsElem) && errorsElem.ValueKind == JsonValueKind.Object)
 {
 var messages = new List<string>();
 foreach (var prop in errorsElem.EnumerateObject())
 {
 if (prop.Value.ValueKind == JsonValueKind.Array)
 {
 foreach (var item in prop.Value.EnumerateArray())
 {
 if (item.ValueKind == JsonValueKind.String)
 messages.Add(item.GetString() ?? "");
 }
 }
 }

 var distinct = messages.Where(m => !string.IsNullOrWhiteSpace(m)).Distinct().ToList();
 if (distinct.Count ==0)
 return "Ungültige Eingabe.";

 // If there are many errors, return a short summary instead of listing all
 if (distinct.Count >2)
 {
 // Show first message and a count of remaining errors
 var first = distinct.First();
 return $"Mehrere Fehler: {first} (und {distinct.Count -1} weitere). Bitte Eingaben überprüfen.";
 }

 // For1-2 messages, join them for clarity
 var joined = string.Join("; ", distinct);
 return "Ungültige Eingabe: " + joined;
 }

 string? code = null;
 string? message = null;
 string? details = null;

 if (root.ValueKind == JsonValueKind.Object)
 {
 if (root.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String)
 message = m.GetString();
 if (root.TryGetProperty("code", out var c) && c.ValueKind == JsonValueKind.String)
 code = c.GetString();
 if (root.TryGetProperty("details", out var d))
 {
 if (d.ValueKind == JsonValueKind.String)
 details = d.GetString();
 else
 details = d.ToString();
 }

 // Map known codes to German
 if (!string.IsNullOrWhiteSpace(code))
 {
 switch (code)
 {
 case "ACCOUNT_ALREADY_EXISTS":
 return "Ein Konto mit dieser E-Mail existiert bereits.";
 case "NO_DATA":
 return "E-Mail und Passwort sind erforderlich.";
 case "INVALID_CREDENTIALS":
 return "E-Mail oder Passwort sind falsch.";
 case "INVALID_EMAIL_DOMAIN":
 return "Ungültige E-Mail-Domain. Bitte nutze eine @gibz.ch-Adresse.";
 case "INVALID_JOB_CATEGORIES":
 return "Ausgewählte Berufskategorien nicht gefunden.";
 case "INVALID_COURSES":
 return "Ausgewählte Module/Fächer nicht gefunden.";
 case "COURSES_JOBCATEGORY_MISMATCH":
 return "Ein oder mehrere ausgewählte Module gehören nicht zu den gewählten Berufskategorien.";
 case "MISSING_COURSES_FOR_JOB_CATEGORIES":
 if (!string.IsNullOrWhiteSpace(details))
 return "Für einige ausgewählte Berufskategorien wurden keine Module ausgewählt: " + details;
 return "Für einige ausgewählte Berufskategorien wurden keine Module ausgewählt.";
 default:
 break;
 }
 }

 if (!string.IsNullOrWhiteSpace(message))
 {
 if ((int)res.StatusCode >=500)
 return "Serverfehler: " + message;
 if ((int)res.StatusCode ==404)
 return "Nicht gefunden: " + message;
 if ((int)res.StatusCode ==400)
 return "Ungültige Eingabe: " + message;
 if ((int)res.StatusCode ==401 || (int)res.StatusCode ==403)
 return "Zugriff verweigert: " + message;

 return message;
 }

 if (!string.IsNullOrWhiteSpace(details))
 {
 return details;
 }
 }
 }
 catch
 {
 // ignore JSON parse errors
 }

 // Fallback: return plain body trimmed
 var trimmed = body.Trim();
 if (trimmed.Length >200)
 trimmed = trimmed.Substring(0,200) + "...";

 return $"Fehler ({(int)res.StatusCode}): {trimmed}";
 }
}

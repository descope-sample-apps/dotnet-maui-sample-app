using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescopeMauiSampleApplication.Services
{
    public class AuthServer
    {
        private HttpListener _listener;
        private readonly int _port = 8080; // Fixed port for simplicity
        private bool _isListening = false;

        public string RedirectUri => $"http://localhost:8080/callback";

        public async Task StartAsync()
        {
            if (_isListening) return;

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:8080/");

            try
            {
                _listener.Start();
                _isListening = true;

                // Handle incoming requests in background
                _ = Task.Run(async () =>
                {
                    while (_isListening && _listener.IsListening)
                    {
                        try
                        {
                            var context = await _listener.GetContextAsync();
                            _ = Task.Run(() => HandleRequest(context));
                        }
                        catch (Exception ex) when (!_isListening)
                        {
                            // Expected when stopping
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Server error: {ex.Message}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not start local server on port {_port}: {ex.Message}");
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                // Get all parameters from the callback URL
                var queryString = request.Url.Query; // e.g., "?code=abc123&state=xyz"

                Console.WriteLine($"Received callback: {request.Url}");

                // Create simple redirect HTML
                var html = CreateRedirectHtml(queryString);
                var buffer = Encoding.UTF8.GetBytes(html);

                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/html; charset=utf-8";
                response.StatusCode = 200;

                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();

                Console.WriteLine("Sent redirect response to browser");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
            }
        }

        private string CreateRedirectHtml(string queryString)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Authentication Complete</title>
</head>
<body>
    <p>Authentication successful! Returning to app...</p>
    <script>
        // Redirect to your app immediately
        window.location.href = 'myapp://auth/callback{queryString}';
       
        // Try to close the window after a short delay
        setTimeout(function() {{
            window.close();
        }}, 500);
    </script>
</body>
</html>";
        }

        public void Stop()
        {
            if (!_isListening) return;

            _isListening = false;
            try
            {
                _listener?.Stop();
                _listener?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping server: {ex.Message}");
            }
        }
    }
}

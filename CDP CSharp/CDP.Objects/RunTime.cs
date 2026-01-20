/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Net.WebSockets;
using CDP.Commands;
using CDP.Utils;

namespace CDP.Objects
{
    public class RunTime
    {
        public RunTime(ClientWebSocket WebSocket)
        {
            this.WebSocket = WebSocket;
        }

        private ClientWebSocket WebSocket { get; }

        public async Task<PropertyDescriptor[]> GetProperties(string ObjectId, bool OwnProperties, TimeSpan? TimeOut = null)
        {
            if (TimeOut == null) { TimeOut = TimeSpan.FromSeconds(10); }
            Stopwatch stopWatch = Stopwatch.StartNew();

            int commandId = 1;
            await this.WebSocket.SendAsync(new RuntimeGetPropertiesCommand(commandId, ObjectId, OwnProperties).Encode(),
                WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] responseBuffer = new byte[1024];
            StringBuilder responseBuilder = new StringBuilder();

            while (true)
            {
                if (stopWatch.Elapsed > TimeOut) { throw new TimeoutException(); }
                WebSocketReceiveResult result = await this.WebSocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);

                responseBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, result.Count)); // remove two args?
                // Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(responseBuilder.ToString()); Console.ResetColor();
                if (result.EndOfMessage == true)
                {
                    string response = responseBuilder.ToString();
                    responseBuilder.Clear();

                    CommandResult? commandResult = JsonSerializer.Deserialize<CommandResult>(response);
                    if (commandResult == null) { throw new InvalidOperationException(); }
                    if (commandResult.Id != commandId) { continue; }

                    JsonElement nodeElement = commandResult.Result.RootElement.GetProperty("result");
                    PropertyDescriptor[]? properties = JsonSerializer.Deserialize<PropertyDescriptor[]>(nodeElement.GetRawText());
                    if (properties == null) { throw new InvalidCastException(); }
                    return properties;
                }
            }
        }
    }
}
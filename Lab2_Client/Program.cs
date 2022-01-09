using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Utility;
using System.Text.Json;

namespace Lab2_Client
{
    class Program
    {
        const string okAnswer = "HttpStatusCode.Ok (200)";
        const string baseUri = "http://127.0.0.1:";
        const string pingMethod = "Ping/";
        const string getInputMethod = "GetInputData/";
        const string writeAnswerMethod = "WriteAnswer/";

        const string defaultConsoleMessage = "Write command number:\n1 - " + pingMethod + 
            "\n2 - " + getInputMethod + 
            "\n3 - " + writeAnswerMethod;


        private async static Task Main()
        {
            int port = -1;

            Console.WriteLine("Please, type necessary port below");
            port = Convert.ToInt32(Console.ReadLine());

            string currentUri = baseUri + port + "/";

            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient httpClient = new HttpClient(clientHandler);

            int option = -1;

            bool isWorking = true;

            Input input = null;
            Output output = null;
            HttpResponseMessage answer = null;
            StringContent content;
            bool isOK = false;

            string bufStr = "";

            while (!isOK)
            {
                string str = await httpClient.GetStringAsync(currentUri + pingMethod);

                if (str == okAnswer) isOK = true;

                Thread.Sleep(5);
            }

            while (isWorking)
            {
                Console.WriteLine(defaultConsoleMessage);

                option = Convert.ToInt32(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        bufStr = await httpClient.GetStringAsync(currentUri + pingMethod);
                        Console.WriteLine(bufStr);
                        break;
                    case 2:
                        bufStr = await httpClient.GetStringAsync(currentUri + getInputMethod);

                        try
                        {
                            input = new Input(bufStr);
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine("Json Exception occured during recieving input from server. Probably input was not a Json string.");
                        }
                        
                        output = new Output(input);
                        break;
                    case 3:
                        content = new StringContent(output.SerializeOutput());                        
                        httpClient.PostAsync(currentUri + writeAnswerMethod, content);
                        break;
                    case 0:
                        isWorking = false;
                        break;
                }
            }
        }
    }
}

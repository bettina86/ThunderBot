using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using ThunderBot.Weather;

namespace ThunderBot.Bot.Dialogs
{
    [Serializable]
    public class LocationDialog : IDialog<object>
    {
        // This is the entry point into the dialog
        public async Task StartAsync(IDialogContext context)
        {
            // Wait until a message comes in and call the continuation delegate
            context.Wait(MessageReceivedAsync);
        }

        // This is the continuation delegate, gets invoked every time a new message comes in
        async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            WeatherLocation weatherLoc = null;
            string theWeather = string.Empty;

            // Get the incoming activity
            var message = await argument;

            var ws = new WeatherService();
            weatherLoc = await ws.FindLocationCoordinatesAsync(message.Text);

            if (weatherLoc == null)
            {
                // Check to see if the location is already in the bot's state
                if (context.PrivateConversationData.TryGetValue("currentloc", out weatherLoc) == false)
                {
                    // We don't have the city, need to return a message saying that
                    theWeather = $"I queried Google and still didn't find anyting matching {message.Text}, mind entering a new location?";
                    await context.PostAsync(theWeather);
                    context.Wait(MessageReceivedAsync);
                }
            }

            // We have a location - save it to the conversation data
            context.PrivateConversationData.SetValue("currentloc", weatherLoc);

            // try to parse the input to see if there's a time
            var timeParser = new Chronic.Parser();
            var chronicResult = timeParser.Parse(message.Text);

            if (chronicResult != null && chronicResult.Start.HasValue)
            {
                theWeather = await ws.GetConditionsForLocationAtTimeAsync(weatherLoc, chronicResult.Start.Value);
            }
            else
            {
                theWeather = await ws.GetCurrentConditionsAsync(weatherLoc);
            }

            // Send something back
            await context.PostAsync(theWeather);

            // Wait for everything again
            context.Wait(MessageReceivedAsync);
        }

        // FLOW:
        // 1 - Check if passed in name is a city -> return current conditions
        // 2 - Check if passed in text is a time -> check if city is in memory -> return forecast for that time
        //      --> otherwise return error
    }
}
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

class Program
{
    static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        string baseUrl = "https://darknetdiaries.com/transcript/{0}/";
        string saveFile = @"C:\Users\username\Desktop\darknet\transcripts\AllTranscripts.txt";
        StreamWriter fileWriter = new StreamWriter(saveFile, append: true);

        for (int episode = 1; episode <= 143; episode++)
        {
            string episodeUrl = string.Format(baseUrl, episode);
            try
            {
                string response = await client.GetStringAsync(episodeUrl);
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var articleNode = htmlDoc.DocumentNode.SelectSingleNode("//article[@class='single-post']");
                if (articleNode != null)
                {
                    string transcriptText = articleNode.InnerText;
                    transcriptText = HtmlEntity.DeEntitize(transcriptText);

                    await fileWriter.WriteLineAsync($"Episode {episode} Transcript:\n{transcriptText}\n\n---\n\n");
                    Console.WriteLine($"Processed transcript for Episode {episode}");
                }
                else
                {
                    Console.WriteLine($"Article node not found for Episode {episode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing transcript for Episode {episode}: {ex.Message}");
            }

            await Task.Delay(1000);
        }

        fileWriter.Close();
    }
}

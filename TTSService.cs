using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NamazuTTS
{
    public class TTSService
    {
        private TextToSpeechClient _ttsClient;
        public TTSService()
        {
            var ttsBuilder = new TextToSpeechClientBuilder()
            {
                CredentialsPath = "gcloud.json"
            };
            _ttsClient = ttsBuilder.Build();
        }
        private async Task<SynthesizeSpeechResponse> CreateSpeech(string msg)
        {
            TextToSpeechClient client = TextToSpeechClient.Create();
            // The input can be provided as text or SSML.
            SynthesisInput input = new SynthesisInput
            {
                Text = msg
            };
            // You can specify a particular voice, or ask the server to pick based
            // on specified criteria.
            VoiceSelectionParams voiceSelection = new VoiceSelectionParams
            {
                LanguageCode = "en-US",
                SsmlGender = SsmlVoiceGender.Female
            };
            // The audio configuration determines the output format and speaking rate.
            AudioConfig audioConfig = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
            };
            SynthesizeSpeechResponse response = await client.SynthesizeSpeechAsync(input, voiceSelection, audioConfig);
            return response;
        }
        public async Task<bool> CreateSpeechToFile(string file, string msg)
        {
            file = "lavalink/" + file;
            //if (!File.Exists(file)) return false;
            using (Stream output = File.Create(file))
            {
                // response.AudioContent is a ByteString. This can easily be converted into
                // a byte array or written to a stream.
                var speech = await CreateSpeech(msg);
                speech.AudioContent.WriteTo(output);
            }
            return true;
        }
        public MemoryStream CreateSpeechToStream(string msg)
        {
            using(MemoryStream stream = new MemoryStream())
            {
                //CreateSpeech(msg).result.AudioContent.WriteTo(stream);
                return stream;
            }
        }
    }
}

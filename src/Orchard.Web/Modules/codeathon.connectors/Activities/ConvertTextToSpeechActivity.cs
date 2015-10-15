using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using codeathon.connectors.Services;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace codeathon.connectors.Activities
{
    public class ConvertTextToSpeechActivity : Task {
        private ITextToSppechFileService _iTextToSppechFileService;
        public ConvertTextToSpeechActivity(ITextToSppechFileService iTextToSppechFileService)
        {
            T = NullLocalizer.Instance;
            _iTextToSppechFileService = iTextToSppechFileService;
        }

        public Localizer T { get; set; }

        public override string Name
        {
            get { return "ConvertTextToSpeech"; }
        }

        public override LocalizedString Category
        {
            get { return T("Tweet"); }
        }

        public override LocalizedString Description
        {
            get { return T("Converts Text into speech"); }
        }

        public override string Form
        {
            get { return "ConvertTextToSpeech"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            return new[] {
                T("Done")
            };
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext)
        {
            var textblock = activityContext.GetState<string>("TextBlock");
            var fileName = activityContext.GetState<string>("FileName");

            if (!String.IsNullOrWhiteSpace(textblock))
            {
                // Initialize a new instance of the speech synthesizer.
                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                using (MemoryStream streamAudio = new MemoryStream())
                {

                    // Create a SoundPlayer instance to play the output audio file.
                    System.Media.SoundPlayer m_SoundPlayer = new System.Media.SoundPlayer();

                    // Configure the synthesizer to output to an audio stream.
                    synth.SetOutputToDefaultAudioDevice();
                    synth.SetOutputToWaveStream(streamAudio);

                    synth.Speak(textblock);
                    // Speak a phrase.
                    streamAudio.Position = 0;
                    if (streamAudio.CanSeek) streamAudio.Seek(0, System.IO.SeekOrigin.Begin);
                    m_SoundPlayer.Stream = null;
                    m_SoundPlayer.Stream = streamAudio;
                    m_SoundPlayer.Play();

                    // Set the synthesizer output to null to release the stream. 
                    //synth.SetOutputToNull();

                    // Insert code to persist or process the stream contents here.
                    if (streamAudio.CanSeek) streamAudio.Seek(0, System.IO.SeekOrigin.Begin);
                    if (!string.IsNullOrWhiteSpace(fileName))
                    _iTextToSppechFileService.AddFile(fileName, streamAudio);
                }

                yield return T("Done");
            }   
          
        }

        private void Reader_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

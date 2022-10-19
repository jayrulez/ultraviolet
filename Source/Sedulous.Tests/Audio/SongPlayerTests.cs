using System;
using NUnit.Framework;
using Sedulous.Audio;
using Sedulous.TestApplication;
using Sedulous.TestFramework;

namespace Sedulous.Tests.Audio
{
    [TestFixture]
    public class SongPlayerTests : SedulousApplicationTestFramework
    {
        [Test]
        [TestCase(AudioImplementation.BASS)]
        [TestCase(AudioImplementation.FMOD)]
        [Category("Audio")]
        public void SongPlayer_PlaySetsVolumePitchAndPan(AudioImplementation audioImplementation)
        {
            var songPlayer = default(SongPlayer);
            var song = default(Song);

            GivenAnSedulousApplicationWithNoWindow()
                .WithAudioImplementation(audioImplementation)
                .WithInitialization(uv => uv.GetAudio().AudioMuted = true)
                .WithContent(content =>
                {
                    songPlayer = SongPlayer.Create();
                    song = content.Load<Song>("Songs/Deep Haze");

                    songPlayer.Play(song, 0.25f, 0.50f, 0.75f, false);

                    TheResultingValue(songPlayer.Volume).ShouldBe(0.25f);
                    TheResultingValue(songPlayer.Pitch).ShouldBe(content.Sedulous.GetAudio().Capabilities.SupportsPitchShifting ? 0.50f : 0.00f);
                    TheResultingValue(songPlayer.Pan).ShouldBe(0.75f);
                })
                .OnUpdate((app, time) =>
                {
                    songPlayer.Update(time);
                })
                .RunForOneFrame();
        }

        [Test]
        [TestCase(AudioImplementation.BASS)]
        [TestCase(AudioImplementation.FMOD)]
        [Category("Audio")]
        public void SongPlayer_PlayResetsVolumePitchAndPanWhenNotSpecified(AudioImplementation audioImplementation)
        {
            var songPlayer = default(SongPlayer);
            var song = default(Song);

            GivenAnSedulousApplicationWithNoWindow()
                .WithAudioImplementation(audioImplementation)
                .WithInitialization(uv => uv.GetAudio().AudioMuted = true)
                .WithContent(content =>
                {
                    songPlayer = SongPlayer.Create();
                    song = content.Load<Song>("Songs/Deep Haze");

                    songPlayer.Play(song, 0.25f, 0.50f, 0.75f, false);
                    songPlayer.Stop();
                    songPlayer.Play(song, false);

                    TheResultingValue(songPlayer.Volume).ShouldBe(1f);
                    TheResultingValue(songPlayer.Pitch).ShouldBe(0f);
                    TheResultingValue(songPlayer.Pan).ShouldBe(0f);
                })
                .OnUpdate((app, time) =>
                {
                    songPlayer.Update(time);
                })
                .RunForOneFrame();
        }

        [Test]
        [TestCase(AudioImplementation.BASS)]
        [TestCase(AudioImplementation.FMOD)]
        [Category("Audio")]
        public void SongPlayer_SlidesVolumeCorrectly(AudioImplementation audioImplementation)
        {
            var songPlayer = default(SongPlayer);
            var song = default(Song);

            GivenAnSedulousApplicationWithNoWindow()
                .WithAudioImplementation(audioImplementation)
                .WithInitialization(uv => uv.GetAudio().AudioMuted = true)
                .WithContent(content =>
                {
                    songPlayer = SongPlayer.Create();
                    song = content.Load<Song>("Songs/Deep Haze");

                    songPlayer.Play(song, false);

                    TheResultingValue(songPlayer.Volume).ShouldBe(1f);

                    songPlayer.SlideVolume(0f, TimeSpan.FromSeconds(1));
                })
                .OnUpdate((app, time) =>
                {
                    songPlayer.Update(time);
                })
                .RunFor(TimeSpan.FromSeconds(2));

            TheResultingValue(songPlayer.Volume).ShouldBe(0f);
        }

        [Test]
        [TestCase(AudioImplementation.BASS)]
        [TestCase(AudioImplementation.FMOD)]
        [Category("Audio")]
        public void SongPlayer_SlidesPitchCorrectly(AudioImplementation audioImplementation)
        {
            var songPlayer = default(SongPlayer);
            var song = default(Song);

            var supported = false;

            GivenAnSedulousApplicationWithNoWindow()
                .WithAudioImplementation(audioImplementation)
                .WithInitialization(uv => uv.GetAudio().AudioMuted = true)
                .WithContent(content =>
                {
                    songPlayer = SongPlayer.Create();
                    song = content.Load<Song>("Songs/Deep Haze");

                    songPlayer.Play(song, false);

                    TheResultingValue(songPlayer.Pitch).ShouldBe(0f);

                    songPlayer.SlidePitch(-1f, TimeSpan.FromSeconds(1));

                    supported = content.Sedulous.GetAudio().Capabilities.SupportsPitchShifting;
                })
                .OnUpdate((app, time) =>
                {
                    songPlayer.Update(time);
                })
                .RunFor(TimeSpan.FromSeconds(2));

            TheResultingValue(songPlayer.Pitch).ShouldBe(supported ? -1f : 0f);
        }

        [Test]
        [TestCase(AudioImplementation.BASS)]
        [TestCase(AudioImplementation.FMOD)]
        [Category("Audio")]
        public void SongPlayer_SlidesPanCorrectly(AudioImplementation audioImplementation)
        {
            var songPlayer = default(SongPlayer);
            var song = default(Song);

            GivenAnSedulousApplicationWithNoWindow()
                .WithAudioImplementation(audioImplementation)
                .WithInitialization(uv => uv.GetAudio().AudioMuted = true)
                .WithContent(content =>
                {
                    songPlayer = SongPlayer.Create();
                    song = content.Load<Song>("Songs/Deep Haze");

                    songPlayer.Play(song, false);

                    TheResultingValue(songPlayer.Pan).ShouldBe(0f);

                    songPlayer.SlidePan(-1f, TimeSpan.FromSeconds(1));
                })
                .OnUpdate((app, time) =>
                {
                    songPlayer.Update(time);
                })
                .RunFor(TimeSpan.FromSeconds(2));

            TheResultingValue(songPlayer.Pan).ShouldBe(-1f);
        }

        [Test]
        [TestCase(AudioImplementation.BASS)]
        [TestCase(AudioImplementation.FMOD)]
        [Category("Audio")]
        public void SongPlayer_ThrowsExceptionIfVolumeSetWhileNotPlaying(AudioImplementation audioImplementation)
        {
            var songPlayer = default(SongPlayer);

            Assert.That(() =>
            {
                GivenAnSedulousApplicationWithNoWindow()
                    .WithAudioImplementation(audioImplementation)
                    .WithInitialization(uv => uv.GetAudio().AudioMuted = true)
                    .WithContent(content =>
                    {
                        songPlayer = SongPlayer.Create();
                        songPlayer.Volume = 0f;
                    })
                    .OnUpdate((app, time) =>
                    {
                        songPlayer.Update(time);
                    })
                    .RunFor(TimeSpan.FromSeconds(1));
            },
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        [TestCase(AudioImplementation.BASS)]
        [TestCase(AudioImplementation.FMOD)]
        [Category("Audio")]
        public void SongPlayer_ThrowsExceptionIfPitchSetWhileNotPlaying(AudioImplementation audioImplementation)
        {
            var songPlayer = default(SongPlayer);

            Assert.That(() =>
            {
                GivenAnSedulousApplicationWithNoWindow()
                    .WithAudioImplementation(audioImplementation)
                    .WithInitialization(uv => uv.GetAudio().AudioMuted = true)
                    .WithContent(content =>
                    {
                        songPlayer = SongPlayer.Create();
                        songPlayer.Pitch = -1f;
                    })
                    .OnUpdate((app, time) =>
                    {
                        songPlayer.Update(time);
                    })
                    .RunFor(TimeSpan.FromSeconds(1));
            },
            Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        [TestCase(AudioImplementation.BASS)]
        [TestCase(AudioImplementation.FMOD)]
        [Category("Audio")]
        public void SongPlayer_ThrowsExceptionIfPanSetWhileNotPlaying(AudioImplementation audioImplementation)
        {
            var songPlayer = default(SongPlayer);

            Assert.That(() =>
            {
                GivenAnSedulousApplicationWithNoWindow()
                    .WithAudioImplementation(audioImplementation)
                    .WithInitialization(uv => uv.GetAudio().AudioMuted = true)
                    .WithContent(content =>
                    {
                        songPlayer = SongPlayer.Create();
                        songPlayer.Pan = 0f;
                    })
                    .OnUpdate((app, time) =>
                    {
                        songPlayer.Update(time);
                    })
                    .RunFor(TimeSpan.FromSeconds(1));
            },
            Throws.TypeOf<InvalidOperationException>());
        }
    }
}

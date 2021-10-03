using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.MediaFiles.MediaInfo;
using NzbDrone.Core.MediaFiles.MovieImport.Aggregation.Aggregators.Augmenters.Language;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Test.Framework;

namespace NzbDrone.Core.Test.MediaFiles.MovieImport.Aggregation.Aggregators.Augmenters.Language
{
    [TestFixture]
    public class AugmentLanguageFromMediaInfoFixture : CoreTest<AugmentLanguageFromMediaInfo>
    {
        [Test]
        public void should_return_null_if_media_info_is_null()
        {
            var localMovie = Builder<LocalMovie>.CreateNew()
                                                    .With(l => l.MediaInfo = null)
                                                    .Build();

            Subject.AugmentLanguage(localMovie, null).Should().Be(null);
        }

        [Test]
        public void should_return_language_for_single_known_language()
        {
            var mediaInfo = new MediaInfoModel(videoFormat: "avc");

            var localMovie = Builder<LocalMovie>.CreateNew()
                                                    .With(l => l.MediaInfo = mediaInfo)
                                                    .Build();

            var result = Subject.AugmentLanguage(localMovie, null);

            result.Languages.Count.Should().Be(1);
            result.Languages.Should().Contain(Core.Languages.Language.English);
        }

        [Test]
        public void should_only_return_one_when_language_duplicated()
        {
            var mediaInfo = new MediaInfoModel(new[] { "eng", "eng" });

            var localMovie = Builder<LocalMovie>.CreateNew()
                                                    .With(l => l.MediaInfo = mediaInfo)
                                                    .Build();

            var result = Subject.AugmentLanguage(localMovie, null);

            result.Languages.Count.Should().Be(1);
            result.Languages.Should().Contain(Core.Languages.Language.English);
        }

        [Test]
        public void should_return_null_if_all_unknown()
        {
            var mediaInfo = new MediaInfoModel(new[] { "pirate", "pirate" });

            var localMovie = Builder<LocalMovie>.CreateNew()
                                                    .With(l => l.MediaInfo = mediaInfo)
                                                    .Build();

            var result = Subject.AugmentLanguage(localMovie, null);

            result.Should().BeNull();
        }

        [Test]
        public void should_return_known_languages_only()
        {
            var mediaInfo = new MediaInfoModel(new[] { "eng", "pirate" });

            var localMovie = Builder<LocalMovie>.CreateNew()
                                                    .With(l => l.MediaInfo = mediaInfo)
                                                    .Build();

            var result = Subject.AugmentLanguage(localMovie, null);

            result.Languages.Count.Should().Be(1);
            result.Languages.Should().Contain(Core.Languages.Language.English);
        }

        [Test]
        public void should_return_multiple_known_languages()
        {
            var mediaInfo = new MediaInfoModel(new[] { "eng", "ger" });

            var localMovie = Builder<LocalMovie>.CreateNew()
                                                    .With(l => l.MediaInfo = mediaInfo)
                                                    .Build();

            var result = Subject.AugmentLanguage(localMovie, null);

            result.Languages.Count.Should().Be(2);
            result.Languages.Should().Contain(Core.Languages.Language.English);
            result.Languages.Should().Contain(Core.Languages.Language.German);
        }
    }
}

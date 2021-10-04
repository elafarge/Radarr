using System;
using System.Collections.Generic;
using NzbDrone.Core.MediaFiles;
using Radarr.Http;
using Radarr.Http.REST;

namespace Radarr.Api.V3.Movies
{
    [V3ApiController("rename")]
    public class RenameMovieController : RestController<RenameMovieResource>
    {
        private readonly IRenameMovieFileService _renameMovieFileService;

        public RenameMovieController(IRenameMovieFileService renameMovieFileService)
        {
            _renameMovieFileService = renameMovieFileService;
        }

        public override RenameMovieResource GetResourceById(int id)
        {
            throw new NotImplementedException();
        }

        private List<RenameMovieResource> GetMovies(int movieId)
        {
            return _renameMovieFileService.GetRenamePreviews(movieId).ToResource();
        }
    }
}

﻿using BeatSyncLib.Playlists;
using Newtonsoft.Json;
using SongFeedReaders.Readers;
using SongFeedReaders.Readers.BeatSaver;
using System;

namespace BeatSyncLib.Configs
{
    public class BeatSaverFavoriteMappers : FeedConfigBase
    {
        #region Defaults
        protected override bool DefaultEnabled => true;

        protected override int DefaultMaxSongs => 20;

        protected override bool DefaultCreatePlaylist => true;

        protected override PlaylistStyle DefaultPlaylistStyle => PlaylistStyle.Append;

        protected override BuiltInPlaylist DefaultFeedPlaylist => BuiltInPlaylist.BeatSaverFavoriteMappers;

        protected bool DefaultSeparateMapperPlaylists => false;
        #endregion
        [JsonIgnore]
        private bool? _separateMapperPlaylists;

        public bool SeparateMapperPlaylists
        {
            get
            {
                if (_separateMapperPlaylists == null)
                {
                    _separateMapperPlaylists = DefaultSeparateMapperPlaylists;
                    SetConfigChanged();
                }
                return _separateMapperPlaylists ?? DefaultSeparateMapperPlaylists;
            }
            set
            {
                if (_separateMapperPlaylists == value)
                    return;
                _separateMapperPlaylists = value;
                SetConfigChanged();
            }
        }

        public override void FillDefaults()
        {
            base.FillDefaults();
            var _ = SeparateMapperPlaylists;
        }

        public override IFeedSettings ToFeedSettings()
        {
            return new BeatSaverFeedSettings((int)BeatSaverFeedName.Author)
            {
                MaxSongs = this.MaxSongs
            };
        }

        public override bool ConfigMatches(ConfigBase other)
        {
            if (other is BeatSaverFavoriteMappers castOther)
            {
                if (!base.ConfigMatches(castOther))
                    return false;
                if (SeparateMapperPlaylists != castOther.SeparateMapperPlaylists)
                    return false;
            }
            else
                return false;
            return true;
        }
    }

    public class BeatSaverLatest : FeedConfigBase
    {
        #region Defaults
        protected override bool DefaultEnabled => false;

        protected override int DefaultMaxSongs => 20;

        protected override bool DefaultCreatePlaylist => true;

        protected override PlaylistStyle DefaultPlaylistStyle => PlaylistStyle.Append;

        protected override BuiltInPlaylist DefaultFeedPlaylist => BuiltInPlaylist.BeatSaverLatest;
        #endregion

        public override IFeedSettings ToFeedSettings()
        {
            return new BeatSaverFeedSettings((int)BeatSaverFeedName.Latest)
            {
                MaxSongs = this.MaxSongs
            };
        }
    }

    public class BeatSaverHot : FeedConfigBase
    {
        #region Defaults
        protected override bool DefaultEnabled => false;

        protected override int DefaultMaxSongs => 20;

        protected override bool DefaultCreatePlaylist => true;

        protected override PlaylistStyle DefaultPlaylistStyle => PlaylistStyle.Append;

        protected override BuiltInPlaylist DefaultFeedPlaylist => BuiltInPlaylist.BeatSaverHot;
        #endregion

        public override IFeedSettings ToFeedSettings()
        {
            return new BeatSaverFeedSettings((int)BeatSaverFeedName.Hot)
            {
                MaxSongs = this.MaxSongs
            };
        }
    }

    [Obsolete("Only has really old play data.")]
    public class BeatSaverPlays : FeedConfigBase
    {
        #region Defaults
        protected override bool DefaultEnabled => false;

        protected override int DefaultMaxSongs => 20;

        protected override bool DefaultCreatePlaylist => true;

        protected override PlaylistStyle DefaultPlaylistStyle => PlaylistStyle.Append;

        protected override BuiltInPlaylist DefaultFeedPlaylist => BuiltInPlaylist.BeatSaverPlays;
        #endregion

        public override IFeedSettings ToFeedSettings()
        {
            return new BeatSaverFeedSettings((int)BeatSaverFeedName.Plays)
            {
                MaxSongs = this.MaxSongs
            };
        }
    }

    public class BeatSaverDownloads : FeedConfigBase
    {
        #region Defaults
        protected override bool DefaultEnabled => false;

        protected override int DefaultMaxSongs => 20;

        protected override bool DefaultCreatePlaylist => true;

        protected override PlaylistStyle DefaultPlaylistStyle => PlaylistStyle.Append;

        protected override BuiltInPlaylist DefaultFeedPlaylist => BuiltInPlaylist.BeatSaverDownloads;
        #endregion

        public override IFeedSettings ToFeedSettings()
        {
            return new BeatSaverFeedSettings((int)BeatSaverFeedName.Downloads)
            {
                MaxSongs = this.MaxSongs
            };
        }
    }


}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BeatSync;
using System.Collections.Concurrent;
using BeatSync.Utilities;

namespace BeatSync.Playlists
{
    public static class PlaylistManager
    {
        static PlaylistManager()
        {
            AvailablePlaylists = new Dictionary<BuiltInPlaylist, Playlist>();

            //AvailablePlaylists.TryAdd("BeatSyncPlaylist", null);
            //AvailablePlaylists.TryAdd("BeatSyncBSaberBookmarks", null);
            //AvailablePlaylists.TryAdd("BeatSyncBSaberFollows", null);
            //AvailablePlaylists.TryAdd("BeatSyncBSaberCuratorRecommended", null);
            //AvailablePlaylists.TryAdd("BeatSyncScoreSaberTopRanked", null);
            //AvailablePlaylists.TryAdd("BeatSyncFavoriteMappers", null);
            //AvailablePlaylists.TryAdd("BeatSyncRecent", null);
            foreach (var key in DefaultPlaylists.Keys)
            {
                AvailablePlaylists.Add(key, null);
            }

        }
        private const string PlaylistPath = @"Playlists";

        private static Dictionary<BuiltInPlaylist, Playlist> AvailablePlaylists; // Doesn't need to be concurrent, basically readonly

        public static Dictionary<BuiltInPlaylist, Playlist> DefaultPlaylists = new Dictionary<BuiltInPlaylist, Playlist>()
        {
            {BuiltInPlaylist.BeatSyncAll, new Playlist("BeatSyncPlaylist", "BeatSync Playlist", "BeatSync", "1") },
            {BuiltInPlaylist.BeastSaberBookmarks, new Playlist("BeatSyncBSaberBookmarks", "BeastSaber Bookmarks", "BeatSync", "1") },
            {BuiltInPlaylist.BeastSaberFollows, new Playlist("BeatSyncBSaberFollows", "BeastSaber Follows", "BeatSync", "1") },
            {BuiltInPlaylist.BeastSaberCurator, new Playlist("BeatSyncBSaberCuratorRecommended", "Curator Recommended", "BeatSync", "1") },
            {BuiltInPlaylist.ScoreSaberTopRanked, new Playlist("BeatSyncScoreSaberTopRanked", "ScoreSaber Top Ranked", "BeatSync", "1") },
            {BuiltInPlaylist.ScoreSaberLatestRanked, new Playlist("BeatSyncScoreSaberLatestRanked", "ScoreSaber Latest Ranked", "BeatSync", "1") },
            {BuiltInPlaylist.ScoreSaberTopPlayed, new Playlist("BeatSyncScoreSaberTopPlayed", "ScoreSaber Top Played", "BeatSync", "1") },
            {BuiltInPlaylist.ScoreSaberTrending, new Playlist("BeatSyncScoreSaberTrending", "ScoreSaber Trending", "BeatSync", "1") },
            {BuiltInPlaylist.BeatSaverFavoriteMappers, new Playlist("BeatSyncFavoriteMappers", "Favorite Mappers", "BeatSync", "1") },
            {BuiltInPlaylist.BeatSaverLatest, new Playlist("BeatSyncBeatSaverLatest", "BeatSaver Latest", "BeatSync", "1") },
            {BuiltInPlaylist.BeatSaverHot, new Playlist("BeatSyncBeatSaverHot", "Beat Saver Hot", "BeatSync", "1") },
            {BuiltInPlaylist.BeatSaverPlays, new Playlist("BeatSyncBeatSaverPlays", "Beat Saver Plays", "BeatSync", "1") },
            {BuiltInPlaylist.BeatSaverDownloads, new Playlist("BeatSyncBeatSaverDownloads", "Beat Saver Downloads", "BeatSync", "1") },
            {BuiltInPlaylist.BeatSyncRecent, new Playlist("BeatSyncRecent", "BeatSync Recent Songs", "BeatSync", "1") }
        };

        public static Dictionary<int, Playlist> LegacyPlaylists = new Dictionary<int, Playlist>()
        {
            { 0, new Playlist("SyncSaberPlaylist", "SyncSaber Playlist", "SyncSaber", "1") },
            { 1, new Playlist("SyncSaberBookmarksPlaylist", "BeastSaber Bookmarks", "brian91292", "1") },
            { 2, new Playlist("SyncSaberFollowingsPlaylist", "BeastSaber Followings", "brian91292", "1") },
            { 3, new Playlist("SyncSaberCuratorRecommendedPlaylist", "BeastSaber Curator Recommended", "brian91292", "1") },
            { 4, new Playlist("ScoreSaberTopRanked", "ScoreSaber Top Ranked", "SyncSaber", "1") }
        };


        /// <summary>
        /// Retrieves the specified playlist. If the playlist doesn't exist, creates one using the default.
        /// </summary>
        /// <param name="builtInPlaylist"></param>
        /// <returns></returns>
        public static Playlist GetPlaylist(BuiltInPlaylist builtInPlaylist)
        {
            Playlist playlist = null;
            if (AvailablePlaylists.TryGetValue(builtInPlaylist, out playlist))
            {
                if (playlist == null)
                {
                    var defPlaylist = DefaultPlaylists[builtInPlaylist];
                    var path = FileIO.GetPlaylistFilePath(defPlaylist.FileName);
                    if (string.IsNullOrEmpty(path)) // If GetPlaylistFilePath returned null, the file doesn't exist
                    {
                        if (AvailablePlaylists[builtInPlaylist] == null)
                            AvailablePlaylists[builtInPlaylist] = defPlaylist;
                        playlist = defPlaylist;
                    }
                    else
                    {
                        playlist = JsonConvert.DeserializeObject<Playlist>(File.ReadAllText(path));
                        playlist.FileName = path;
                        Logger.log.Critical($"Playlist FileName is {playlist.FileName}");
                    }
                }
            }
            Logger.log.Critical($"Returning {playlist?.FileName}: {playlist?.Title} for {builtInPlaylist.ToString()}");
            return playlist;
        }



        public static void ConvertLegacyPlaylists()
        {
            foreach (var playlistPair in LegacyPlaylists)
            {
                var legPlaylist = FileIO.ReadPlaylist(playlistPair.Value);
                var songCount = legPlaylist?.Songs.Count ?? 0;
                if (songCount > 0)
                {
                    var newPlaylist = DefaultPlaylists.Values.ElementAt(playlistPair.Key);
                    newPlaylist.Songs = newPlaylist.Songs.Union(legPlaylist.Songs).ToList();
                    foreach (var song in newPlaylist.Songs)
                    {
                        song.AddPlaylist(newPlaylist);
                        //MasterList.AddOrUpdate(song.Hash, song, (hash, existingSong) =>
                        //{
                        //    existingSong.AddPlaylist(newPlaylist);
                        //    return existingSong;
                        //});
                    }
                    FileIO.WritePlaylist(DefaultPlaylists.Values.ElementAt(playlistPair.Key));
                }
            }
        }
    }

    public enum BuiltInPlaylist
    {
        BeatSyncAll = 0,
        BeastSaberBookmarks = 1,
        BeastSaberFollows = 2,
        BeastSaberCurator = 3,
        ScoreSaberTopRanked = 4,
        ScoreSaberLatestRanked = 5,
        ScoreSaberTopPlayed = 6,
        ScoreSaberTrending = 7,
        BeatSaverFavoriteMappers = 8,
        BeatSaverLatest = 9,
        BeatSaverHot = 10,
        BeatSaverPlays = 11,
        BeatSaverDownloads = 12,
        BeatSyncRecent = 13
    }
}

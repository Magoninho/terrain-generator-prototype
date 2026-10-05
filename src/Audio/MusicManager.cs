using Raylib_cs;

namespace terrain_prototype_raylib.src.Audio;

public class MusicManager
{
    public Dictionary<string, List<Music>> Playlists { get; set; } = new();

    private int CurrentTrackIndex;
    private string CurrentPlaylistName = "";
    private Music CurrentMusic;

    public void CreatePlaylist(string playlistName, string[] filenames)
    {
        Playlists[playlistName] = new List<Music>();

        foreach (var filename in filenames)
        {
            Music music = Raylib.LoadMusicStream(filename);

            music.Looping = false;

            Playlists[playlistName].Add(music);
        }
    }

    public void Play(string playlistName)
    {
        if (!Playlists.TryGetValue(playlistName, out var playlist))
        {
            Console.WriteLine("failed to load playlist " + playlistName);
            return;
        }

        if (playlist.Count == 0)
            return;

        CurrentTrackIndex = 0;
        CurrentPlaylistName = playlistName;
        CurrentMusic = playlist[CurrentTrackIndex];

        Raylib.PlayMusicStream(CurrentMusic);
    }

    public void NextTrack()
    {
        List<Music> currentPlaylist = Playlists[CurrentPlaylistName];

        if (currentPlaylist.Count == 0)
            return;

        Raylib.StopMusicStream(CurrentMusic);

        CurrentTrackIndex =
            (CurrentTrackIndex + 1) % currentPlaylist.Count;

        // This was important!
        CurrentMusic = currentPlaylist[CurrentTrackIndex];

        Raylib.PlayMusicStream(CurrentMusic);
    }

    public void Update()
    {
        Raylib.UpdateMusicStream(CurrentMusic);

        float timePlayed = Raylib.GetMusicTimePlayed(CurrentMusic);
        float timeLength = Raylib.GetMusicTimeLength(CurrentMusic);

        if (timePlayed >= timeLength - 0.05f)
        {
            NextTrack();
        }
    }
}
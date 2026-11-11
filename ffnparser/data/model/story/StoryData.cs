namespace ffnparser.data.model.story;

using System;
using System.Collections.Generic;

using ffnparser.data.model.addresses;

public class StoryData
{
    private readonly Dictionary<int, ChapterData> _chapters;

    public UserAddress? AuthorAddress
    {
        get; set;
    }

    public int ChapterCount => _chapters.Count;

    public IReadOnlyDictionary<int, ChapterData> Chapters => _chapters;

    public DateTime DateAdded
    {
        get; set;
    }

    public DateTime DateLastVisited
    {
        get; set;
    }

    public string Genre { get; set; } = "Unknown";

    public string MediaType { get; set; } = "Uncategorized";

    public StoryAddress StoryAddress
    {
        get; set;
    }

    public string StoryTitle
    {
        get; set;
    }

    public StoryData(string storyTitle, StoryAddress storyAddress, UserAddress authorAddress = null, DateTime? dateCreated = null, DateTime? dateLastVisited = null)
    {
        StoryTitle = storyTitle;
        AuthorAddress = authorAddress;
        StoryAddress = storyAddress;
        DateAdded = dateCreated ?? DateTime.Now;
        DateLastVisited = dateLastVisited ?? DateTime.Now;
        _chapters = new Dictionary<int, ChapterData>();
    }

    public StoryData(StoryData other)
    {
        StoryTitle = other.StoryTitle;
        AuthorAddress = other.AuthorAddress;
        StoryAddress = other.StoryAddress;
        MediaType = other.MediaType;
        Genre = other.Genre;
        _chapters = (Dictionary<int, ChapterData>)other.Chapters;
    }

    public void AddChapter(int index, ChapterData chapter)
    {
        if( _chapters.ContainsKey(index) )
        {
            byte[] current = _chapters[index].GetCrc32();
            if( chapter.GetCrc32() != current )
            {
                _chapters[index] = chapter;
            }
            return;
        }

        _chapters.Add(index, chapter);
    }

    public ChapterData GetChapter(int index)
    {
        return _chapters[index];
    }

    public void RemoveChapter(int index)
    {
        _chapters.Remove(index);
    }
}
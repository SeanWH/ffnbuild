namespace ffnparser.data.model.story;

using System;
using System.Collections.Generic;
using System.IO.Hashing;

using System.Text;

public class ChapterData
{
    public int ChapterId
    {
        get; set;
    }

    public string ChapterText
    {
        get
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach( string line in Paragraphs )
            {
                stringBuilder.AppendLine(line);
            }
            return stringBuilder.ToString();
        }

        set
        {
            if( Paragraphs != null && Paragraphs.Count > 0 )
            {
                Paragraphs.Clear();
            }
            else
            {
                Paragraphs = new List<string>();
            }

            string[] lines = value.Split('\n');
            foreach( string line in lines )
            {
                Paragraphs.Add(line);
            }
        }
    }

    public string ChapterTitle { get; set; } = string.Empty;

    public List<string> Paragraphs
    {
        get; set;
    }

    public string StoryFanfictionNetId { get; set; } = string.Empty;

    public ChapterData()
    {
        Paragraphs = new List<string>();
    }

    public ChapterData(int chapterId, string chapterTitle, List<string> paras, string storyId)
    {
        ChapterId = chapterId;
        ChapterTitle = chapterTitle;
        Paragraphs = paras;
        StoryFanfictionNetId = storyId;
    }

    public byte[] GetCrc32()
    {
        byte[] data = Encoding.UTF8.GetBytes(ChapterText);
        return Crc32.Hash(new ReadOnlySpan<byte>(data));
    }
}
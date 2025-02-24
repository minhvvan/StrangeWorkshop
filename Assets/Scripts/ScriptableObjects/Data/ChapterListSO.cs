using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterList", menuName = "SO/Chapter/ChapterList")]
public class ChapterListSO : ScriptableObject
{
    [Header("Chapters")]
    public List<ChapterDataSO> chapters = new List<ChapterDataSO>();

    public ChapterDataSO GetChapterData(string chapterId)
    {
        return chapters.Find(c => c.chapterId == chapterId);
    }
    
    public ChapterDataSO GetChapterData(int chapterIndex)
    {
        return chapters.Find(c => c.chapterId == $"{Constants.CHAPTER_NAME_PREFIX + (chapterIndex + 1)}");
    }

    public ChapterDataSO GetNextChapter(ChapterDataSO currentChapter)
    {
        int currentIndex = chapters.FindIndex(c => c == currentChapter);
        if (currentIndex < chapters.Count - 1)
            return chapters[currentIndex + 1];
        return null;
    }

    public ChapterDataSO GetFirstChapter()
    {
        if (chapters.Count != 0) return chapters[0];
        return null;
    }
}
using System.Collections.Generic;

namespace BookSearch.Model
{
    public class OCRModel
    {
        public bool isAngleDetected { get; set; }
        public string language { get; set; }
        public string orientation { get; set; }
        public List<Region> regions { get; set; }
        public double textAngle { get; set; }
    }

    public class Region
    {
        public string boundingBox { get; set; }
        public List<Line> lines { get; set; }
    }

    public class Line
    {
        public string boundingBox { get; set; }
        public bool isVertical { get; set; }
        public List<Word> words { get; set; }
    }
    public class Word
    {
        public string boundingBox { get; set; }
        public string text { get; set; }
    }
}
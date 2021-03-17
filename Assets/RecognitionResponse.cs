using System;

namespace Assets
{
    [Serializable]
    public class RecognitionResponse
    {
        public string id;

        public string project;

        public string tteration;

        public DateTime created;

        public Prediction[] predictions;

        public RecognitionResponse()
        {
        }
    }

    [Serializable]
    public class BoundingBox
    {
        public double left;

        public double top;

        public double width;

        public double height;

        public BoundingBox()
        {
        }
    }

    [Serializable]
    public class Prediction
    {
        public double probability;

        public string tagId;

        public string tagName;

        public BoundingBox boundingBox;

        public Prediction()
        {
        }
    }
}

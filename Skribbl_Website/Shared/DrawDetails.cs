namespace Skribbl_Website.Shared
{
    public class DrawDetails
    {
        public DrawDetails(float x, float y, bool isDown, string color, float size)
        {
            X = x;
            Y = y;
            IsDown = isDown;
            Color = color;
            Size = size;
        }

        public DrawDetails()
        {
        }

        public float X { get; set; }
        public float Y { get; set; }
        public bool IsDown { get; set; }
        public string Color { get; set; }
        public float Size { get; set; }
    }
}
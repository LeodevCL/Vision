using System.Collections.ObjectModel;

namespace Vision
{
    public class RotationList : ObservableCollection<Rotation>
    {
        public double getAngle(Picture picture)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Path.Equals(picture.Path))
                {
                    return this[i].Angle;
                }
            }

            return 0;
        }
    }

    public class Rotation
    {
        public string Path
        {
            get; set;
        }
        public double Angle
        {
            get; set;
        }

        public Rotation()
        {

        }

        public Rotation(string path, double angle)
        {
            Path = path;
            Angle = angle;
        }
    }
}

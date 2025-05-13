namespace MapRoutingLogic
{
    // initially classes from chat-gp

    public class Query
    {
        public double SourceX { get; set; }
        public double SourceY { get; set; }
        public double DestinationX { get; set; }
        public double DestinationY { get; set; }
        public double R { get; set; }

        public Query(double sx, double sy, double dx, double dy, double r)
        {
            SourceX = sx;
            SourceY = sy;
            DestinationX = dx;
            DestinationY = dy;
            R = r;
        }
        public override string ToString()
        {
            return $"{SourceX} {SourceY} {DestinationX} {DestinationY} {R}";
        }
    }
}
